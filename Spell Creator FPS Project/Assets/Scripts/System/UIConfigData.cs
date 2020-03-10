using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UIConfigData")]
public class UIConfigData : ScriptableObject
{
    [SerializeField] protected Color _neutralUIColor;
    [SerializeField] protected Color _highlightedUIColor;
    [SerializeField] protected Color _deniedUIColor;

    public Color NeutralUIColor => _neutralUIColor;
    public Color HighlightedUIColor => _highlightedUIColor;
    public Color DeniedUIColor => _deniedUIColor;
}
