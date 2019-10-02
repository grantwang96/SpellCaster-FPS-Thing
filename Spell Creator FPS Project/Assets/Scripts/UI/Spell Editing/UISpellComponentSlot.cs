using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISpellComponentSlot : UIViewCell, IPointerClickHandler, IPointerEnterHandler {
    
    public string Text { get; protected set; }

    [SerializeField] private Text _text;

    public override void Initialize(int x, int y) {
        SpellComponentData componentData = new SpellComponentData(x, y) {
            Id = GameplayValues.UI.EmptyInventoryItemId,
            Name = GameplayValues.UI.EmptyUIElementId,
            Text = GameplayValues.UI.EmptySpellStageText,
        };
        SetValue(componentData);
    }

    public override void SetValue(IUIInteractableData initData) {
        SpellComponentData componentData = initData as SpellComponentData;
        if (componentData == null) {
            componentData = new SpellComponentData(initData.X, initData.Y) {
                Id = GameplayValues.UI.EmptyInventoryItemId,
                Name = GameplayValues.UI.EmptyUIElementId,
                Text = GameplayValues.UI.EmptySpellStageText,
            };
        }
        xCoord = initData.X;
        yCoord = initData.Y;
        _id = componentData.Id;
        if (componentData.Id == GameplayValues.UI.EmptyInventoryItemId) {
            _text.text = GameplayValues.UI.EmptySpellStageText;
            name = GameplayValues.UI.EmptyUIElementId;
            return;
        }
        _text.text = componentData.Text;
        name = componentData.Name;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(componentData.Id);
    }

    public override void InteractableHighlight() {
        _text.rectTransform.localScale = Vector3.one * 1.25f;
    }

    public override void InteractableUnhighlight() {
        _text.rectTransform.localScale = Vector3.one;
    }

    public override void InteractableSelect() {
        EventSystem.current.SetSelectedGameObject(null);
        OnSelect();
    }

    public override IUIInteractableData ExtractData() {
        SpellComponentData data = new SpellComponentData(xCoord, yCoord);
        data.Id = _id;
        data.Text = _text.text;
        data.Name = name;
        return data;
    }

    public void OnPointerClick(PointerEventData eventData) {
        PointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnter();
    }
}

public class SpellComponentData : ViewCellData {
    public string Text;

    public SpellComponentData(int x, int y) : base(x, y) {

    }
}
