using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Class that contains the actual data of the object
/// </summary>
public class InventoryViewCell : UIViewCell, IPointerClickHandler, IPointerEnterHandler {

    [SerializeField] private int _itemCount;
    public int ItemCount { get { return _itemCount; } }

    [SerializeField] private RectTransform _content;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _count;

    public override void Initialize(int x, int y) {
        InventoryViewCellData inventoryVCInitData = new InventoryViewCellData(x, y) {
            Id = GameplayValues.UI.EmptyInventoryItemId,
            Name = GameplayValues.UI.EmptyUIElementId
        };
        SetValue(inventoryVCInitData);
    }

    public override void SetValue(IUIInteractableData data) {
        if(data == null) {
            Debug.LogError("UIInteractable Data is null!");
            return;
        }
        InventoryViewCellData inventoryVCInitData = data as InventoryViewCellData;
        if (inventoryVCInitData == null) {
            inventoryVCInitData = new InventoryViewCellData(data.X, data.Y) {
                Id = GameplayValues.UI.EmptyInventoryItemId,
                Name = GameplayValues.UI.EmptyUIElementId
            };
        }
        _id = inventoryVCInitData.Id;
        _itemCount = inventoryVCInitData.itemCount;
        _count.text = _itemCount.ToString();
        xCoord = inventoryVCInitData.X;
        yCoord = inventoryVCInitData.Y;

        if (_id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            _icon.enabled = false;
            return;
        }
        _icon.enabled = true;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_id);
        _icon.sprite = storable.SmallIcon;
    }

    public override void InteractableHighlight() {
        _content.localScale = Vector3.one * 1.25f;
    }

    public override void InteractableUnhighlight() {
        _content.localScale = Vector3.one;
    }

    public override void InteractableSelect() {
        EventSystem.current.SetSelectedGameObject(null);
        OnSelect();
    }

    public override IUIInteractableData ExtractData() {
        InventoryViewCellData data = new InventoryViewCellData(xCoord, yCoord);
        data.Id = _id;
        data.Name = name;
        data.itemCount = _itemCount;
        return data;
    }

    public void OnPointerClick(PointerEventData eventData) {
        PointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnter();
    }
}

public class InventoryViewCellData : ViewCellData {
    public int itemCount = 0;

    public InventoryViewCellData(int x, int y) : base(x, y){
        
    }
}
