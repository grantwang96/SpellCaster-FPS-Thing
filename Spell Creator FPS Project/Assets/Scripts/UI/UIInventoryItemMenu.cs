using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventoryItemMenu : UIPanel {

    [SerializeField] private Button _menuButtonPrefab;
    [SerializeField] private RectTransform _content;

    public void AddItemOption(string buttonId, UnityAction buttonAction) {
        Button newMenuOption = Instantiate(_menuButtonPrefab, _content);
        newMenuOption.onClick.AddListener(buttonAction);
    }
}
