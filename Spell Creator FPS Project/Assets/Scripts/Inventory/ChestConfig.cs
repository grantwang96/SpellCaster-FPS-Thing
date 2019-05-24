using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestConfig")]
public class ChestConfig : ScriptableObject {

    [SerializeField] private ChestTypeData[] _chestTypes;
    public ChestTypeData[] ChestTypes => _chestTypes;
}
