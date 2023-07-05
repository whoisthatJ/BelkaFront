using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class NotificationsMenu : MonoBehaviour
{
    public static NotificationsMenu Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private GameObject inboxPanel;
    [SerializeField] private GameObject presentsPanel;
    [SerializeField] private GameObject archivePanel;
    [SerializeField] private Button inboxBtn;
    [SerializeField] private Button presentsBtn;
    [SerializeField] private Button archiveBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button clearArchiveBtn;
    [SerializeField] private GameObject noNotificationsMsg;


    private List<GameObject> inboxList;
    private List<GameObject> archiveList;
    private List<TextMeshProUGUI> inboxTextsList;
    private List<TextMeshProUGUI> archiveTextsList;


    private void Awake()
    {
        Instance = this;
    }
    
    private void OnEnable()
    {
        searchInputField.onValueChanged.AddListener(Search);
        searchInputField.onEndEdit.AddListener(EndSearch);
        inboxBtn.onClick.AddListener(InboxButtonPressed);
        presentsBtn.onClick.AddListener(PresentsButtonPressed);
        archiveBtn.onClick.AddListener(ArchiveButtonPressed);
        backBtn.onClick.AddListener(Close);
        clearArchiveBtn.onClick.AddListener(ClearArchiveButtonPressed);
    }

    private void OnDisable()
    {
        searchInputField.onValueChanged.RemoveAllListeners();
        searchInputField.onEndEdit.RemoveAllListeners();
        inboxBtn.onClick.RemoveAllListeners();
        presentsBtn.onClick.RemoveAllListeners();
        archiveBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        clearArchiveBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        inboxList = new List<GameObject>();
        archiveList = new List<GameObject>();
        inboxTextsList = new List<TextMeshProUGUI>();
        archiveTextsList = new List<TextMeshProUGUI>();
        
        //temp
        foreach (Transform t in inboxPanel.transform.GetChild(0).GetChild(0))
        {
            inboxList.Add(t.gameObject);
            foreach (TextMeshProUGUI tm in t.GetComponentsInChildren<TextMeshProUGUI>(false))
            {
                inboxTextsList.Add(tm);
            }
        }
        foreach (Transform t in archivePanel.transform.GetChild(0).GetChild(0))
        {
            archiveList.Add(t.gameObject);
            foreach (TextMeshProUGUI tm in t.GetComponentsInChildren<TextMeshProUGUI>(false))
            {
                archiveTextsList.Add(tm);
            }
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        InboxButtonPressed();
    }

    public void Close()
    {
        panel.SetActive(false);
        searchInputField.text = string.Empty;
    }

    public void Search(string searchValue)
    {
        if (inboxPanel.activeSelf)
        {
            foreach (GameObject g in inboxList)
            {
                g.SetActive(false);
            }
            var searchTexts = inboxTextsList.Where(t => t.text.ToLower().Contains(searchValue.ToLower()));
            foreach (TextMeshProUGUI t in searchTexts)
            {
                t.transform.parent.gameObject.SetActive(true);
                t.transform.parent.parent.gameObject.SetActive(true);
            }
        }
        else if (archivePanel.activeSelf)
        {
            foreach (GameObject g in archiveList)
            {
                g.SetActive(false);
            }
            var searchTexts = archiveTextsList.Where(t => t.text.ToLower().Contains(searchValue.ToLower()));
            foreach (TextMeshProUGUI t in searchTexts)
            {
                t.transform.parent.gameObject.SetActive(true);
                t.transform.parent.parent.gameObject.SetActive(true);
            }
        }
    }

    public void EndSearch(string searchValue)
    {
        Search(searchValue);
    }

    private void InboxButtonPressed()
    {
        if (!inboxPanel.activeSelf)
        {
            searchInputField.text = string.Empty;
        }
        inboxPanel.SetActive(true);
        presentsPanel.SetActive(false);
        archivePanel.SetActive(false);
        clearArchiveBtn.gameObject.SetActive(false);
        inboxBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        inboxBtn.transform.GetChild(1).gameObject.SetActive(true);
        presentsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        presentsBtn.transform.GetChild(1).gameObject.SetActive(false);
        archiveBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        archiveBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void PresentsButtonPressed()
    {
        inboxPanel.SetActive(false);
        presentsPanel.SetActive(true);
        archivePanel.SetActive(false);
        inboxBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255); ;
        inboxBtn.transform.GetChild(1).gameObject.SetActive(false);
        presentsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        presentsBtn.transform.GetChild(1).gameObject.SetActive(true);
        archiveBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        archiveBtn.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void ArchiveButtonPressed()
    {
        if (!archivePanel.activeSelf)
        {
            searchInputField.text = string.Empty;
        }
        inboxPanel.SetActive(false);
        presentsPanel.SetActive(false);
        archivePanel.SetActive(true);
        clearArchiveBtn.gameObject.SetActive(true);
        inboxBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        inboxBtn.transform.GetChild(1).gameObject.SetActive(false);
        presentsBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(170, 181, 199, 255);
        presentsBtn.transform.GetChild(1).gameObject.SetActive(false);
        archiveBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(78, 83, 109, 255);
        archiveBtn.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void ClearArchiveButtonPressed()
    {
        foreach (GameObject g in archiveList)
        {
            Destroy(g);            
        }
        archiveList = new List<GameObject>();
        searchInputField.text = string.Empty;
    }
}
