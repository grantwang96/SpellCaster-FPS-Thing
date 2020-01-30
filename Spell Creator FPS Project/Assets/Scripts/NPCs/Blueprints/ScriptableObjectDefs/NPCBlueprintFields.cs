using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class NPCBlueprint : ScriptableObject {

    [SerializeField] protected string _npcIdPrefix;
    public string NpcIdPrefix => _npcIdPrefix;

    [SerializeField] private int _scoreValue;
    public int ScoreValue => _scoreValue;

    [SerializeField] protected int _totalHealth;
    public int TotalHealth { get { return _totalHealth; } }
    [SerializeField] protected StatusEffect[] _statResistances;
    public StatusEffect[] StatResistances => _statResistances;
    [SerializeField] protected StatusEffect[] _statWeaknesses;
    public StatusEffect[] StatWeaknesses => _statWeaknesses;

    [SerializeField] protected float _walkSpeed;
    public float WalkSpeed { get { return _walkSpeed; } }
    [SerializeField] protected float _runSpeed;
    public float RunSpeed { get { return _runSpeed; } }

    [SerializeField] private float _idleTimeMinimum;
    [SerializeField] private float _idleTimeMaximum;
    public float GetNewIdleTime { get { return Random.Range(_idleTimeMinimum, _idleTimeMaximum); } }

    [SerializeField] protected float _visionRange;
    public float VisionRange { get { return _visionRange; } }
    [SerializeField] protected float _visionAngle;
    public float VisionAngle { get { return _visionAngle; } }
    [SerializeField] protected LayerMask _visionMask;
    public LayerMask VisionMask { get { return _visionMask; } }

    // what type of enemy this is
    [SerializeField] private string[] _unitTags;
    public string[] UnitTags => _unitTags;

    // what type of enemies to be aggressive against
    [SerializeField] private string[] _enemyTags;
    public string[] EnemyTags => _enemyTags;

    // loot drop data
    [SerializeField] private MinMax_Int _healthOrbRewards;
    public MinMax_Int HealthOrbRewards => _healthOrbRewards;
    [SerializeField] private MinMax_Int _manaOrbRewards;
    public MinMax_Int ManaOrbRewards => _manaOrbRewards;

    // range of the total of how many reward items can drop
    [SerializeField] private MinMax_Int _rewardDropRange;
    public MinMax_Int RewardDropRange => _rewardDropRange;

    // loot information for each tier
    [SerializeField] private List<LootInfo> _lootTable;
    public IReadOnlyList<LootInfo> LootTable => _lootTable;
}
