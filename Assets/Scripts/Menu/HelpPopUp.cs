using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HelpPopUp : MonoBehaviour
{
    public static HelpPopUp Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject scrollParent;
    [SerializeField] private TMP_InputField searchInputField;

    private List<GameObject> questionsList;
    private List<TextMeshProUGUI> questionTextsList;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(Close);
        searchInputField.onValueChanged.AddListener(Search);
        searchInputField.onEndEdit.AddListener(Search);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
        searchInputField.onValueChanged.RemoveAllListeners();
        searchInputField.onEndEdit.RemoveAllListeners();
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        questionsList = new List<GameObject>();
        questionTextsList = new List<TextMeshProUGUI>();

        foreach (Transform t in scrollParent.transform)
        {
            questionsList.Add(t.gameObject);
            foreach (TextMeshProUGUI tm in t.GetComponentsInChildren<TextMeshProUGUI>(false))
            {
                questionTextsList.Add(tm);
            }
        }        
    }
    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        searchInputField.text = string.Empty;
    }

    private void Search(string searchValue)
    {
        
            foreach (GameObject g in questionsList)
            {
                g.SetActive(false);
            }
            var searchTexts = questionTextsList.Where(t => t.text.ToLower().Contains(searchValue.ToLower()));
            foreach (TextMeshProUGUI t in searchTexts)
            {
                t.transform.parent.gameObject.SetActive(true);
                t.transform.parent.parent.gameObject.SetActive(true);
            }        
    }
}
