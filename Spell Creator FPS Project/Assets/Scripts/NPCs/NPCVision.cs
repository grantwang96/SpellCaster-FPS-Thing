using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVision : MonoBehaviour, IVision{

    private List<CharacterBehaviour> _knownCharacters = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> KnownCharacters => _knownCharacters;
    private List<CharacterBehaviour> _enemyCharacters = new List<CharacterBehaviour>(); // WE SHOULD PHASE THIS OUT AND USE BLUEPRINT TO CHECK IF ENEMY TARGET(tag system?)
    public List<CharacterBehaviour> EnemyCharacters => _enemyCharacters;
    private CharacterBehaviour _currentTarget;
    public CharacterBehaviour CurrentTarget => _currentTarget;

    private NPCBehaviour _npcBehaviour;

    private void Awake() {
        _npcBehaviour = GetComponent<NPCBehaviour>();
    }

    public virtual bool CheckVision() {
        for (int i = 0; i < KnownCharacters.Count; i++) {
            CharacterBehaviour knownCharacter = KnownCharacters[i];
            if (CheckVision(knownCharacter)) {
                return true;
            }
        }
        return false;
    }

    public virtual bool CheckVision(CharacterBehaviour target) {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
        if (distance <= _npcBehaviour.Blueprint.VisionRange && angle <= _npcBehaviour.Blueprint.VisionAngle) {
            Vector3 dir = target.Head.position - _npcBehaviour.Head.position;
            RaycastHit hit;
            if (Physics.Raycast(_npcBehaviour.Head.position, dir, out hit, _npcBehaviour.Blueprint.VisionRange, _npcBehaviour.Blueprint.VisionMask)) {
                CharacterBehaviour otherCB = hit.transform.GetComponent<CharacterBehaviour>();
                if (otherCB != null && _knownCharacters.Contains(otherCB) && _npcBehaviour.Blueprint.IsEnemy(otherCB)) {
                    _currentTarget = otherCB;
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

    public virtual void ClearCurrentTarget() {
        _currentTarget = null;
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
}

/// <summary>
/// Interface that allows NPCs to check for vision
/// </summary>
public interface IVision {
    // WIP: these should return values
    CharacterBehaviour CurrentTarget { get; }

    bool CheckVision(); // checks general vision and returns first custom object it sees
    bool CheckVision(CharacterBehaviour target);
    bool CanSeeTarget(Vector3 target); // checks to see if this target is viewable(if it has one)
    void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour);
    void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour);
}
