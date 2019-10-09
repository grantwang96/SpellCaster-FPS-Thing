using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaStart : MonoBehaviour, IInteractable {

    [SerializeField] private string _interactableId;
    public string InteractableId => _interactableId;

    [SerializeField] private bool _interactable;
    public bool Interactable => _interactable;

    public Vector3 InteractableCenter => transform.position;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    private GameObject _parentObject;

    public void Detect() {
        
    }

    public void InteractHold(CharacterBehaviour character) {
        
    }

    public void InteractPress(CharacterBehaviour character) {
        if(character == PlayerController.Instance && Interactable) {
            ArenaManager.Instance.StartRound();
        }
    }
    
    private void Start() {
        _parentObject = transform.parent.gameObject;
    }
}
