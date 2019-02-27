using UnityEngine;
using UnityEngine.UI;

public class UISpellComponentSlot : UIViewCell {

    [SerializeField] private Text _text;
    [SerializeField] private Image _image;
    [SerializeField] private Image _imagePrefab;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public override void Initialize(ViewCellInitData initData) {
        SpellComponentInitData componentData = initData as SpellComponentInitData;
        if(componentData == null) {
            Debug.LogError("Did not receive SpellComponentInitData!");
            return;
        }
        if(componentData.itemId == GameplayValues.EmptyInventoryItemId) {
            _text.text = GameplayValues.EmptySpellStageText;
            name = GameplayValues.EmptyUIElementId;
            return;
        }
        _text.text = componentData.Text;
        name = componentData.Name;
        if(_image == null) {
            _image = Instantiate(_imagePrefab, componentData.ImageParent);
        }
        IInventoryStorable storable = InventoryRegistry.Instance.GetItemById(initData.itemId);
        _image.sprite = storable.Sprite; // temp
    }

    public override void Highlight() {
        _text.rectTransform.localScale = Vector3.one * 1.25f;
    }

    public override void Unhighlight() {
        _text.rectTransform.localScale = Vector3.one;
    }

    private void OnDestroy() {
        if(_image == null) {
            return;
        }
        Destroy(_image.gameObject);
    }
}

public class SpellComponentInitData : ViewCellInitData{
    public string Text;
    public RectTransform ImageParent;
}
