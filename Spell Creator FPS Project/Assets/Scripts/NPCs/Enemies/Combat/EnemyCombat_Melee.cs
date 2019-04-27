using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat_Melee : NPCCombat {

    protected Coroutine _attackRoutine;
    protected MeleeAttackData _currentAttack;
    public override string CurrentAttackName => _currentAttack?.AttackName ?? "";

    protected List<MeleeAttackData> _comboMovesDatas = new List<MeleeAttackData>();
    protected List<MeleeAttackData> _specialMovesDatas = new List<MeleeAttackData>();
    [SerializeField] protected List<MeleeAttackData> _attackDatas = new List<MeleeAttackData>();

    private void Awake() {
        InitializeMovesData();
    }
    
    private void InitializeMovesData() {
        for(int i = 0; i < _attackDatas.Count; i++) {
            if (!_attackDatas[i].ComboContinueAttack) {
                _specialMovesDatas.Add(_attackDatas[i]);
            } else {
                _comboMovesDatas.Add(_attackDatas[i]);
            }
        }
    }

    public override bool Attack(string attackName) {
        if(_attackRoutine != null) { StopCoroutine(_attackRoutine); }
        int attackDataIndex = GetAttackDataIndex(attackName);
        if (attackDataIndex == -1) {
            Debug.LogError($"Invalid attack name {attackName} was given for NPC {name}");
            return false;
        }
        _currentAttack = _attackDatas[attackDataIndex];
        _attackRoutine = StartCoroutine(ProcessAttack());
        return true;
    }

    protected override IEnumerator ProcessAttack() {
        string attackName = "";
        if (!_currentAttack.ComboContinueAttack) {
            attackName = _currentAttack.AttackName;
        }
        while (_animController.IsStateByName(_currentAttack.AttackName)) {
            yield return new WaitForEndOfFrame();
        }
        StartAttack(attackName);
        while (!_animController.IsStateByName(_currentAttack.AttackName)) {
            yield return new WaitForEndOfFrame();
        }
        _currentAttack.HitBoxesActive = false;
        while (CurrentAnimationTime < _currentAttack.ActivateHitBoxTime) {
            yield return new WaitForEndOfFrame();
        }
        _currentAttack.HitBoxesActive = true;
        while(CurrentAnimationTime < _currentAttack.DeactivateHitBoxTime) {
            yield return new WaitForEndOfFrame();
        }
        _currentAttack.HitBoxesActive = false;
        EndAttack();
        while (CurrentAnimationTime < 1f) {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("End of attack!");
        _attackRoutine = null;
    }

    public override string NextAttack() {
        SelectAttack();
        string nextAttack = _currentAttack?.AttackName ?? "";
        Debug.Log("Next Attack: " + _currentAttack.AttackName);
        return nextAttack;
    }

    private void SelectAttack() {
        // select attack based on hp tiers
        AttackPhase attackPhase = null;
        for (int i = 1; i <= AttackPhases.Length; i++) {
            if(_damageable.Health <= _damageable.MaxHealth * i / AttackPhases.Length) {
                attackPhase = AttackPhases[i - 1];
                break;
            }
        }
        if(attackPhase != null) {
            if(Random.value <= attackPhase.NormalAttack) {
                if (_currentAttack == null) {
                    _currentAttack = _comboMovesDatas[0];
                    return;
                }
                int index = _comboMovesDatas.IndexOf(_currentAttack);
                Debug.Log("Current Attack Index: " + index);
                _currentAttack = (index + 1) < _comboMovesDatas.Count ? _comboMovesDatas[index + 1] : _comboMovesDatas[0];
                Debug.Log("Next Attack Index: " + (index + 1));
                return;
            }
            _currentAttack = _specialMovesDatas[Random.Range(0, _specialMovesDatas.Count)];
        }
    }

    private int GetAttackDataIndex(string attackName) {
        int index = -1;
        for(int i = 0; i < _attackDatas.Count; i++) {
            if (_attackDatas[i].AttackName.Equals(attackName)) {
                index = i;
                break;
            }
        }
        return index;
    }

    private void SetHitBoxesActive(Collider[] hitBoxes, bool active) {
        foreach (Collider hitBox in hitBoxes) {
            hitBox.gameObject.SetActive(active);
        }
    }
}

[System.Serializable]
public class MeleeAttackData : IAttackData{
    [SerializeField] private string _attackName;
    [SerializeField] private Collider[] _hitBoxes;
    [Range(0f, 1f)] [SerializeField] private float _activateHitBoxTime;
    [Range(0f, 1f)] [SerializeField] private float _deactivateHitBoxTime;
    [SerializeField] private bool _comboContinueAttack;

    public string AttackName => _attackName;
    public float ActivateHitBoxTime => _activateHitBoxTime;
    public float DeactivateHitBoxTime => _deactivateHitBoxTime;
    public bool ComboContinueAttack => _comboContinueAttack;

    public bool HitBoxesActive {
        get { return _hitBoxes != null && _hitBoxes[0].enabled; }
        set {
            foreach(Collider coll in _hitBoxes) {
                coll.gameObject.SetActive(value);
            }
        }
    }
}

