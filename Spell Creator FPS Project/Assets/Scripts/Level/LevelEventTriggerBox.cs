using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEventTriggerBox : MonoBehaviour
{
    [SerializeField] private UnityAction _action;
    [SerializeField] private bool _repeatable;
    [SerializeField] private Collider _collider;

    private void OnTriggerEnter(Collider other) {
        CharacterBehaviour character = other.GetComponent<CharacterBehaviour>();
        if(character != null && character == PlayerController.Instance) {
            _action?.Invoke();
            _collider.enabled = _repeatable;
        }
    }
}
