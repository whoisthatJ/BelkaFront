using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Dropdown serverDropdown;
    [SerializeField] private TMP_InputField idIF;
    [SerializeField] private TMP_InputField nameIF;
    [SerializeField] private Button loginBtn;

    private void OnEnable()
    {
        loginBtn.onClick.AddListener(LoginBtnClick);
    }

    private void OnDisable()
    {
        loginBtn.onClick.RemoveAllListeners();
    }

    public void SetPanelActive(bool isActive)
    {
        panel.SetActive(isActive);
    }

    private void LoginBtnClick()
    {
        ServiceWeb.Instance.RewriteLoginParams((LoginTypes)serverDropdown.value, idIF.text, nameIF.text);
        ServiceIO.Instance.ChangeServer((LoginTypes)serverDropdown.value);
        SetPanelActive(false);
        Preloader.Instance.FirstLoad();
    }
}
