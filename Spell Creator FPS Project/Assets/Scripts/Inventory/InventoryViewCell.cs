using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class that contains the actual data of the object
/// </summary>
public class InventoryViewCell : MonoBehaviour {

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

    public void Initialize(InventoryView inventoryView, int x, int y, string itemId, int itemCount = 0) {
        _inventoryView = inventoryView;
        GridX = x;
        GridY = y;
        _itemId = itemId;
        _itemCount = itemCount;
        _count.text = _itemCount.ToString();

        if (_itemId.Equals(GameplayValues.EmptyInventoryItemId)) {
            _icon.enabled = false;
            return;
        }
        _icon.enabled = true;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(itemId);
        _icon.sprite = storable.Sprite;
    }

    public void Highlight() {
        _content.localScale = Vector3.one * 1.25f;
    }

    public void Dehighlight() {
        _content.localScale = Vector3.one;
    }
}
