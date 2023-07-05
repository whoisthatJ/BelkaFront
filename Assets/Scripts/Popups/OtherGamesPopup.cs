using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherGamesPopup : MonoBehaviour
{
	[Header("---UI---")]
	[SerializeField] private Button backBtn;
    
	[Space(10)]
	[SerializeField] private GameObject container;
	// Use this for initialization
	private void Start()
	{
		
	}

	private void OnEnable()
    {   
		InitListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void InitListeners()
	{
        Preloader.BackButtonPressed += BackBtnClick;
		backBtn.onClick.AddListener(BackBtnClick);
	}

	private void RemoveListeners()
    {
        Preloader.BackButtonPressed -= BackBtnClick;
        backBtn.onClick.RemoveAllListeners();
    }

    private void BackBtnClick()
	{
		container.SetActive(false);
        Preloader.Instance.RemovePanelFromTheList(container);
    }

    public void OpenPanel()
	{
		container.SetActive(true);
        Preloader.Instance.AddPanelToTheList(container);
    }

	//This method tunes in inscpector in buttons
	public void OpenOtherGamesDirect(string path)
    {
		Application.OpenURL(path);
    }
}
