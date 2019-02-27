using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpellStagingArea : UISubPanel {

    // the 3 rects that will contain the images

    [SerializeField] private RectTransform[] _componentsLists;
    [SerializeField] private RectTransform[] _imageTargets;

    [SerializeField] private UISpellComponentSlot[][] _spellComponentSlots;
    
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
                SpellComponentInitData slotInitData = new SpellComponentInitData() {
                    itemId = GameplayValues.EmptyInventoryItemId,
                    Name = GameplayValues.EmptyUIElementId,
                    Text = GameplayValues.EmptySpellStageText,
                    ImageParent = _imageTargets[i]
                };
                newSlot.Initialize(slotInitData);
                _spellComponentSlots[i][j] = newSlot;
            }
        }
        _parentPanel.OnSubPanelChanged += OnSubPanelChanged;
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
        _currentItemX = x;
        if(_currentItemY >= _spellComponentSlots[x].Length) {
            _currentItemY = _spellComponentSlots[x].Length - 1;
        }

        if (y < 0) {
            _parentPanel.ChangePanel(new IntVector3(0, -1, 0));
            return;
        }
        else if (y >= _spellComponentSlots[_currentItemX].Length) {
            y = _spellComponentSlots[x].Length - 1;
        }
        _currentItemY = y;

        UpdateHighlightedViewCell();
    }

    private void UpdateHighlightedViewCell() {
        for (int i = 0; i < _spellComponentSlots.Length; i++) {
            for(int j = 0; j < _spellComponentSlots[i].Length; j++) {
                if(i == _currentItemX && j == _currentItemY) {
                    _spellComponentSlots[i][j].Highlight();
                    continue;
                }
                _spellComponentSlots[i][j].Unhighlight();
            }
        }
    }

    private void SelectPressed() {

    }

    public void SetUICastingMethod(Spell_CastingMethod castingMethod) {
        SpellComponentInitData initData = new SpellComponentInitData() {
            Name = castingMethod.Id,
            itemId = castingMethod.Id,
            Text = castingMethod.name,
            ImageParent = _imageTargets[0]
        };
        _spellComponentSlots[0][0].Initialize(initData);
    }

    public void AddUISpellEffect(Spell_Effect spell_Effect) {
        // add a text prefab to the appropriate components list
        // add an image prefab to the appropriate image target
    }

    public void AddUISpellModifier(SpellModifier modifier) {
        // add a text prefab to the appropriate components list
        // add an image prefab to the appropriate image target
    }

    private void GenerateComponentText(Transform parent, string id, string text) {
        RectTransform newText = Instantiate(_componentListPrefab, parent);
        newText.name = id;
        newText.GetComponentInChildren<Text>().text = text;
    }

    private void GenerateImageComponent(Transform parent, string id, Sprite sprite) {
        RectTransform newImage = Instantiate(_imageTargetPrefab, parent);
        newImage.name = id;
        newImage.GetComponentInChildren<Image>().sprite = sprite;
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