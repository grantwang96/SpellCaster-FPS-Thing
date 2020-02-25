using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _fill;

    private float _currentPercent;

    private void Awake() {
        UpdateFill(0f);
    }

    public void SetBarSize(Vector2 sizeDelta) {
        _background.rectTransform.sizeDelta = sizeDelta;
        UpdateFill(_currentPercent);
    }

    public void UpdateFill(float percent) {
        _currentPercent = Mathf.Clamp01(percent);
        Vector2 maxSize = _background.rectTransform.sizeDelta;
        float xSize = percent * maxSize.x;
        Vector2 fillSizeDelta = new Vector2(xSize, maxSize.y);
        _fill.rectTransform.sizeDelta = fillSizeDelta;
    }
}
