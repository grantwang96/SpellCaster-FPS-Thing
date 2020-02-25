using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Summon")]
public class Effect_Summon : Effect {

    [SerializeField] private string _unitPrefabId;
    [SerializeField] private int _poolBaseCount;

    public override void TriggerEffect(Damageable caster, float powerScale, List<Effect> additionalEffects = null) {
        CharacterBehaviour behaviour = caster.Root.GetComponent<CharacterBehaviour>();
        TrySpawnUnit(caster.Body.position, caster.Body.eulerAngles, behaviour.UnitTags);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        CharacterBehaviour behaviour = caster.Root.GetComponent<CharacterBehaviour>();
        TrySpawnUnit(position, caster.Body.eulerAngles, behaviour.UnitTags);
    }

    public override void TriggerEffect(Damageable caster, Vector3 velocity, float powerScale, Vector3 position, Damageable damageable = null, List<Effect> additionalEffects = null) {
        CharacterBehaviour behaviour = caster.Root.GetComponent<CharacterBehaviour>();
        TrySpawnUnit(position, caster.Body.eulerAngles, behaviour.UnitTags);
    }

    public override void TriggerEffect(Damageable caster, float powerScale, Vector3 position, Collider collider, List<Effect> additionalEffects = null) {
        CharacterBehaviour behaviour = caster.Root.GetComponent<CharacterBehaviour>();
        TrySpawnUnit(position, caster.Body.eulerAngles, behaviour.UnitTags);
    }

    private void TrySpawnUnit(Vector3 position, Vector3 rotation, List<string> overrideTags) {
        EnemyBehaviour newUnit = NPCManager.Instance?.SpawnPooledNPC(_unitPrefabId, position, rotation);
        newUnit.OverrideUnitTags(overrideTags);
    }
}
