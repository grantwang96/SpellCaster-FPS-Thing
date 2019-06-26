﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISpellStagingArea : UISubPanel, IUIViewGridParent {

    // the 3 rects that will contain the images

    [SerializeField] private RectTransform[] _componentsLists;
    [SerializeField] private RectTransform _imageTarget;
    [SerializeField] private UICustomButton _craftSpellButton;

    [SerializeField] private string _cachedSpellName;
    public string SpellName => _cachedSpellName;

    [SerializeField] private int[] _spellNameEditorViewRowLengths;
    [SerializeField] private int[] _spellComponentsViewRowLengths;
    [SerializeField] private int[] _craftButtonViewRowLengths;

    private UISpellComponentSlot[][] _spellComponentSlots;

    [SerializeField] private UIViewGrid _spellNameEditorView;
    [SerializeField] private UIViewGrid _spellComponentsView;
    [SerializeField] private UIViewGrid _craftButtonView;
    
    public delegate void SpellSlotEvent(string itemId);
    public event SpellSlotEvent OnSpellSlotHighlighted;
    public event SpellSlotEvent OnSpellSlotSelected;
    public delegate void CraftSpellEvent();
    public event CraftSpellEvent OnCraftSpellPressed;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    public void Initialize() {
        UIViewGridInitData spellNameEditorInitData = new UIViewGridInitData();
        spellNameEditorInitData.RowLengths = _spellNameEditorViewRowLengths;
        _spellNameEditorView.Initialize(spellNameEditorInitData);
        _spellNameEditorView.OnSelectPressed += OnSpellNameEditorSelected;
        _spellNameEditorView.OnHighlighted += OnSpellNameEditorHighlighted;

        UIViewGridInitData spellComponentsInit = new UIViewGridInitData();
        spellComponentsInit.RowLengths = _spellComponentsViewRowLengths;
        _spellComponentsView.Initialize(spellComponentsInit);
        _spellComponentsView.OnSelectPressed += OnSpellComponentSelected;
        _spellComponentsView.OnHighlighted += OnSpellComponentHighlighted;

        UIViewGridInitData craftButtonInit = new UIViewGridInitData();
        craftButtonInit.RowLengths = _craftButtonViewRowLengths;
        _craftButtonView.Initialize(craftButtonInit);
        _craftButtonView.OnSelectPressed += OnCraftSpellButtonSelected;
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _craftButtonView.Active = newGrid == _craftButtonView;
        _spellComponentsView.Active = newGrid == _spellComponentsView;
        _spellNameEditorView.Active = newGrid == _spellNameEditorView;
        newGrid.UpdateHighlightedViewCell(newGrid.CurrentItemX, newGrid.CurrentItemY);
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
        _spellNameEditorView.Active = false;
        _spellNameEditorView.UnhighlightCell(_spellNameEditorView.CurrentItemX, _spellNameEditorView.CurrentItemY);
        _parentPanel.ChangePanel(neighbor, dir);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        _parentPanel.ChangePanel(this, IntVector3.Zero);
    }

    public void SetUICastingMethod(Spell_CastingMethod castingMethod) {
        SpellComponentData componentData = new SpellComponentData(0, 0);
        // temp
        componentData.Id = castingMethod.Id;
        componentData.Name = castingMethod.Name;
        componentData.Text = castingMethod.Name;

        // _spellComponentsView.AddInteractableItemToRow(0, componentData);
        _spellComponentsView.SetInteractableItem(0, 0, componentData);
    }

    public void AddUISpellEffect(Effect spell_Effect) {
        SpellComponentData componentData = new SpellComponentData(1, 0);
        componentData.Id = spell_Effect.Id;
        componentData.Name = spell_Effect.Name;
        componentData.Text = spell_Effect.ShortDescription;

        _spellComponentsView.AddInteractableItemToRow(1, componentData);
    }

    public void AddUISpellModifier(SpellModifier spellModifier) {
        SpellComponentData componentData = new SpellComponentData(2, 0);
        componentData.Id = spellModifier.Id;
        componentData.Name = spellModifier.Name;
        componentData.Text = spellModifier.ShortDescription;

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

    private void OnSpellNameEditorHighlighted(IUIInteractable interactable) {

    }

    private void OnSpellNameEditorSelected(IUIInteractable interactable) {

    }

    private void OnSpellComponentHighlighted(IUIInteractable interactable) {
        string id = interactable.Id;
        if (id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            OnSpellSlotHighlighted?.Invoke(id);
            return;
        }
        OnSpellSlotHighlighted?.Invoke(id);
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
