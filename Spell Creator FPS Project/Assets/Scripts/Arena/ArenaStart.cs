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
        if(character == GameplayController.Instance && Interactable) {
            _interactable = false;
            ArenaManager.Instance.StartRound();
        }
    }

    private void OnRoundStart(int round) {
        _parentObject.SetActive(false);
    }

    private void OnRoundEnd(int round) {
        _interactable = true;
        _parentObject.SetActive(true);
    }
    
    private void Start() {
        _parentObject = transform.parent.gameObject;
        ArenaManager.Instance.OnRoundStarted += OnRoundStart;
        ArenaManager.Instance.OnRoundEnded += OnRoundEnd;
    }
}
