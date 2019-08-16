
public static partial class GameplayValues {
    // ultra broad or weird stuff here
}

public static partial class GameplayValues {

    public static class Inventory {
        public const string UnknownInventoryItemId = "UNKNOWN_ITEM_ID";
    }

    public static class UI {
        public const string EmptyInventoryItemId = "NONE";
        public const int MaximumSpellCastingMethods = 1;
        public const int MaximumSpellEffects = 5;
        public const int MaximumSpellModifiers = 5;
        public const string EmptySpellStageText = "-----";
        public const string EmptyUIElementId = "EMPTY_UI_ELEMENT";
        public const string GenericButtonIdClose = "GENERIC_BUTTON_CLOSE";
        public const string GenericButtonIdYes = "GENERIC_BUTTON_YES";
        public const string GenericButtonIdNo = "GENERIC_BUTTON_NO";
    }

    public static class UnitTags {
        public const string PlayerUnitTag = "PLAYER_UNIT";
        public const string PlayerFriendlyUnitTag = "PLAYER_FRIENDLY_UNIT";
        public const string PlayerEnemyUnitTag = "PLAYER_ENEMY_UNIT";
    }

    public static class Combat {
        public const string NPCNormalAttackTriggerId = "Attack";
        public const string NPCAttackComboIndexId = "AttackComboIndex";
    }

    // pooled objects that will only ever have one version
    public static class ObjectPooling {
        public const string RecoveryOrbPrefabId = "RecoveryOrb";
        public const string WorldRunePrefabId = "RecoveryRune";
    }

    public static class Level {
        public const int HealthOrbValue = 3;
        public const int ManaOrbValue = 100;

        public const string RuneInstanceIdPrefix = "SpellComponentRune_";
        public const int RuneInstanceIdSize = 20;

        public const string SpellCraftStationInstanceIdPrefix = "SpellCraftStation_";
        public const int SpellCraftStationInstanceIdSize = 20;

        public const string LoadoutStationInstanceIdPrefix = "LoadoutStation_";
        public const int LoadoutStationInstanceIdSize = 20;
    }

    public static class Magic {
        public const string StorableSpellInstanceIdPrefix = "StoredSpell_";
        public const int StorableSpellInstanceIdSize = 20;
        public const int PlayerLoadoutMaxSize = 3;
    }
}
