
public static partial class GameplayValues {
    // ultra broad or weird stuff here
}

public static partial class GameplayValues {

    public static class Inventory {
        public const string UnknownInventoryItemId = "UNKNOWN_ITEM_ID";
    }

    public static class UI {
        public const int DefaultSpellName_MaxCastingMethods = 1;
        public const int DefaultSpellName_MaxEffects = 2;
        public const int DefaultSpellName_MaxModifiers = 2;

        public const string EmptyInventoryItemId = "NONE";
        public const string EmptySpellStageText = "-----";
        public const string EmptyUIElementId = "EMPTY_UI_ELEMENT";
        public const string GenericButtonIdClose = "GENERIC_BUTTON_CLOSE";
        public const string GenericButtonIdYes = "GENERIC_BUTTON_YES";
        public const string GenericButtonIdNo = "GENERIC_BUTTON_NO";

        public const string PickedUpItemMessageBase = "Picked up {0}!";
    }

    public static class BrainStates {
        public const string IdleStateId = "IdleBrainState";
        public const string WalkStateId = "WalkBrainState";
        public const string ChaseStateId = "ChaseBrainState";
        public const string ScanStateId = "ScanBrainState";
        public const string RunStateId = "RunBrainState";
        public const string TakeCoverStateId = "TakeCoverBrainState";
        public const string AlertStateId = "AlertBrainStateId";
        public const string MeleeAttackStateId = "MeleeAttackBrainState";
        public const string RangeAttackStateId = "RangeAttackBrainState";
    }

    public static class UnitTags {
        public const string PlayerUnitTag = "PLAYER_UNIT";
        public const string PlayerFriendlyUnitTag = "PLAYER_FRIENDLY_UNIT";
        public const string PlayerEnemyUnitTag = "PLAYER_ENEMY_UNIT";
    }

    public static class Combat {
        public const int NPCIdSize = 10;
        public const string NPCNormalAttackTriggerId = "Attack";
        public const string NPCAttackComboIndexId = "AttackComboIndex";
    }

    // pooled objects that will only ever have one version
    public static class ObjectPooling {
        public const string RecoveryOrbPrefabId = "prefab.RecoveryOrb";
        public const string WorldRunePrefabId = "prefab.Interactable_Rune";
    }

    public static class Loot {
        public const string HealthOrbId = "HealthOrb";
        public const string ManaOrbId = "ManaOrb";
    }

    public static class Level {
        public const int HealthOrbValue = 3;
        public const int ManaOrbValue = 25;

        public const string RuneInstanceIdPrefix = "SpellComponentRune_";
        public const int RuneInstanceIdSize = 20;

        public const string SpellCraftStationInstanceIdPrefix = "SpellCraftStation_";
        public const int SpellCraftStationInstanceIdSize = 20;

        public const string LoadoutStationInstanceIdPrefix = "LoadoutStation_";
        public const int LoadoutStationInstanceIdSize = 20;
    }

    public static class Magic {
        public const int MaximumSpellCastingMethods = 1;
        public const int MaximumSpellEffects = 5;
        public const int MaximumSpellModifiers = 5;
        public const string StorableSpellInstanceIdPrefix = "StoredSpell_";
        public const int StorableSpellInstanceIdSize = 20;
        public const int PlayerLoadoutMaxSize = 3;
    }

    public static class Navigation {
        public const string EnterTutorialLevelTransitionId = "enter_tutoriallevel";
        public const string EnterArenaTransitionId = "enter_arena";
        public const string ReenterArenaTransitionId = "reenter_arena";
    }

    public static class Tutorial {
        public const string TutorialLevelCompletedId = "TUTORIAL_LEVEL_COMPLETED";
    }
}
