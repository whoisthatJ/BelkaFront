using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SwitchSpriteSelectedButtons : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;
    [SerializeField] private List<Image> _images;
    [SerializeField] private bool _isDefaultSelected;
    [SerializeField] private bool _isColorUse;
    [SerializeField] private Color _selected;
    [SerializeField] private Color _unselected;
    
    private Image _currentImage;
    
    private void Start()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            Image image = _images[i];
            _buttons[i].onClick.AddListener(()=> Select(image));
        }

        if (_isDefaultSelected)
        {
            _images[0].gameObject.SetActive(true);
            _currentImage = _images[0];
            if (_isColorUse){
                _buttons[0].transform.Find("Text").GetComponent<TextMeshProUGUI>().color = _selected;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void Select(Image image)
    {
        foreach (var im in _images)
        {
            im.gameObject.SetActive(false);
            if (_isColorUse)
            im.transform.parent.Find("Text").GetComponent<TextMeshProUGUI>().color = _unselected;
        }
        image.gameObject.SetActive(true);
        _currentImage = image;
        if (_isColorUse) image.transform.parent.Find("Text").GetComponent<TextMeshProUGUI>().color = _selected;
    }

    public Image GetCurrentImage()
    {
        return _currentImage;
    }

    public Image GetImage(Button btn)
    {
        int index = _buttons.FindIndex(x => x.name == btn.name);
        if (index != -1) return _images[index];
        return null;
    }
}
