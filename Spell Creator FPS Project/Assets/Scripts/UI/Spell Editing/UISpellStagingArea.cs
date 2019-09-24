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

    [SerializeField] private string _cachedSpellName;
    public string SpellName => _cachedSpellName;

    [SerializeField] private int[] _spellNameEditorViewRowLengths;
    [SerializeField] private int[] _spellComponentsViewRowLengths;
    [SerializeField] private int[] _craftButtonViewRowLengths;

    [SerializeField] private UIViewGrid _spellNameEditorView;
    [SerializeField] private UIViewGrid _spellComponentsView;
    [SerializeField] private UIViewGrid _craftButtonView;

    private UIViewGrid _cachedUIViewGrid;
    private IUIInteractable _currentInteractable;

    public delegate void SpellSlotsUpdatedEvent(string itemId);
    public event SpellSlotsUpdatedEvent OnSpellSlotsUpdated;

    public delegate void SpellSlotEvent(string itemId);
    public event SpellSlotEvent OnSpellSlotHighlighted;
    public event SpellSlotEvent OnSpellSlotSelected;

    public delegate void CraftSpellEvent();
    public event CraftSpellEvent OnCraftSpellPressed;
    public event UpdateActiveGrid OnUpdateActiveGrid;

    public override void Initialize(UIPanelInitData initData) {
        base.Initialize(initData);
        UIViewGridInitData spellNameEditorInitData = new UIViewGridInitData();
        spellNameEditorInitData.RowLengths = _spellNameEditorViewRowLengths;
        _spellNameEditorView.Initialize(spellNameEditorInitData);
        _spellNameEditorView.OnSelectPressed += OnSpellNameEditorSelected;
        _spellNameEditorView.OnHighlighted += OnSpellNameEditorHighlighted;

        UIViewGridInitData spellComponentsInit = new UIViewGridInitData();
        spellComponentsInit.RowLengths = _spellComponentsViewRowLengths;
        _spellComponentsView.Initialize(spellComponentsInit);
        _spellComponentsView.SetCurrentAtBound(IntVector3.Right);
        _spellComponentsView.OnSelectPressed += OnSpellComponentSelected;
        _spellComponentsView.OnHighlighted += OnSpellComponentHighlighted;

        UIViewGridInitData craftButtonInit = new UIViewGridInitData();
        craftButtonInit.RowLengths = _craftButtonViewRowLengths;
        _craftButtonView.Initialize(craftButtonInit);
        _craftButtonView.OnSelectPressed += OnCraftSpellButtonSelected;
        _craftButtonView.OnHighlighted += OnCraftSpellButtonHighlighted;
    }

    public void UpdateActiveGrid(UIViewGrid newGrid) {
        _craftButtonView.SetActive(newGrid == _craftButtonView);
        _spellComponentsView.SetActive(newGrid == _spellComponentsView);
        _spellNameEditorView.SetActive(newGrid == _spellNameEditorView);
        newGrid.UpdateHighlightedViewCell(newGrid.CurrentItemX, newGrid.CurrentItemY);
        _currentInteractable = newGrid.GetInteractableAt(newGrid.CurrentItemX, newGrid.CurrentItemY);
        _cachedUIViewGrid = newGrid;
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
        _craftButtonView.SetActive(false);
        _craftButtonView.UnhighlightCell(_craftButtonView.CurrentItemX, _craftButtonView.CurrentItemY);
        _spellComponentsView.SetActive(false);
        _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        _spellNameEditorView.SetActive(false);
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
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
    }

    public void RemoveHighlightedSpellComponent() {
        int x = _spellComponentsView.CurrentItemX;
        int y = _spellComponentsView.CurrentItemY;
        _spellComponentsView.RemoveInteractableFromRow(x, y);
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(x, y).Id);
    }

    public void ClearSpellComponentSlots() {
        for (int i = 0; i < _spellComponentsViewRowLengths.Length; i++) {
            for(int j = 0; j < _spellComponentsViewRowLengths[i]; j++) {
                _spellComponentsView.RemoveInteractableFromRow(i, j);
            }
        }
        OnSpellSlotsUpdated?.Invoke(_spellComponentsView.GetInteractableAt(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY).Id);
    }

    private void OnSpellNameEditorHighlighted(IUIInteractable interactable) {
        _craftButtonView.SetActive(false);
        _spellComponentsView.SetActive(false);
        _spellNameEditorView.SetActive(true);
    }

    private void OnSpellNameEditorSelected(IUIInteractable interactable) {

    }

    private void OnSpellComponentHighlighted(IUIInteractable interactable) {
        _craftButtonView.SetActive(false);
        _spellComponentsView.SetActive(true);
        _spellNameEditorView.SetActive(false);
        string id = interactable.Id;
        OnSpellSlotHighlighted?.Invoke(id);
    }

    private void OnSpellComponentSelected(IUIInteractable interactable) {
        string id = interactable.Id;
        if (id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
            return;
        }
        OnSpellSlotSelected?.Invoke(id);
    }

    private void OnCraftSpellButtonHighlighted(IUIInteractable interactable) {
        Debug.Log($"Craft Spell Button Highlighted!");
        _spellComponentsView.UnhighlightCell(_spellComponentsView.CurrentItemX, _spellComponentsView.CurrentItemY);
        _spellNameEditorView.UnhighlightCell(_spellNameEditorView.CurrentItemX, _spellNameEditorView.CurrentItemY);
        _craftButtonView.SetActive(true);
        _spellComponentsView.SetActive(false);
        _spellNameEditorView.SetActive(false);
    }

    private void OnCraftSpellButtonSelected(IUIInteractable interactable) {
        OnCraftSpellPressed?.Invoke();
    }

    public override void SetFocus(bool active, bool hardLocked, IntVector3 dir) {
        base.SetFocus(active, hardLocked, dir);
        if (active) {
            _spellComponentsView.SetCurrentAtBound(dir);
        }
        _spellComponentsView.SetActive(active, hardLocked);
        _craftButtonView.SetActive(false, hardLocked);
        _spellNameEditorView.SetActive(false, hardLocked);
    }

    public IUIInteractable GetCurrentInteractable() {
        return _currentInteractable;
    }

    protected override void OnActivePanelUpdated(bool isCurrentPanel) {
        base.OnActivePanelUpdated(isCurrentPanel);
        if (isCurrentPanel) {
            Debug.Log($"Cached UIViewGrid: {_cachedUIViewGrid?.name}");
            Debug.Log($"Is Focused: {IsFocused}");
            _spellNameEditorView.SetActive(_spellNameEditorView == _cachedUIViewGrid && IsFocused);
            _spellComponentsView.SetActive(_spellComponentsView == _cachedUIViewGrid && IsFocused);
            _craftButtonView.SetActive(_craftButtonView == _cachedUIViewGrid && IsFocused);
        } else {
            _spellNameEditorView.SetActive(false);
            _spellComponentsView.SetActive(false);
            _craftButtonView.SetActive(false);
        }
    }
}
