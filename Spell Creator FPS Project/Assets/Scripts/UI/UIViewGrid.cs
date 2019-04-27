using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateActiveGrid();

public interface IUIViewGridParent {

    void UpdateActiveGrid(UIViewGrid newGrid);
    void OutOfBounds(IntVector3 dir);

    event UpdateActiveGrid OnUpdateActiveGrid;
}

/// <summary>
/// Interactable view cell grids.
/// Note: They don't actually know what to parent things to, the child will have that information.
/// </summary>
public class UIViewGrid : MonoBehaviour {

    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    [SerializeField] private GameObject _parentPanelGameObject;
    private IUIViewGridParent _parentPanel;
    public bool Active; // is this grid currently receiving inputs?
    private IUIInteractable[][] _interactableGrid; // the grid of interactable objects

    [SerializeField] private RectTransform _content; // where the main content will be held
    [SerializeField] private RectTransform _rowPrefab; // an empty rect that will be used to generate the rows of the grid
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private int _currentItemX;
    public int CurrentItemX => _currentItemX;
    [SerializeField] private int _currentItemY;
    public int CurrentItemY => _currentItemY;

    [SerializeField] private UIViewGrid _up;
    [SerializeField] private UIViewGrid _right;
    [SerializeField] private UIViewGrid _down;
    [SerializeField] private UIViewGrid _left;

    [Range(0f, 1f)] [SerializeField] private float _directionHoldThreshold;
    [Range(0f, 1f)] [SerializeField] private float _directionHoldFreq;
    [SerializeField] private bool _inverted;

    private float _horizontal;
    private float _vertical;
    private float _intervalHoldTime;
    private float _holdTime;
    private bool _directionButtonsPressed;

    public delegate void SelectPressed(IUIInteractable interactable);
    public event SelectPressed OnSelectPressed;

    public virtual void Initialize(UIViewGridInitData viewGridInitData) {
        _parentPanel = _parentPanelGameObject.GetComponent<IUIViewGridParent>();
        _interactableGrid = new IUIInteractable[viewGridInitData.RowLengths.Length][];
        for(int i = 0; i < viewGridInitData.RowLengths.Length; i++) {
            RectTransform row = Instantiate(_rowPrefab, _content);
            row.gameObject.SetActive(true);
            _interactableGrid[i] = new IUIInteractable[viewGridInitData.RowLengths[i]];
            for (int j = 0; j < _interactableGrid[i].Length; j++){
                GameObject newCell = Instantiate(_cellPrefab, row);
                IUIInteractable uIInteractable = newCell.GetComponent<IUIInteractable>();
                if(uIInteractable != null) {
                    _interactableGrid[i][j] = uIInteractable;
                    uIInteractable.Initialize(i, j);
                    _interactableGrid[i][j].OnSelected += OnSelect;
                    _interactableGrid[i][j].OnHighlighted += OnViewCellHighlighted;
                }
            }
        }
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInputs();
	}

    private void ProcessInputs() {
        if (!Active) { return; }
        DirectionalInputs();
        SelectButtonPressed();
    }

    private void DirectionalInputs() {
        float _horizontal = _inverted ? -Input.GetAxisRaw(Vertical) : Input.GetAxisRaw(Horizontal);
        float _vertical = _inverted ? -Input.GetAxisRaw(Horizontal) : Input.GetAxisRaw(Vertical);

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

        int x = CurrentItemX + Mathf.RoundToInt(_horizontal);
        int y = CurrentItemY - Mathf.RoundToInt(_vertical);

        if (x < 0) {
            if (_inverted) { TryChangeViewGrid(IntVector3.Down); }
            else { TryChangeViewGrid(IntVector3.Left); }
            return;
        } else if (x >= _interactableGrid.Length) {
            if (_inverted) { TryChangeViewGrid(IntVector3.Up); }
            else { TryChangeViewGrid(IntVector3.Right); }
            return;
        }
        if(CurrentItemY >= _interactableGrid[x].Length) {
            y = _interactableGrid[x].Length - 1 - Mathf.RoundToInt(_vertical);
        }
        if (y < 0) {
            if (_inverted) { TryChangeViewGrid(IntVector3.Left); }
            else { TryChangeViewGrid(IntVector3.Up); }
            return;
        } else if (y >= _interactableGrid[x].Length) {
            if (_inverted) { TryChangeViewGrid(IntVector3.Right); }
            else { TryChangeViewGrid(IntVector3.Down); }
            return;
        }

        if(_interactableGrid[x][y] == null) {
            return;
        }

        UpdateHighlightedViewCell(x, y);
    }

    private void TryChangeViewGrid(IntVector3 dir) {
        UIViewGrid nextGrid;
        if (dir == IntVector3.Left) {
            nextGrid = _left;
        } else if (dir == IntVector3.Right) {
            nextGrid = _right;
        } else if (dir == IntVector3.Up) {
            nextGrid = _up;
        } else {
            nextGrid = _down;
        }
        if(nextGrid == null) {
            _parentPanel.OutOfBounds(dir);
            return;
        }
        // tell parent to change view grid
        UnhighlightCell(CurrentItemX, CurrentItemY);
        _parentPanel.UpdateActiveGrid(nextGrid);
    }

    private void OnViewCellHighlighted(IUIInteractable interactable) {
        UIViewCell viewCell = interactable as UIViewCell;
        if(viewCell == null) {
            return;
        }
        UpdateHighlightedViewCell(viewCell.XCoord, viewCell.YCoord);
    }

    public void UpdateHighlightedViewCell(int x, int y) {
        _interactableGrid[CurrentItemX][CurrentItemY].Unhighlight();
        _currentItemX = x;
        _currentItemY = y;
        _interactableGrid[CurrentItemX][CurrentItemY].Highlight();
    }

    public void UnhighlightCell(int x, int y) {
        _interactableGrid[CurrentItemX][CurrentItemY]?.Unhighlight();
    }

    public void SetCurrentAtBound(IntVector3 dir) {
        if (_inverted) {
            if (dir == IntVector3.Up) {
                dir = IntVector3.Right;
            } else if (dir == IntVector3.Right) {
                dir = IntVector3.Down;
            } else if (dir == IntVector3.Down) {
                dir = IntVector3.Left;
            } else if (dir == IntVector3.Left) {
                dir = IntVector3.Up;
            }
        }

        if (dir == IntVector3.Up) {
            _currentItemX = _interactableGrid.Length / 2;
            _currentItemY = _interactableGrid[CurrentItemX].Length - 1;
        } else if (dir == IntVector3.Right) {
            _currentItemX = 0;
            _currentItemY = _interactableGrid[CurrentItemX].Length / 2;
        } else if (dir == IntVector3.Down) {
            _currentItemX = _interactableGrid.Length / 2;
            _currentItemY = 0;
        } else if(dir == IntVector3.Left) {
            _currentItemX = _interactableGrid.Length - 1;
            _currentItemY = _interactableGrid[CurrentItemX].Length / 2;
        }
    }

    private void SelectButtonPressed() {
        if (Input.GetButtonDown("Submit")) {
            IUIInteractable selected = _interactableGrid[CurrentItemX][CurrentItemY];
            OnSelect(selected);
        }
    }

    private void OnSelect(IUIInteractable interactable) {
        OnSelectPressed?.Invoke(interactable);
    }

    public void SetInteractableItem(int x, int y, IUIInteractableData data) {
        data.X = x;
        data.Y = y;
        _interactableGrid[x][y].SetValue(data);
    }

    public void AddInteractableItemToRow(int x, IUIInteractableData data) {
        for(int i = 0; i < _interactableGrid[x].Length; i++) {
            if (_interactableGrid[x][i].Id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
                data.X = x;
                data.Y = i;
                _interactableGrid[x][i].SetValue(data);
                return;
            }
        }
        Debug.Log("Row full!");
    }

    public void AddInteractableItemToColumn(int y, IUIInteractableData data) {
        for(int i = 0; i < _interactableGrid.Length; i++) {
            if (_interactableGrid[i][y].Id.Equals(GameplayValues.UI.EmptyInventoryItemId)) {
                data.X = i;
                data.Y = y;
                _interactableGrid[i][y].SetValue(data);
                return;
            }
        }
        Debug.Log("Column Full!");
    }

    public void AddInteractableToGrid(IUIInteractableData data) {

    }

    public void AddInteractableAt(int x, int y, IUIInteractable data) {
        IUIInteractable slot = _interactableGrid[x][y];
    }

    public void ClearInteractableItem(int x, int y) {
        _interactableGrid[x][y].Initialize(x, y);
    }

    public void RemoveInteractableFromRow(int x, int y) {
        for(int i = y; i < _interactableGrid[x].Length; i++) {
            if (i + 1 >= _interactableGrid[x].Length) {
                _interactableGrid[x][i].Initialize(x, y);
                break;
            }
            _interactableGrid[x][i].SetValue(_interactableGrid[x][i + 1].ExtractData());
        }
    }

    private void OnActiveGridUpdated(int index) {
        
    }
}

public class UIViewGridInitData {
    public int[] RowLengths;
}
