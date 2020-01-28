using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Data that should be updated in animation handler
/// </summary>
[System.Serializable]
public class AnimationData
{
    [SerializeField] private string _animationName;
    [SerializeField] private List<string> _triggers = new List<string>();
    [SerializeField] private List<string> _resetTriggers = new List<string>();
    [SerializeField] private List<SerializedStringBool> _bools = new List<SerializedStringBool>();
    [SerializeField] private List<SerializedStringInt> _ints = new List<SerializedStringInt>();
    [SerializeField] private List<SerializedStringFloat> _floats = new List<SerializedStringFloat>();

    public string AnimationName => _animationName;
    public IReadOnlyList<string> Triggers => _triggers;
    public IReadOnlyList<string> ResetTriggers => _resetTriggers;
    public IReadOnlyList<SerializedStringBool> Bools => _bools;
    public IReadOnlyList<SerializedStringInt> Ints => _ints;
    public IReadOnlyList<SerializedStringFloat> Floats => _floats;
}
