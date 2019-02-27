using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class that contains the actual data of the object
/// </summary>
public class InventoryViewCell : UIViewCell {

    [SerializeField] private string _itemId;
    public string ItemId { get { return _itemId; } }
    [SerializeField] private int _itemCount;
    public int ItemCount { get { return _itemCount; } }

    public int GridX;
    public int GridY;

    [SerializeField] private RectTransform _content;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _count;

    private InventoryView _inventoryView;

    public override void Initialize(ViewCellInitData initData) {
        InventoryViewCellInitData inventoryVCInitData = initData as InventoryViewCellInitData;
        if(inventoryVCInitData == null) {
            Debug.LogError("Init data passed was not InventoryViewCellInitData!");
            return;
        }

        _inventoryView = inventoryVCInitData.inventoryView;
        GridX = inventoryVCInitData.x;
        GridY = inventoryVCInitData.y;
        _itemId = inventoryVCInitData.itemId;
        _itemCount = inventoryVCInitData.itemCount;
        _count.text = _itemCount.ToString();

        if (_itemId.Equals(GameplayValues.EmptyInventoryItemId)) {
            _icon.enabled = false;
            return;
        }
        _icon.enabled = true;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(_itemId);
        _icon.sprite = storable.Sprite;
    }

    public override void Highlight() {
        _content.localScale = Vector3.one * 1.25f;
    }

    public override void Unhighlight() {
        _content.localScale = Vector3.one;
    }
}

public class InventoryViewCellInitData : ViewCellInitData {

    public InventoryView inventoryView;
    public int x;
    public int y;
    public int itemCount = 0;
}
