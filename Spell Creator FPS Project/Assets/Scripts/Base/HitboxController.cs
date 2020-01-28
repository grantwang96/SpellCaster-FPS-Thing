using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private List<Hitbox> _hitBoxGOs = new List<Hitbox>();
    [SerializeField] private CharacterBehaviour _characterBehaviour;

    private Dictionary<string, Hitbox> _hitBoxes = new Dictionary<string, Hitbox>();
    public IReadOnlyDictionary<string, Hitbox> HitBoxes => _hitBoxes;

    private void Awake() {
        InitializeHitBoxes();
    }

    private void InitializeHitBoxes() {
        _hitBoxes.Clear();
        for (int i = 0; i < _hitBoxGOs.Count; i++) {
            _hitBoxes.Add(_hitBoxGOs[i].name, _hitBoxGOs[i]);
        }
    }
}
