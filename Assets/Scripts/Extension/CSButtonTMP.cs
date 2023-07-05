using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CSButtonTMP : Button
{
    [SerializeField] private TextMeshProUGUI _text;

    public TextMeshProUGUI Text
    {
        get => _text;

        set => _text = value;
    }
    
    [ExecuteInEditMode]
    private void Awake()
    {
        _text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
