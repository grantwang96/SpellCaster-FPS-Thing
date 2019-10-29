using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    [SerializeField] private Key _key;
    [SerializeField] private bool _canOpen;
    public bool CanOpen => _canOpen;
    private bool _isOpen;

    [SerializeField] private Collider _doorCollider;
    [SerializeField] private MeshRenderer _meshRenderer;

    private static Color OpenColor = new Color(0f, 1f, 0f, 0.5f);
    private static Color ClosedColor = new Color(1f, 0f, 0f, 0.5f);

    private List<CharacterBehaviour> _npcsWithinRange = new List<CharacterBehaviour>();

    private void Start() {
        Close();
        if (!_canOpen) {
            Lock();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!_canOpen) {
            return;
        }
        NPCBehaviour npc = other.GetComponent<NPCBehaviour>();
        if(npc == null) {
            return;
        }
        TryOpen(npc);
    }

    private void OnTriggerExit(Collider other) {
        if (!_canOpen) {
            return;
        }
        NPCBehaviour npc = other.GetComponent<NPCBehaviour>();
        if (npc == null) {
            return;
        }
        TryClose(npc);
    }

    private void TryOpen(NPCBehaviour npc) {
        if (_npcsWithinRange.Contains(npc)) {
            Debug.Log("We've somehow added this NPC twice. What gives?");
            return;
        }
        _npcsWithinRange.Add(npc);
        Open();
    }

    private void Open() {
        // temp code
        _isOpen = true;
        _doorCollider.enabled = false;
        _meshRenderer.material.color = OpenColor;
    }

    private void TryClose(NPCBehaviour npc) {
        _npcsWithinRange.Remove(npc);
        if(_npcsWithinRange.Count == 0) {
            Close();
        }
    }

    private void Close() {
        _isOpen = false;
        _doorCollider.enabled = true;
        _meshRenderer.material.color = ClosedColor;
    }

    public void Lock() {
        if(_key == null) {
            return;
        }
        _key.OnInteractSuccess += Unlock;
    }

    private void Unlock() {
        _canOpen = true;
        _key.OnInteractSuccess -= Unlock;
    }

    public void PlayerInteracted() {
        if (_canOpen && !_isOpen) {
            Open();
        } else {
            Close();
        }
    }
}