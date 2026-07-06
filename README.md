# Dungeon Loot Rush

Rogue-like 地牢探索 + 裝備養成手遊。完整設計文件見 [`docs/GDD.md`](docs/GDD.md)。

這是依 GDD 第 11 節 MVP 範圍建立的 Unity 專案骨架:資料夾結構 + 核心系統的 C# 腳本,尚未包含場景、Prefab 與美術資源(需在 Unity Editor 中手動組裝)。

## 如何開啟

1. 用 Unity Hub 開啟本專案根目錄(需要 Unity **2022.3 LTS**,見 `ProjectSettings/ProjectVersion.txt`)。
2. 首次開啟時 Unity 會自動產生 `Library/`、`.meta` 檔與其餘 `ProjectSettings/*.asset`,屬正常現象。
3. `Packages/manifest.json` 已包含 2D Sprite、TextMeshPro、UGUI、Physics2D 等 MVP 所需模組。

## 專案結構

```
Assets/
  Scripts/
    Core/      GameManager(場景流程)、PlayerProfile(存讀檔)、AdManager(廣告 SDK 樁)
    Player/    PlayerController、AutoAttack、Revive、SkillController + Skills/
    Enemy/     EnemyController(追擊+接觸傷害)、EnemyRangedAttack、EnemySpawnPoint
    Combat/    Health、IDamageable、Projectile(單發/扇形/彈射/貫穿/追蹤)
    Dungeon/   RoomTemplateData、RoomInstance、DungeonGenerator、StageManager
    Items/     EquipmentData/Instance/Database、EquipmentUpgradeSystem、Chest 系列
    Economy/   RunSession(單局結算)、RunResultSettlement
    UI/        VirtualJoystick、SkillButtonUI、HealthBarUI、ResultScreenUI、ChestTimerUI
  Prefabs/     Rooms / Enemies / Player / Projectiles / UI(待建立)
  Data/        Equipment / RoomTemplates / Skills 的 ScriptableObject 資產(待建立)
  Scenes/      Boot / Town / Dungeon(待建立)
  Art/ Audio/  色塊美術與音效資源(待建立)
```

## 架構對應 GDD 的關鍵決策

- **房間生成**(GDD 3.3):`RoomTemplateData` 是模板庫的資料容器,`DungeonGenerator` 只負責挑模板、決定商人/寶箱房出現機率,`RoomInstance` 從 5-8 個候選點隨機挑 3-5 個生怪 —— 對應「模板庫 + 內容隨機化」而非完全隨機生成。
- **彈道詞綴**(GDD 6.2):`ProjectileBehavior` enum(Single/Fan/Bounce/Pierce/Homing)同時是武器詞綴與 `Projectile` 的飛行邏輯,避免兩套定義。扇形在發射端展開多顆彈道,彈射/貫穿在 `Projectile` 內處理碰撞後行為。
- **裝備強化**(GDD 6.1):`EquipmentUpgradeSystem.GetUpgradeCost` 實作三段式曲線(1-10 純金幣、11-30 金幣+強化石、31+ 突破材料+失敗機率)。
- **結算制**(GDD 5):`RunSession` 只存在於單局跑圖期間,死亡耗盡復活次數或放棄時,`RunResultSettlement` 才把戰利品寫入 `PlayerProfile` 並存檔。
- **廣告觸點**(GDD 8):只有 `AdManager.ShowRewardedAd` 一個接縫,`PlayerRevive`(死亡復活)與 `TownChest.SkipWaitWithAd`(寶箱加速)呼叫它。目前是永遠成功的樁實作,之後串 LevelPlay/AppLovin MAX 時只需改這一個檔案。

## 已套用的預設值(對應 GDD 第 12 節待確認事項)

目前先採用 GDD 文中給出的範例值,之後確認後可直接調整常數:

- 主城寶箱等待時間:銅 5 分鐘 / 銀 30 分鐘 / 金 2 小時 / 傳說 6 小時(`TownChest.GetWaitDuration`)。
- 技能鍵:`SkillController` 用 list 設計,同時支援 1 個或 2 個技能欄位,已附一個範例技能 `AoeBurstSkill`。
- 商人房:`DungeonGenerator` 已含商人房機率欄位(預設 25%),第一版若決定不做,把 `merchantTemplates` 留空即可自動跳過。

## 尚未包含,需要在 Editor 中接續完成

- 建立 `Boot` / `Town` / `Dungeon` 三個場景,並把 `GameManager`、`RunSession`、`AdManager`、`DungeonGenerator`、`StageManager` 掛到對應場景物件上。
- 用色塊 Sprite 建立 Player / Enemy / Projectile / Chest Prefab,掛上對應腳本並串好 `SerializeField` 參照。
- 建立 5-8 組房間 Prefab(對應 `RoomTemplateData`),每間房設定 Entry/Exit Point 與 5-8 個 `EnemySpawnPoint`。
- 建立 `EquipmentData`、`EquipmentDatabase`、`AoeBurstSkill` 的實際資產(Create > DungeonLootRush > ...)。
- 串接虛擬搖桿與技能鍵的 UI Canvas。
- 若要正式上線,把 `AdManager` 換成真正的 LevelPlay/AppLovin MAX SDK 呼叫。
