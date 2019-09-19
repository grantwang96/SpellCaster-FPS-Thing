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
    public bool Active { get; private set; } // is this grid currently receiving inputs?
    public bool _hardLocked { get; private set; } // can the player leave this view grid
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

    [SerializeField] private bool _inverted;

    private bool _isBuilt = false;
    private float _horizontal;
    private float _vertical;

    public delegate void GridItemEvent(IUIInteractable interactable);
    public event GridItemEvent OnHighlighted;
    public event GridItemEvent OnSelectPressed;

    public virtual void Initialize(UIViewGridInitData viewGridInitData) {
        if(_parentPanel == null) {
            _parentPanel = _parentPanelGameObject.GetComponent<IUIViewGridParent>();
        }
        InitializeInteractableGrid(viewGridInitData.RowLengths);
    }
    
    private void InitializeInteractableGrid(int[] rows) {
        if (_isBuilt) {
            return;
        }
        _interactableGrid = new IUIInteractable[rows.Length][];
        for (int i = 0; i < rows.Length; i++) {
            RectTransform row = Instantiate(_rowPrefab, _content);
            row.gameObject.SetActive(true);
            _interactableGrid[i] = new IUIInteractable[rows[i]];
            for (int j = 0; j < rows[i]; j++) {
                IUIInteractable uIInteractable = GenerateViewCell(row);
                if (uIInteractable != null) {
                    _interactableGrid[i][j] = uIInteractable;
                    uIInteractable.Initialize(i, j);
                    _interactableGrid[i][j].OnMousePointerClick += OnSelect;
                    _interactableGrid[i][j].OnMousePointerHighlight += OnViewCellHighlighted;
                }
            }
        }
        _isBuilt = true;
    }

    private IUIInteractable GenerateViewCell(Transform row) {
        GameObject newCell = Instantiate(_cellPrefab, row);
        IUIInteractable uIInteractable = newCell.GetComponent<IUIInteractable>();
        if(uIInteractable != null) {
            return uIInteractable;
        }
        return null;
    }

    private void DirectionalInputs(Vector2 moveVector) {

        float _horizontal = _inverted ? -moveVector.y : moveVector.x;
        float _vertical = _inverted ? -moveVector.x : moveVector.y;

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
        if (_hardLocked) {
            return;
        }
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
        UpdateHighlightedViewCell(interactable.XCoord, interactable.YCoord);
    }

    public void UpdateHighlightedViewCell(int x, int y) {
        _interactableGrid[CurrentItemX][CurrentItemY].InteractableUnhighlight();
        _currentItemX = x;
        _currentItemY = y;
        _interactableGrid[CurrentItemX][CurrentItemY].InteractableHighlight();
        OnHighlighted?.Invoke(_interactableGrid[CurrentItemX][CurrentItemY]);
    }

    public void UnhighlightCell(int x, int y) {
        _interactableGrid[CurrentItemX][CurrentItemY]?.InteractableUnhighlight();
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
        IUIInteractable selected = _interactableGrid[CurrentItemX][CurrentItemY];
        Debug.Log("Select button pressed!");
        selected.InteractableSelect();
    }

    private void OnSelect(IUIInteractable interactable) {
        if (!Active) {
            return;
        }
        Debug.Log("Select interactable!");
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
            IUIInteractableData data = _interactableGrid[x][i + 1].ExtractData();
            data.X = x;
            data.Y = y;
            _interactableGrid[x][i].SetValue(data);
        }
    }

    public IUIInteractable GetInteractableAt(int x, int y) {
        if(x < 0 || x > _interactableGrid.Length || y < 0 || y > _interactableGrid[x].Length) {
            Debug.LogError($"[{nameof(UIViewGrid)}] Index {x}, {y} is out of bounds!");
            return null;
        }
        return _interactableGrid[x][y];
    }

    public void SetActive(bool active, bool hardLock = false) {
        Active = active;
        // set whether player can leave this view grid
        _hardLocked = hardLock;

        UnsubscribeToController();
        if (!Active) {
            UnhighlightCell(CurrentItemX, CurrentItemY);
        } else {
            _interactableGrid[CurrentItemX][CurrentItemY].InteractableHighlight();
            SubscribeToController();
        }
    }

    private void SubscribeToController() {
        GameplayController.Instance.DirectionalInput += DirectionalInputs;
        GameplayController.Instance.OnSubmitPressed += SelectButtonPressed;
    }

    private void UnsubscribeToController() {
        GameplayController.Instance.DirectionalInput -= DirectionalInputs;
        GameplayController.Instance.OnSubmitPressed -= SelectButtonPressed;
    }
}

public class UIViewGridInitData {
    public int[] RowLengths;
}
