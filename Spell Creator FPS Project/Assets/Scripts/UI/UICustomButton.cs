using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICustomButton : Button, UIInteractable {

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private RectTransform _rect;

    public void Initialize(UIInteractableInitData initData) {

    }

    public void Highlight() {
        _rect.localScale = Vector3.one * 1.25f;
    }

    public void Unhighlight() {
        _rect.localScale = Vector3.one;
    }

    // Use this for initialization


    // Update is called once per frame
    void Update () {
		
	}
}
