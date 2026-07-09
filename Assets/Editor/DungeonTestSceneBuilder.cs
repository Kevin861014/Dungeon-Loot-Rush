using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DungeonLootRush.Combat;
using DungeonLootRush.Core;
using DungeonLootRush.Dungeon;
using DungeonLootRush.Economy;
using DungeonLootRush.Enemy;
using DungeonLootRush.Player;

namespace DungeonLootRush.EditorTools
{
    /// One-click assembler for a minimal playable test: a Player prefab (using
    /// the finished 8-direction art), a Goblin prefab (using the 0x72
    /// DungeonTilesetII frames), a single combat Room prefab wired into the
    /// existing DungeonGenerator/StageManager pipeline, and a scene with a
    /// camera and virtual joystick so the whole loop can be pressed Play on.
    ///
    /// Known simplifications for this first pass (not bugs, just scope cuts):
    /// - Enemies don't collide with walls (their collider is a trigger, needed
    ///   for contact-damage detection instead of physical blocking).
    /// - The room is a flat grid of individual floor/wall tile sprites, not a
    ///   real Tilemap — good enough to prove the generation/spawn logic works.
    public static class DungeonTestSceneBuilder
    {
        private const string ArtRoot = "Assets/Art";
        private const string TilesetFrames = ArtRoot + "/DungeonTileset/frames";
        private const string PrefabRoot = "Assets/Prefabs";
        private const string DataRoot = "Assets/Data";
        private const string SceneRoot = "Assets/Scenes";

        [MenuItem("Tools/Dungeon Loot Rush/Build Test Scene")]
        public static void BuildTestScene()
        {
            FixPixelArtImportSettings();

            var projectilePrefab = BuildProjectilePrefab();
            var goblinPrefab = BuildGoblinPrefab();
            var playerPrefab = BuildPlayerPrefab(projectilePrefab);
            var roomPrefab = BuildRoomPrefab(goblinPrefab);
            var roomTemplate = BuildRoomTemplateAsset(roomPrefab);

            BuildScene(playerPrefab, roomTemplate);

            Debug.Log("Dungeon Loot Rush: test scene built at Assets/Scenes/DungeonTest.unity. Open it and press Play.");
        }

        private static void FixPixelArtImportSettings()
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { TilesetFrames });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!(AssetImporter.GetAtPath(path) is TextureImporter importer))
                {
                    continue;
                }

                bool changed = false;
                if (importer.spritePixelsPerUnit != 16f)
                {
                    importer.spritePixelsPerUnit = 16f;
                    changed = true;
                }

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    changed = true;
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"Dungeon Loot Rush: missing sprite at {path}");
            }

            return sprite;
        }

        private static Sprite[] LoadFrames(params string[] fileNames)
        {
            return fileNames.Select(f => LoadSprite($"{TilesetFrames}/{f}")).ToArray();
        }

        private static int EnsureLayerExists(string layerName)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                {
                    return i;
                }
            }

            for (int i = 8; i < layers.arraySize; i++)
            {
                var element = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(element.stringValue))
                {
                    element.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    return i;
                }
            }

            Debug.LogError("Dungeon Loot Rush: no free layer slots for '" + layerName + "'.");
            return 0;
        }

        private static void SetObjectRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedProperties();
        }

        private static void SetObjectArray(Object target, string field, Object[] values)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            prop.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            so.ApplyModifiedProperties();
        }

        private static void SetFloat(Object target, string field, float value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).floatValue = value;
            so.ApplyModifiedProperties();
        }

        private static void SetLayerMask(Object target, string field, int layerIndex)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).intValue = 1 << layerIndex;
            so.ApplyModifiedProperties();
        }

        private static GameObject SaveAndDiscard(GameObject root, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BuildProjectilePrefab()
        {
            int enemyLayer = EnsureLayerExists("Enemy");

            var root = new GameObject("Projectile_Basic");
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite($"{TilesetFrames}/weapon_arrow.png");
            renderer.sortingOrder = 5;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var collider = root.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.2f;

            var projectile = root.AddComponent<Projectile>();
            SetLayerMask(projectile, "HitMask", enemyLayer);

            return SaveAndDiscard(root, $"{PrefabRoot}/Projectiles/Projectile_Basic.prefab");
        }

        private static GameObject BuildGoblinPrefab()
        {
            int enemyLayer = EnsureLayerExists("Enemy");

            var root = new GameObject("Goblin");
            root.layer = enemyLayer;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var collider = root.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.4f;

            root.AddComponent<Health>();
            root.AddComponent<EnemyController>();

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform);
            visual.transform.localPosition = Vector3.zero;

            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 1;

            var idleFrames = LoadFrames(
                "goblin_idle_anim_f0.png", "goblin_idle_anim_f1.png",
                "goblin_idle_anim_f2.png", "goblin_idle_anim_f3.png");
            var runFrames = LoadFrames(
                "goblin_run_anim_f0.png", "goblin_run_anim_f1.png",
                "goblin_run_anim_f2.png", "goblin_run_anim_f3.png");
            renderer.sprite = idleFrames.FirstOrDefault();

            var animator = visual.AddComponent<EnemyAnimator>();
            SetObjectArray(animator, "idleFrames", idleFrames);
            SetObjectArray(animator, "runFrames", runFrames);

            return SaveAndDiscard(root, $"{PrefabRoot}/Enemies/Goblin.prefab");
        }

        private static GameObject BuildPlayerPrefab(GameObject projectilePrefab)
        {
            int enemyLayer = EnsureLayerExists("Enemy");

            var root = new GameObject("Player") { tag = "Player" };

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            root.AddComponent<CircleCollider2D>().radius = 0.4f;

            root.AddComponent<Health>();
            root.AddComponent<PlayerRevive>();
            root.AddComponent<PlayerHealth>();
            root.AddComponent<SkillController>();
            var autoAttack = root.AddComponent<PlayerAutoAttack>();
            var controller = root.AddComponent<PlayerController>();

            var firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(root.transform);
            firePoint.transform.localPosition = Vector3.zero;

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.AddComponent<SpriteRenderer>().sortingOrder = 2;

            var directionalSprite = visual.AddComponent<PlayerDirectionalSprite>();
            SetObjectRef(directionalSprite, "down", LoadSprite($"{ArtRoot}/Character_Down.png"));
            SetObjectRef(directionalSprite, "up", LoadSprite($"{ArtRoot}/Character_Up.png"));
            SetObjectRef(directionalSprite, "left", LoadSprite($"{ArtRoot}/Character_Left.png"));
            SetObjectRef(directionalSprite, "right", LoadSprite($"{ArtRoot}/Character_Right.png"));
            SetObjectRef(directionalSprite, "northWest", LoadSprite($"{ArtRoot}/Character_NW.png"));
            SetObjectRef(directionalSprite, "northEast", LoadSprite($"{ArtRoot}/Character_NE.png"));
            SetObjectRef(directionalSprite, "southWest", LoadSprite($"{ArtRoot}/Character_SW.png"));
            SetObjectRef(directionalSprite, "southEast", LoadSprite($"{ArtRoot}/Character_SE.png"));

            SetObjectRef(autoAttack, "projectilePrefab", projectilePrefab.GetComponent<Projectile>());
            SetObjectRef(autoAttack, "firePoint", firePoint.transform);
            SetLayerMask(autoAttack, "enemyMask", enemyLayer);

            // joystick field is wired in BuildScene, once the UI exists.
            GC.KeepAlive(controller);

            return SaveAndDiscard(root, $"{PrefabRoot}/Player/Player.prefab");
        }

        private static GameObject BuildRoomPrefab(GameObject enemyPrefab)
        {
            const int width = 10;
            const int height = 8;

            var root = new GameObject("Room_Combat_01");

            var floorSprites = LoadFrames("floor_1.png", "floor_2.png", "floor_3.png", "floor_4.png");
            var wallMid = LoadSprite($"{TilesetFrames}/wall_mid.png");
            var wallTopMid = LoadSprite($"{TilesetFrames}/wall_top_mid.png");

            var floorParent = new GameObject("Floor");
            floorParent.transform.SetParent(root.transform);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new GameObject($"Floor_{x}_{y}");
                    tile.transform.SetParent(floorParent.transform);
                    tile.transform.localPosition = new Vector3(x, y, 0f);
                    var sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = floorSprites[(x + y) % floorSprites.Length];
                    sr.sortingOrder = -10;
                }
            }

            var wallParent = new GameObject("Walls");
            wallParent.transform.SetParent(root.transform);
            for (int x = -1; x <= width; x++)
            {
                AddWallTile(wallParent.transform, x, -1, wallTopMid);
                AddWallTile(wallParent.transform, x, height, wallTopMid);
            }

            for (int y = 0; y < height; y++)
            {
                AddWallTile(wallParent.transform, -1, y, wallMid);
                AddWallTile(wallParent.transform, width, y, wallMid);
            }

            var entryPoint = new GameObject("EntryPoint");
            entryPoint.transform.SetParent(root.transform);
            entryPoint.transform.localPosition = new Vector3(0.5f, height / 2f, 0f);

            var exitPoint = new GameObject("ExitPoint");
            exitPoint.transform.SetParent(root.transform);
            exitPoint.transform.localPosition = new Vector3(width - 0.5f, height / 2f, 0f);

            var spawnPointsParent = new GameObject("SpawnPoints");
            spawnPointsParent.transform.SetParent(root.transform);

            var spawnPositions = new[]
            {
                new Vector2(2, 2), new Vector2(2, height - 2),
                new Vector2(width / 2f, height / 2f),
                new Vector2(width - 2, 2), new Vector2(width - 2, height - 2),
                new Vector2(width / 2f, 2),
            };

            var spawnPoints = new EnemySpawnPoint[spawnPositions.Length];
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                var spawnGo = new GameObject($"SpawnPoint_{i}");
                spawnGo.transform.SetParent(spawnPointsParent.transform);
                spawnGo.transform.localPosition = spawnPositions[i];
                var spawnPoint = spawnGo.AddComponent<EnemySpawnPoint>();
                SetObjectArray(spawnPoint, "possibleEnemyPrefabs", new Object[] { enemyPrefab });
                spawnPoints[i] = spawnPoint;
            }

            var roomInstance = root.AddComponent<RoomInstance>();
            SetObjectRef(roomInstance, "entryPoint", entryPoint.transform);
            SetObjectRef(roomInstance, "exitPoint", exitPoint.transform);
            SetObjectArray(roomInstance, "candidateSpawnPoints", spawnPoints);

            return SaveAndDiscard(root, $"{PrefabRoot}/Rooms/Room_Combat_01.prefab");
        }

        private static void AddWallTile(Transform parent, float x, float y, Sprite sprite)
        {
            var tile = new GameObject($"Wall_{x}_{y}");
            tile.transform.SetParent(parent);
            tile.transform.localPosition = new Vector3(x, y, 0f);
            var sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            var collider = tile.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static RoomTemplateData BuildRoomTemplateAsset(GameObject roomPrefab)
        {
            Directory.CreateDirectory($"{DataRoot}/RoomTemplates");
            string path = $"{DataRoot}/RoomTemplates/RoomTemplate_Combat_01.asset";

            var template = AssetDatabase.LoadAssetAtPath<RoomTemplateData>(path);
            bool isNew = template == null;
            if (isNew)
            {
                template = ScriptableObject.CreateInstance<RoomTemplateData>();
            }

            template.TemplateId = "combat_01";
            template.Type = RoomType.Combat;
            template.RoomPrefab = roomPrefab;
            template.MinActiveSpawnPoints = 3;
            template.MaxActiveSpawnPoints = 5;

            if (isNew)
            {
                AssetDatabase.CreateAsset(template, path);
            }
            else
            {
                EditorUtility.SetDirty(template);
            }

            AssetDatabase.SaveAssets();
            return template;
        }

        private static void BuildScene(GameObject playerPrefab, RoomTemplateData roomTemplate)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("RunSession").AddComponent<RunSession>();
            new GameObject("AdManager").AddComponent<AdManager>();

            var dungeonRoot = new GameObject("DungeonRoot");

            var generator = new GameObject("DungeonGenerator").AddComponent<DungeonGenerator>();
            SetObjectArray(generator, "combatTemplates", new Object[] { roomTemplate });
            SetObjectRef(generator, "bossTemplate", roomTemplate);

            var stageManager = new GameObject("StageManager").AddComponent<StageManager>();
            SetObjectRef(stageManager, "generator", generator);
            SetObjectRef(stageManager, "dungeonRoot", dungeonRoot.transform);

            var playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInstance.transform.position = new Vector3(1f, 4f, 0f);
            var playerController = playerInstance.GetComponent<PlayerController>();

            var cameraGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            cameraGo.transform.position = new Vector3(1f, 4f, -10f);
            var follow = cameraGo.AddComponent<CameraFollow>();
            SetObjectRef(follow, "target", playerInstance.transform);

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var joystickBg = CreateUIImage("JoystickBackground", canvasGo.transform, new Vector2(150f, 150f));
            joystickBg.rectTransform.anchorMin = Vector2.zero;
            joystickBg.rectTransform.anchorMax = Vector2.zero;
            joystickBg.rectTransform.anchoredPosition = new Vector2(150f, 150f);
            joystickBg.color = new Color(1f, 1f, 1f, 0.3f);

            var joystickHandle = CreateUIImage("JoystickHandle", joystickBg.transform, new Vector2(70f, 70f));
            joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
            joystickHandle.color = new Color(1f, 1f, 1f, 0.7f);

            var joystick = joystickBg.gameObject.AddComponent<DungeonLootRush.UI.VirtualJoystick>();
            SetObjectRef(joystick, "background", joystickBg.rectTransform);
            SetObjectRef(joystick, "handle", joystickHandle.rectTransform);
            SetFloat(joystick, "handleRange", 60f);

            SetObjectRef(playerController, "joystick", joystick);

            Directory.CreateDirectory(SceneRoot);
            EditorSceneManager.SaveScene(scene, $"{SceneRoot}/DungeonTest.unity");
        }

        private static Image CreateUIImage(string name, Transform parent, Vector2 size)
        {
            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            image.rectTransform.sizeDelta = size;
            return image;
        }
    }
}
