using UnityEngine;

namespace DungeonLootRush.Player
{
    /// Picks the matching static sprite for the player's current 8-way facing
    /// direction and fakes a walk cycle with a simple bob/sway instead of a
    /// per-frame animation, since the robe design hides the legs entirely.
    ///
    /// Must live on a child GameObject of the object carrying PlayerController
    /// and its Rigidbody2D — the bob/sway drives this transform's local
    /// position/rotation, which would fight the physics body if applied to the
    /// same GameObject the Rigidbody2D moves.
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerDirectionalSprite : MonoBehaviour
    {
        [SerializeField] private Sprite down;
        [SerializeField] private Sprite up;
        [SerializeField] private Sprite left;
        [SerializeField] private Sprite right;
        [SerializeField] private Sprite northWest;
        [SerializeField] private Sprite northEast;
        [SerializeField] private Sprite southWest;
        [SerializeField] private Sprite southEast;

        [Header("Walk bob/sway")]
        [SerializeField] private float bobHeight = 0.05f;
        [SerializeField] private float bobSpeed = 8f;
        [SerializeField] private float swayAngle = 4f;

        private PlayerController _controller;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _basePosition;
        private float _bobPhase;

        private void Awake()
        {
            _controller = GetComponentInParent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _basePosition = transform.localPosition;
        }

        private void Update()
        {
            _spriteRenderer.sprite = PickSprite(_controller.FacingDirection);

            if (_controller.IsMoving)
            {
                _bobPhase += Time.deltaTime * bobSpeed;
            }
            else
            {
                _bobPhase = 0f;
            }

            float bob = Mathf.Abs(Mathf.Sin(_bobPhase)) * bobHeight;
            float sway = Mathf.Sin(_bobPhase) * swayAngle;

            transform.localPosition = _basePosition + new Vector3(0f, bob, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, sway);
        }

        private Sprite PickSprite(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f)
            {
                return down;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            int sector = Mathf.RoundToInt(angle / 45f) & 7;

            switch (sector)
            {
                case 0: return right;
                case 1: return northEast;
                case 2: return up;
                case 3: return northWest;
                case 4: return left;
                case 5: return southWest;
                case 6: return down;
                default: return southEast;
            }
        }
    }
}
