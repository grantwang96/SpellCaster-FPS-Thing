using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVision : MonoBehaviour {

    private List<CharacterBehaviour> _knownCharacters = new List<CharacterBehaviour>();
    public IReadOnlyList<CharacterBehaviour> KnownCharacters => _knownCharacters;

    public CharacterBehaviour CurrentTarget { get; private set; }

    private NPCBehaviour _npcBehaviour;
    private NPCDamageable _npcDamageable;

    private void Awake() {
        _npcBehaviour = GetComponent<NPCBehaviour>();
        _npcDamageable = GetComponent<NPCDamageable>();
    }

    public virtual CharacterBehaviour CheckVision() {
        for (int i = 0; i < KnownCharacters.Count; i++) {
            CharacterBehaviour knownCharacter = KnownCharacters[i];
            if (CheckVision(knownCharacter)) {
                return knownCharacter;
            }
        }
        return null;
    }

    public virtual bool CheckVision(CharacterBehaviour target) {
        if (target == null) {
            return false;
        }
        float distance = Vector3.Distance(target.transform.position, transform.position);
        float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
        if (distance <= _npcBehaviour.Blueprint.VisionRange && angle <= _npcBehaviour.Blueprint.VisionAngle) {
            Vector3 dir = target.Head.position - _npcBehaviour.Head.position;
            RaycastHit hit;
            if (Physics.Raycast(_npcBehaviour.Head.position, dir, out hit, _npcBehaviour.Blueprint.VisionRange, _npcBehaviour.Blueprint.VisionMask)) {
                CharacterBehaviour otherCB = hit.transform.GetComponent<CharacterBehaviour>();
                if (otherCB != null && _knownCharacters.Contains(otherCB) && _npcBehaviour.IsAnEnemy(otherCB)) {
                    return true;
                }
            }
        }
        return false;
    }

    public virtual bool CheckVisionRadial(CharacterBehaviour target) {
        if(target == null) {
            return false;
        }
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance <= _npcBehaviour.Blueprint.VisionRange) {
            Vector3 dir = target.Head.position - _npcBehaviour.Head.position;
            RaycastHit hit;
            if (Physics.Raycast(_npcBehaviour.Head.position, dir, out hit, _npcBehaviour.Blueprint.VisionRange, _npcBehaviour.Blueprint.VisionMask)) {
                CharacterBehaviour otherCB = hit.transform.GetComponent<CharacterBehaviour>();
                if (otherCB != null && _knownCharacters.Contains(otherCB) && _npcBehaviour.IsAnEnemy(otherCB)) {
                    return true;
                }
            }
        }
        return false;
    }

    public virtual bool CanSeeTarget(Vector3 target) {
        float distance = Vector3.Distance(target, _npcBehaviour.Head.position);
        Vector3 targetDir = target - _npcBehaviour.Head.position;
        Vector3 headForward = _npcBehaviour.Head.forward;
        headForward.y = 0f;
        if (distance < _npcBehaviour.Blueprint.VisionRange) {
            RaycastHit hit;
            if (Physics.Raycast(_npcBehaviour.Head.position, targetDir, out hit, _npcBehaviour.Blueprint.VisionRange, _npcBehaviour.Blueprint.VisionMask)) {
                if (hit.transform == CurrentTarget.transform) { return true; }
            }
        }
        return false;
    }

    public void SetCurrentTarget(CharacterBehaviour character) {
        if(CurrentTarget != null) {
            ClearCurrentTarget();
        }
        CurrentTarget = character;
        CurrentTarget.Damageable.OnDeath += OnTargetDefeated;
    }

    public virtual void ClearCurrentTarget() {
        CurrentTarget.Damageable.OnDeath -= OnTargetDefeated;
        CurrentTarget = null;
    }

    public virtual void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (!_knownCharacters.Contains(characterBehaviour)) {
            _knownCharacters.Add(characterBehaviour);
        }
    }

    public virtual void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (_knownCharacters.Contains(characterBehaviour)) {
            _knownCharacters.Remove(characterBehaviour);
        }
    }

    private void OnTakeDamage(CharacterBehaviour attacker, int damage, Element element, Vector3 velocity, StatusEffect statusEffect) {
        if(CurrentTarget != null) {
            return;
        }
        if (CanSeeTarget(attacker.GetBodyPosition())) {
            if (!_knownCharacters.Contains(attacker)) {
                _knownCharacters.Add(attacker);
            }
            CurrentTarget = attacker;
        }
    }

    private void OnTargetDefeated(bool dead, Damageable damageable) {
        damageable.OnDeath -= OnTargetDefeated;
        ClearCurrentTarget();
    }
}
