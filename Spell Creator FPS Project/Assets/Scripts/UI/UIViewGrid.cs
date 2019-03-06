using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateActiveGrid(int index);

public interface IUIViewGridParent {
    int ActiveGridIndex { get; }

    void UpdateActiveGrid(int index);
    void UpdateActivePanel(IntVector3 dir);

    event UpdateActiveGrid OnUpdateActiveGrid;
}

/// <summary>
/// Interactable view cell grids. 
/// </summary>
public class UIViewGrid : MonoBehaviour {

    [SerializeField] private IUIViewGridParent _parentPanel;
    public int GridIndex; // the index of this grid
    public bool Active; // is this grid currently receiving inputs?
    private UIInteractable[][] _interactableGrid; // the grid of interactable objects

    private int _currentItemX; // current highlightable object x
    private int _currentItemY; // current highlightable object y

    [SerializeField] private UIViewGrid _up;
    [SerializeField] private UIViewGrid _right;
    [SerializeField] private UIViewGrid _down;
    [SerializeField] private UIViewGrid _left;

    [Range(0f, 1f)] [SerializeField] private float _directionHoldThreshold;
    [Range(0f, 1f)] [SerializeField] private float _directionHoldFreq;

    private float _horizontal;
    private float _vertical;
    private float _intervalHoldTime;
    private float _holdTime;
    private bool _directionButtonsPressed;

    public delegate void SelectPressed(string id);
    public event SelectPressed OnSelectPressed;

    public virtual void Initialize(int[] ys) {
        _interactableGrid = new UIInteractable[ys.Length][];
        for(int i = 0; i < ys.Length; i++) {
            _interactableGrid[i] = new UIInteractable[ys[i]];
        }
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ProcessInputs() {
        if (!Active) { return; }
        DirectionalInputs();
        SelectButtonPressed();
    }

    private void DirectionalInputs() {
        float _horizontal = Input.GetAxisRaw("Horizontal");
        float _vertical = Input.GetAxisRaw("Vertical");

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
            TryChangeViewGrid(IntVector3.Left);
            return;
        } else if (x >= _interactableGrid.Length) {
            TryChangeViewGrid(IntVector3.Right);
            return;
        }
        if(_currentItemY >= _interactableGrid[x].Length) {
            y = _interactableGrid[x].Length - 1 - Mathf.RoundToInt(_vertical);
        }
        if (y < 0) {
            TryChangeViewGrid(IntVector3.Up);
            return;
        } else if (y >= _interactableGrid[x].Length) {
            TryChangeViewGrid(IntVector3.Down);
            return;
        }

        if(_interactableGrid[x][y] == null) {
            return;
        }

        UpdateHighlightedViewCell(x, y);
    }

    private void TryChangeViewGrid(IntVector3 dir) {
        UIViewGrid nextGrid;
        if (dir == IntVector3.Up) {
            nextGrid = _up;
        } else if(dir == IntVector3.Right) {
            nextGrid = _right;
        } else if(dir == IntVector3.Down) {
            nextGrid = _down;
        } else {
            nextGrid = _left;
        }
        if(nextGrid == null) {
            _parentPanel.UpdateActivePanel(dir);
            return;
        }
        _parentPanel.UpdateActiveGrid(nextGrid.GridIndex);
    }

    private void UpdateHighlightedViewCell(int x, int y) {
        _interactableGrid[_currentItemX][_currentItemY].Unhighlight();
        _currentItemX = x;
        _currentItemY = y;
        _interactableGrid[_currentItemX][_currentItemY].Highlight();
    }

    private void SelectButtonPressed() {
        if (Input.GetButtonDown("Submit")) {
            UIInteractable selected = _interactableGrid[_currentItemX][_currentItemY];
            OnSelectPressed?.Invoke(selected.Id);
        }
    }

    public void SetInteractableItem(int x, int y, UIInteractable interactable) {
        _interactableGrid[x][y] = interactable;
    }

    public void ClearInteractableItem(int x, int y) {
        _interactableGrid[x][y] = null;
    }

    private void OnActiveGridUpdated(int index) {
        Active = index == GridIndex;
    }
}

public class UIViewGridInitData {
    public int x;
    public int y;
}
