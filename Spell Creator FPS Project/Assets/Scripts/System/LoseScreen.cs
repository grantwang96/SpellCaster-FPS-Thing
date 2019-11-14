using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreen : UIPanel, IUIViewGridParent {

    protected const string RetryGameButtonId = "RETRY_GAME_BTN";
    protected const string QuitGameButtonId = "QUIT_GAME_BTN";

    [SerializeField] protected string _mainMenuPrefabId;
    [SerializeField] protected UIViewGrid _buttonsView;

    protected int[] _rowLengths = new int[] { 2 };

    public event UpdateActiveGrid OnUpdateActiveGrid;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        InitializeButtonView();
        LoseScreenInit(initData);
    }

    protected virtual void LoseScreenInit(UIPanelInitData initData) {

    }

    protected virtual void InitializeButtonView() {
        UIViewGridInitData initData = new UIViewGridInitData();
        initData.RowLengths = _rowLengths;
        _buttonsView.Initialize(initData);
        UICustomButtonInitData retryButtonInit = new UICustomButtonInitData() {
            Id = RetryGameButtonId,
            ButtonText = "Retry",
        };
        _buttonsView.AddInteractableItemToRow(0, retryButtonInit);
        UICustomButtonInitData quitButtonInit = new UICustomButtonInitData() {
            Id = QuitGameButtonId,
            ButtonText = "Quit",
        };
        _buttonsView.AddInteractableItemToRow(0, quitButtonInit);
        _buttonsView.OnSelectPressed += OnButtonViewSubmit;
        _buttonsView.SetActive(true);
    }

    protected virtual void RetryGame() {

    }

    protected virtual void QuitGame() {

    }

    private void OnButtonViewSubmit(IUIInteractable interactable) {
        switch (interactable.Id) {
            case RetryGameButtonId:
                RetryGame();
                break;
            case QuitGameButtonId:
                QuitGame();
                break;
        }
    }

    protected override void MenuBtnAction() {
        UIPanelManager.Instance.OpenUIPanel(_mainMenuPrefabId);
    }

    public IUIInteractable GetCurrentInteractable() {
        return _buttonsView.GetInteractableAt(_buttonsView.CurrentItemX, _buttonsView.CurrentItemY);
    }

    public void OutOfBounds(IntVector3 dir) {
        
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        
    }

    public override void ClosePanel() {
        base.ClosePanel();
        _buttonsView.OnSelectPressed -= OnButtonViewSubmit;
    }
}

public class LoseScreenInitData : UIPanelInitData {
    public List<ButtonActionData> ButtonDatas;
}
