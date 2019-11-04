using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueObject : MonoBehaviour
{
    [SerializeField] private Image _headImage;
    [SerializeField] private Text _textBox;
    [SerializeField] private RectTransform _viewRect;

    public void SetDialogue(Sprite sprite, string text) {
        _headImage.sprite = sprite;
        _textBox.text = text;
    }

    public void Display() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
