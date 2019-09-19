using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays information about given inventory item
/// </summary>
public class UISpellComponentDescView : MonoBehaviour {

    // spell component name field
    [SerializeField] private Text _currentName;
    // spell component type field
    [SerializeField] private Text _currentType;
    // spell component large icon field
    [SerializeField] private Image _currentLargeIcon;
    // spell component description field
    [SerializeField] private Text _currentDescription;

    // Called by parent menu
    public void UpdateDescription(string itemId) {
        if(itemId.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            // clear description values
            ClearDescriptionView();
            return;
        }
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(itemId);
        if(storable == null) {
            Debug.LogError($"[{nameof(UISpellComponentDescView)}] Could not find inventory item {itemId} in registry!");
            ClearDescriptionView();
            return;
        }
        _currentName.text = storable.Name;
        _currentType.text = ParseItemType(storable.ItemType);
        _currentLargeIcon.enabled = true;
        _currentLargeIcon.sprite = storable.SmallIcon; // temp
        _currentDescription.text = storable.LongDescription;
    }

    private void ClearDescriptionView() {
        _currentName.text = string.Empty;
        _currentType.text = string.Empty;
        _currentLargeIcon.enabled = false;
        _currentDescription.text = string.Empty;
    }

    private string ParseItemType(InventoryItemType itemType) {
        string itemTypeText = "";
        switch (itemType) {
            case InventoryItemType.CASTINGMETHOD:
                itemTypeText = "Casting Method";
                break;
            case InventoryItemType.SPELLEFFECT:
                itemTypeText = "Spell Effect";
                break;
            case InventoryItemType.SPELLMODIFIER:
                itemTypeText = "Spell Modifier";
                break;
            default:
                break;
        }
        return itemTypeText;
    }
}