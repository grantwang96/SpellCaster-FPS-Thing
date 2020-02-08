using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVision : MonoBehaviour, IVision {

    public List<CharacterBehaviour> KnownCharacters { get; } = new List<CharacterBehaviour>();
    public List<CharacterBehaviour> EnemyCharacters { get; } = new List<CharacterBehaviour>(); // REPLACE THIS WITH TAG SYSTEM
    public CharacterBehaviour CurrentTarget { get; private set; }

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
                if (otherCB != null && KnownCharacters.Contains(otherCB)) {
                    CurrentTarget = otherCB;
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
        CurrentTarget = null;
    }

    public virtual void RegisterToKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (!KnownCharacters.Contains(characterBehaviour)) {
            KnownCharacters.Add(characterBehaviour);
        }
    }

    public virtual void DeregisterFromKnownCharacters(CharacterBehaviour characterBehaviour) {
        if (KnownCharacters.Contains(characterBehaviour)) {
            KnownCharacters.Remove(characterBehaviour);
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
