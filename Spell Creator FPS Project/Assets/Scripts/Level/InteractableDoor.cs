using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IRaycastInteractable {

    public bool Interactable { get; private set; } = true;
    public Vector3 InteractableCenter => transform.position;

    [SerializeField] private string _interactableId;
    public string InteractableId => _interactableId;
    [SerializeField] private Door _parentDoor;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _interactMessage;

    public event InteractEvent OnInteractAttempt;
    public event InteractEvent OnInteractSuccess;

    private void Awake() {
        Undetect();
    }

    public void InteractPress(CharacterBehaviour character) {
        if (!Interactable) {
            return;
        }
        if (character == PlayerController.Instance) {
            _parentDoor.PlayerInteracted();
        }
    }

    public void InteractHold(CharacterBehaviour character) {

    }

    public void Detect(CharacterBehaviour character) {
        // _animator.SetBool("PlayerDetect", true);
        _interactMessage.gameObject.SetActive(true);
    }

    public void Undetect() {
        _interactMessage.gameObject.SetActive(false);
    }
}
