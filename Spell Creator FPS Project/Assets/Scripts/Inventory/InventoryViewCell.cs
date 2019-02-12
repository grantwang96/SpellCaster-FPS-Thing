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

    [SerializeField] private Image _icon;
    [SerializeField] private Text _count;

    public void Initialize(string itemId, int itemCount = 0) {
        _itemId = itemId;
        _itemCount = itemCount;
        _count.text = _itemCount.ToString();

        if (_itemId.Equals("NONE")) {
            _icon.enabled = false;
            return;
        }
        _icon.enabled = true;
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(itemId);
        _icon.sprite = storable.Sprite;
    }
}
