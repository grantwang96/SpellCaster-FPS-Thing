using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericMessageBox : UIPanel {

    [SerializeField] private Text header;
    [SerializeField] private Text message;
    
    public override void Initialize(UIPanelInitData initData) {

    }
}
