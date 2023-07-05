using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour{
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private List<Containers> _containers;
    [SerializeField] private TextMeshProUGUI _countItems;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private QuestionItem _prefab;
    [SerializeField] private Image _preview;
    
    private List<QuestionItem> _items;
    public void Open(string title, Dictionary<int, int> rarities, int countItems){
        gameObject.SetActive(true);
        var mm = MainRoot.Instance.mainModel;
        
        _items = new List<QuestionItem>();
        _countItems.text = $"При открытии, выпадет {countItems.ToString()} случайных предмета";
        _title.text = title;
        
        foreach (var kvp in rarities){
            var obj = _containers.Find(x => x.Number == kvp.Key);
            obj.Container.gameObject.SetActive(true);
            obj.Percentage.text = kvp.Value.ToString() + "%";
            obj.Title.gameObject.SetActive(true);

            
            var shopItems = mm.ShopItems;
            var itemCategory = shopItems.Where(x => x.RarityCategory == kvp.Key).ToList();

            if (itemCategory.Count < 3)
                obj.Container.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
            else obj.Container.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            
            foreach (var item in itemCategory){
                QuestionItem q = Instantiate(_prefab, obj.Container);
                q.SetPreview(item.Picture);
            }
        }
    }

    public void Close(){
        foreach (var item in _containers){
            foreach (Transform tr in item.Container){
                if (tr.name != "Percentage"){
                    Destroy(tr.gameObject);
                }
            }
            item.Container.gameObject.SetActive(false);
            item.Title.gameObject.SetActive(false);
        }
        _items.Clear();
        gameObject.SetActive(false);
    }

    public void SetPreview(Sprite sp){
        _preview.sprite = sp;
        
        float aspectRatio = (float) _preview.sprite.texture.width / (float) _preview.sprite.texture.height;
        _preview.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        //Sprite size bug fixed
        var sprite = _preview.sprite;
        _preview.sprite = null;
        _preview.sprite = sprite;
    }
    
    [System.Serializable]
    class Containers{
        public int Number;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Percentage;
        public Transform Container;
    }
}
