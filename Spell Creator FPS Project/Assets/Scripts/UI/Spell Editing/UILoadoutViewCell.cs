﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadoutViewCell : UIViewCell {

    public override IUIInteractableData ExtractData() {
        throw new System.NotImplementedException();
    }

    public override void InteractableHighlight() {
        throw new System.NotImplementedException();
    }

    public override void Initialize(int x, int y) {
        throw new System.NotImplementedException();
    }

    public override void SetValue(IUIInteractableData data) {
        throw new System.NotImplementedException();
    }

    public override void InteractableUnhighlight() {
        throw new System.NotImplementedException();
    }

    public override void InteractableSelect() {
        OnSelect();
    }
}
