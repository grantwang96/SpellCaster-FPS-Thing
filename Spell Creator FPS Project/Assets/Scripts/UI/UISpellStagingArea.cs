using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISpellStagingArea : UISubPanel, IUIViewGridParent {

    // the 3 rects that will contain the images

    [SerializeField] private RectTransform[] _componentsLists;
    [SerializeField] private RectTransform _imageTarget;
    [SerializeField] private UICustomButton _craftSpellButton;

    [SerializeField] private int[] _spellComponentsViewRowLengths;
    [SerializeField] private int[] _craftButtonViewRowLengths;

    private UISpellComponentSlot[][] _spellComponentSlots;

    [SerializeField] private UIViewGrid _spellComponentsView;
    [SerializeField] private UIViewGrid _craftButtonView;
    
    public delegate void SpellSlotSelected(string itemId);
    public event SpellSlotSelected OnSpellSlotSelected;
    public delegate void CraftSpellEvent();
    public event CraftSpellEvent OnCraftSpellPressed;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    public void Initialize() {
        UIViewGridInitData spellComponentsInit = new UIViewGridInitData();
        spellComponentsInit.RowLengths = _spellComponentsViewRowLengths;
        _spellComponentsView.Initialize(spellComponentsInit);
        _spellComponentsView.OnSelectPressed += OnSpellComponentSelected;

        UIViewGridInitData craftButtonInit = new UIViewGridInitData();
        craftButtonInit.RowLengths = _craftButtonViewRowLengths;
        _craftButtonView.Initialize(craftButtonInit);
        _craftButtonView.OnSelectPressed += OnCraftSpellButtonSelected;
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _craftButtonView.Active = newGrid == _craftButtonView;
        if (_craftButtonView.Active) {
            _craftButtonView.UpdateHighlightedViewCell(0, 0);
        }
        _spellComponentsView.Active = newGrid == _spellComponentsView;
        if (_spellComponentsView.Active) {
            _spellComponentsView.UpdateHighlightedViewCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        }
    }

    public void OutOfBounds(IntVector3 dir) {
        UISubPanel neighbor;
        if (dir == IntVector3.Up) {
            neighbor = _upNeighbor;
        } else if (dir == IntVector3.Right) {
            neighbor = _rightNeighbor;
        } else if (dir == IntVector3.Down) {
            neighbor = _downNeighbor;
        } else {
            neighbor = _leftNeighbor;
        }
        if (neighbor == null) {
            return;
        }
        _craftButtonView.Active = false;
        _craftButtonView.UnhighlightCell(_craftButtonView.CurrentItemX, _craftButtonView.CurrentItemY);
        _spellComponentsView.Active = false;
        _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        _parentPanel.ChangePanel(this, IntVector3.Zero);
    }

    public void SetUICastingMethod(Spell_CastingMethod castingMethod) {
        SpellComponentData componentData = new SpellComponentData(0, 0);
        // temp
        componentData.itemId = castingMethod.Id;
        componentData.Name = castingMethod.name;
        componentData.Text = castingMethod.name;

        _spellComponentsView.AddInteractableItemToRow(0, componentData);
    }

    public void AddUISpellEffect(Effect spell_Effect) {
        SpellComponentData componentData = new SpellComponentData(1, 0);
        componentData.itemId = spell_Effect.Id;
        componentData.Name = spell_Effect.name;
        componentData.Text = spell_Effect.name;

        _spellComponentsView.AddInteractableItemToRow(1, componentData);
    }

    public void AddUISpellModifier(SpellModifier spellModifier) {
        SpellComponentData componentData = new SpellComponentData(2, 0);
        componentData.itemId = spellModifier.Id;
        componentData.Name = spellModifier.name;
        componentData.Text = spellModifier.name;

        _spellComponentsView.AddInteractableItemToRow(2, componentData);
    }

    public void RemoveHighlightedSpellSlot() {
        int x = _spellComponentsView.CurrentItemX;
        int y = _spellComponentsView.CurrentItemY;
        _spellComponentsView.RemoveInteractableFromRow(x, y);
    }

    public void ClearSpellComponentSlots() {
        for (int i = 0; i < _spellComponentsViewRowLengths.Length; i++) {
            for(int j = 0; j < _spellComponentsViewRowLengths[i]; j++) {
                _spellComponentsView.RemoveInteractableFromRow(i, j);
            }
        }
    }

    private void OnSpellComponentSelected(IUIInteractable interactable) {
        string id = interactable.Id;
        if (id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            return;
        }
        OnSpellSlotSelected?.Invoke(id);
    }

    private void OnCraftSpellButtonSelected(IUIInteractable interactable) {
        OnCraftSpellPressed?.Invoke();
    }

    public override void SetActive(bool active, IntVector3 dir) {
        _spellComponentsView.Active = active;
        if (active) {
            _spellComponentsView.SetCurrentAtBound(dir);
            _spellComponentsView.UpdateHighlightedViewCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        } else {
            _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
            _craftButtonView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        }
    }
}
