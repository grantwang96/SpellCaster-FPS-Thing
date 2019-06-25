using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISpellComponentSlot : UIViewCell, IPointerClickHandler, IPointerEnterHandler {
    
    public string Text { get; protected set; }

    [SerializeField] private Text _text;
    [SerializeField] private Image _image;
    [SerializeField] private Image _imagePrefab;

    private RectTransform _imageParent;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public override void Initialize(int x, int y) {
        SpellComponentData componentData = new SpellComponentData(x, y) {
            Id = GameplayValues.UI.EmptyInventoryItemId,
            Name = GameplayValues.UI.EmptyUIElementId,
            Text = GameplayValues.UI.EmptySpellStageText,
        };
        SetValue(componentData);
    }

    public override void SetValue(IUIInteractableData initData) {
        SpellComponentData componentData = initData as SpellComponentData;
        if (componentData == null) {
            componentData = new SpellComponentData(initData.X, initData.Y) {
                Id = GameplayValues.UI.EmptyInventoryItemId,
                Name = GameplayValues.UI.EmptyUIElementId,
                Text = GameplayValues.UI.EmptySpellStageText,
            };
        }
        xCoord = initData.X;
        yCoord = initData.Y;
        _id = componentData.Id;
        if (componentData.Id == GameplayValues.UI.EmptyInventoryItemId) {
            _text.text = GameplayValues.UI.EmptySpellStageText;
            name = GameplayValues.UI.EmptyUIElementId;
            if (_image != null) {
                Destroy(_image.gameObject);
            }
            return;
        }
        _text.text = componentData.Text;
        name = componentData.Name;
        _imageParent = componentData.ImageParent;
        if (_image == null) {
            _image = Instantiate(_imagePrefab, componentData.ImageParent);
        }
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(componentData.Id);
        _image.sprite = storable.SmallIcon; // temp
    }

    public override void Highlight() {
        _text.rectTransform.localScale = Vector3.one * 1.25f;
    }

    public override void Unhighlight() {
        _text.rectTransform.localScale = Vector3.one;
    }

    public override IUIInteractableData ExtractData() {
        SpellComponentData data = new SpellComponentData(xCoord, yCoord);
        data.Id = _id;
        data.ImageParent = _imageParent;
        data.Text = _text.text;
        data.Name = name;
        return data;
    }

    private void OnDestroy() {
        if(_image == null) {
            return;
        }
        Destroy(_image.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        PointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnter();
    }
}

public class SpellComponentData : ViewCellData {
    public string Text;
    public RectTransform ImageParent;

    public SpellComponentData(int x, int y) : base(x, y) {

    }
}
