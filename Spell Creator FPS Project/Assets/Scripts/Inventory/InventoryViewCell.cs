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
            itemId = GameplayValues.EmptyInventoryItemId,
            Name = GameplayValues.EmptyUIElementId
        };
        SetValue(inventoryVCInitData);
    }

    public override void SetValue(IUIInteractableData data) {
        if(data == null) {
            Debug.LogError("UIINteractable Data is null!");
            return;
        }
        InventoryViewCellData inventoryVCInitData = data as InventoryViewCellData;
        if (inventoryVCInitData == null) {
            inventoryVCInitData = new InventoryViewCellData(data.X, data.Y) {
                itemId = GameplayValues.EmptyInventoryItemId,
                Name = GameplayValues.EmptyUIElementId
            };
        }
        _id = inventoryVCInitData.itemId;
        _itemCount = inventoryVCInitData.itemCount;
        _count.text = _itemCount.ToString();
        xCoord = inventoryVCInitData.X;
        yCoord = inventoryVCInitData.Y;

        if (_id.Equals(GameplayValues.EmptyInventoryItemId)) {
            _icon.enabled = false;
            return;
        }
        _icon.enabled = true;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_id);
        _icon.sprite = storable.Icon;
    }

    public override void Highlight() {
        _content.localScale = Vector3.one * 1.25f;
    }

    public override void Unhighlight() {
        _content.localScale = Vector3.one;
    }

    public override IUIInteractableData ExtractData() {
        InventoryViewCellData data = new InventoryViewCellData(xCoord, yCoord);
        data.itemId = _id;
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
