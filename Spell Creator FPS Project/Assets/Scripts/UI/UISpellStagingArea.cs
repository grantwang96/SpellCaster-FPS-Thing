using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellStagingArea : UISubPanel {

    // the 3 rects that will contain the images

    [SerializeField] private RectTransform[] _componentsLists;
    [SerializeField] private RectTransform _imageTarget;
    [SerializeField] private UICustomButton _craftSpellButton;

    private UISpellComponentSlot[][] _spellComponentSlots;
    
    [SerializeField] private UISpellComponentSlot _componentSlotPrefab;
    [SerializeField] private RectTransform _componentListPrefab;
    [SerializeField] private RectTransform _imageTargetPrefab;

    [Range(0f, 1f)] [SerializeField] private float _directionHoldThreshold;
    [Range(0f, 1f)] [SerializeField] private float _directionHoldFreq;

    private float _horizontal;
    private float _vertical;
    private float _intervalHoldTime;
    private float _holdTime;
    private bool _directionButtonsPressed;

    [SerializeField] private int _currentItemX;
    [SerializeField] private int _currentItemY;

    public delegate void SpellSlotSelected(string itemId);
    public event SpellSlotSelected OnSpellSlotSelected;
    public delegate void CraftSpellEvent();
    public event CraftSpellEvent OnCraftSpellPressed;

    public override void Initialize(UIPanelInitData initData = null) {
        // clear any remaining elements in the components list
        _spellComponentSlots = new UISpellComponentSlot[3][];
        for(int i = 0; i < _spellComponentSlots.Length; i++) {
            switch (i) {
                case 0:
                    _spellComponentSlots[i] = new UISpellComponentSlot[GameplayValues.MaximumSpellCastingMethods];
                    break;
                case 1:
                    _spellComponentSlots[i] = new UISpellComponentSlot[GameplayValues.MaximumSpellEffects];
                    break;
                default:
                    _spellComponentSlots[i] = new UISpellComponentSlot[GameplayValues.MaximumSpellModifiers];
                    break;
            }
            for(int j = 0; j < _spellComponentSlots[i].Length; j++) {
                UISpellComponentSlot newSlot = Instantiate(_componentSlotPrefab, _componentsLists[i]);
                SpellComponentInitData slotInitData = SpellComponentInitData.Default;
                slotInitData.ImageParent = _imageTarget;
                newSlot.Initialize(slotInitData);
                _spellComponentSlots[i][j] = newSlot;
            }
        }
        _parentPanel.OnSubPanelChanged += OnSubPanelChanged;
        _craftSpellButton.onClick.AddListener(CraftSpell);
        _currentItemX = 0;
        _currentItemY = 0;
        // register events
    }

    protected override void ProcessInputs() {
        base.ProcessInputs();
        DirectionalInputs();
        SelectPressed();
    }

    private void DirectionalInputs() {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        // if no buttons are being pressed, reset values and carry on.
        if (_horizontal == 0 && _vertical == 0) {
            _holdTime = 0f;
            _intervalHoldTime = 0f;
            _directionButtonsPressed = false;
            return;
        }

        // if the button is being held
        if (_directionButtonsPressed) {
            if (_holdTime < _directionHoldThreshold) { // check if they're waiting to do continuous movement
                _holdTime += Time.deltaTime;
                return;
            }
            if (_intervalHoldTime < _directionHoldFreq) { // check if they're waiting on interval
                _intervalHoldTime += Time.deltaTime;
                return;
            }
        }

        // Process the actual movement;
        _directionButtonsPressed = true;
        _intervalHoldTime = 0f;

        int x = _currentItemX + Mathf.RoundToInt(_horizontal);
        int y = _currentItemY - Mathf.RoundToInt(_vertical);

        if (x < 0) {
            _parentPanel.ChangePanel(new IntVector3(-1, 0, 0));
            return;
        }
        else if (x >= _spellComponentSlots.Length) {
            _parentPanel.ChangePanel(new IntVector3(1, 0, 0));
            return;
        }
        if(y >= _spellComponentSlots[x].Length) {
            y = _spellComponentSlots[x].Length - 1;
        }

        if (y < 0) {
            _spellComponentSlots[_currentItemX][_currentItemY].Unhighlight();
            // highlight button
            return;
        }
        else if (y >= _spellComponentSlots[x].Length) {
            y = _spellComponentSlots[x].Length - 1;
        }

        UpdateHighlightedViewCell(x, y);
    }

    private void UpdateHighlightedViewCell(int x, int y) {
        _spellComponentSlots[_currentItemX][_currentItemY].Unhighlight();
        _currentItemX = x;
        _currentItemY = y;
        _spellComponentSlots[_currentItemX][_currentItemY].Highlight();
    }

    private void SelectPressed() {
        if (Input.GetButtonDown("Submit")) {
            UISpellComponentSlot selected = _spellComponentSlots[_currentItemX][_currentItemY];
            OnSpellSlotSelected?.Invoke(selected.ItemId);
        }
    }

    public void SetUICastingMethod(Spell_CastingMethod castingMethod) {
        SpellComponentInitData initData = new SpellComponentInitData() {
            Name = castingMethod.Id,
            itemId = castingMethod.Id,
            Text = castingMethod.name,
            ImageParent = _imageTarget
        };
        _spellComponentSlots[0][0].Initialize(initData);
    }

    public void AddUISpellEffect(Spell_Effect spell_Effect) {
        for(int i = 0; i < _spellComponentSlots[1].Length; i++) {
            if (_spellComponentSlots[1][i].ItemId.Equals(GameplayValues.EmptyInventoryItemId)) {
                SpellComponentInitData initData = new SpellComponentInitData() {
                    Name = spell_Effect.Id,
                    itemId = spell_Effect.Id,
                    Text = spell_Effect.name,
                    ImageParent = _imageTarget
                };
                _spellComponentSlots[1][i].Initialize(initData);
                return;
            }
        }
        Debug.Log("Spell Effect slots are full!");
    }

    public void AddUISpellModifier(SpellModifier modifier) {
        for (int i = 0; i < _spellComponentSlots[2].Length; i++) {
            if (_spellComponentSlots[2][i].ItemId.Equals(GameplayValues.EmptyInventoryItemId)) {
                SpellComponentInitData initData = new SpellComponentInitData() {
                    Name = modifier.Id,
                    itemId = modifier.Id,
                    Text = modifier.name,
                    ImageParent = _imageTarget
                };
                _spellComponentSlots[2][i].Initialize(initData);
                return;
            }
        }
        Debug.Log("Spell Effect slots are full!");
    }

    public void RemoveHighlightedSpellSlot() {
        UISpellComponentSlot highlighted = _spellComponentSlots[_currentItemX][_currentItemY];
        for(int i = _currentItemY; i < _spellComponentSlots[_currentItemX].Length; i++) {
            int next = i + 1;
            SpellComponentInitData initData;
            if(next > _spellComponentSlots[_currentItemX].Length - 1) {
                initData = SpellComponentInitData.Default;
                _spellComponentSlots[_currentItemX][i].Initialize(initData);
                continue;
            }
            initData = new SpellComponentInitData() {
                itemId = _spellComponentSlots[_currentItemX][next].ItemId,
                Text = _spellComponentSlots[_currentItemX][next].Text,
                Name = _spellComponentSlots[_currentItemX][next].name,
                ImageParent = _imageTarget
            };
            _spellComponentSlots[_currentItemX][i].Initialize(initData);
        }
    }

    private void CraftSpell() {
        OnCraftSpellPressed?.Invoke();
    }

    protected override void OnSubPanelChanged() {
        if (!ActiveSubPanel) {
            _spellComponentSlots[_currentItemX][_currentItemY].Unhighlight();
            return;
        }
        _spellComponentSlots[_currentItemX][_currentItemY].Highlight();
    }
}

public static partial class GameplayValues {

    public const string EmptySpellStageText = "-----";
    public const string EmptyUIElementId = "EMPTY_UI_ELEMENT";
}