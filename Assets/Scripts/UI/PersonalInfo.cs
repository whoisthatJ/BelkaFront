using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* Personal info edit controller.
 * Input fields and drop down is active when user editing his info.
 * User info properties are connected to main model properties.
 */

public class PersonalInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fullNameTxt;
    [SerializeField] TMP_InputField fullNameIF; // Input Field

    [SerializeField] TextMeshProUGUI ageTxt;
    [SerializeField] TMP_Dropdown ageDD; // Drop Down

    [SerializeField] TextMeshProUGUI cityTxt;
    [SerializeField] TMP_InputField cityIF; // Input Field

    [SerializeField] TextMeshProUGUI emailTxt;
    [SerializeField] TMP_InputField emailIF; // Input Field

    [SerializeField] Button button;

    private bool inEdit;

    private void OnEnable()
    {
        MainModel.OnFullNameChanged += RefreshFullName;
        MainModel.OnAgeChanged += RefreshAge;
        MainModel.OnCityChanged += RefreshCity;
        MainModel.OnEmailChanged += RefreshEmail;
    }

    private void OnDisable()
    {
        MainModel.OnFullNameChanged -= RefreshFullName;
        MainModel.OnAgeChanged -= RefreshAge;
        MainModel.OnCityChanged -= RefreshCity;
        MainModel.OnEmailChanged -= RefreshEmail;
    }

    private void Start()
    {
        RefreshFullName();
        RefreshAge();
        RefreshCity();
        RefreshEmail();
    }

    void IFSetActive(bool state)
    {
        fullNameIF.gameObject.SetActive(state);
        ageDD.gameObject.SetActive(state);
        cityIF.gameObject.SetActive(state);
        emailIF.gameObject.SetActive(state);
    }

    void SetIFValues()
    {
        fullNameIF.text = fullNameTxt.text.ToString();
        try
        {
            ageDD.value = int.Parse(ageTxt.text);
        }
        catch (FormatException)
        {
            ageDD.value = 0;
        }
        cityIF.text = cityTxt.text.ToString();
        emailIF.text = emailTxt.text.ToString();
    }

    // Display main model values in profile
    void RefreshFullName()
    {
        if (MainRoot.Instance.mainModel.FullName != null)
        {
            fullNameTxt.text = MainRoot.Instance.mainModel.FullName.ToString();
        }
        else
        {
            fullNameTxt.text = string.Empty;
        }
    }

    void RefreshAge()
    {
        if (MainRoot.Instance.mainModel.Age != 0)
        {
            ageTxt.text = MainRoot.Instance.mainModel.Age.ToString();
        }
        else
        {
            ageTxt.text = string.Empty;
        }
    }

    void RefreshCity()
    {
        if (MainRoot.Instance.mainModel.City != null)
        {
            cityTxt.text = MainRoot.Instance.mainModel.City.ToString();
        }
        else
        {
            cityTxt.text = string.Empty;
        }
    }

    void RefreshEmail()
    {
        if (MainRoot.Instance.mainModel.Email != null)
        {
            emailTxt.text = MainRoot.Instance.mainModel.Email.ToString();
        }
        else
        {
            emailTxt.text = string.Empty;
        }
    }

    // On edit button click mode changes
    // , so user can change fields.
    public void ButtonClicked()
    {
        if (!inEdit)
        {
            Edit();
            inEdit = true;
        }
        else
        {
            ConfirmChanges();
            inEdit = false;
        }
    }

    // Activate "input mode" for user to change personal info
    private void Edit()
    {
        IFSetActive(true);
        SetIFValues();
    }

    // Save changes, deactivate "input mode"
    private void ConfirmChanges()
    {
        bool fullNameChanged = false;
        bool ageChanged = false;
        bool cityChanged = false;
        bool emailChanged = false;

        MainModel mm = MainRoot.Instance.mainModel;

        // Input field is not empty
        if (!fullNameIF.text.Equals(string.Empty))
        {
            // User had FullName before
            if (mm.FullName != null)
            {
                // New FullName is not equal to old FullName
                if (!mm.FullName.Equals(fullNameIF.text))
                {
                    // Change FullName
                    mm.FullName = fullNameIF.text;
                    fullNameChanged = true;
                }
            }
            // User didn't have FullName before
            else
            {
                // Set FullName
                mm.FullName = fullNameIF.text;
                fullNameChanged = true;
            }
        }
        // Nothing is wrote in input field
        else
        {
            // User had FullName before
            if (mm.FullName != null)
            {
                // Remove user FullName
                mm.FullName = null;
                fullNameChanged = true;
            }
        }

        // Drop down is not 0
        if (ageDD.value != 0)
        {
            // User had Age before
            if (mm.Age != 0)
            {
                // New Age is not equal to old Age
                if (mm.Age != ageDD.value)
                {
                    // Change Age
                    mm.Age = ageDD.value;
                    ageChanged = true;
                }
            }
            // User didn't have Age before
            else
            {
                // Set Age
                mm.Age = ageDD.value;
                ageChanged = true;
            }
        }
        // Drop down is 0
        else
        {
            // User had Age before
            if (mm.Age != 0)
            {
                // Remove user Age
                mm.Age = 0;
                ageChanged = true;
            }
        }

        // Input field is not empty
        if (!cityIF.text.Equals(string.Empty))
        {
            // User had City before
            if (mm.City != null)
            {
                // New City is not equal to old City
                if (!mm.City.Equals(cityIF.text))
                {
                    // Change City
                    mm.City = cityIF.text;
                    cityChanged = true;
                }
            }
            // User didn't have City before
            else
            {
                // Set City
                mm.City = cityIF.text;
                cityChanged = true;
            }
        }
        // Nothing is wrote in input field
        else
        {
            // User had City before
            if (mm.City != null)
            {
                // Remove user City
                mm.City = null;
                cityChanged = true;
            }
        }

        // Input field is not empty
        if (!emailIF.text.Equals(string.Empty))
        {
            // User had Email before
            if (mm.Email != null)
            {
                // New Email is not equal to old Email
                if (!mm.Email.Equals(emailIF.text))
                {
                    // Change Email
                    mm.Email = emailIF.text;
                    emailChanged = true;
                }
            }
            // User didn't have Email before
            else
            {
                // Set Email
                mm.Email = emailIF.text;
                emailChanged = true;
            }
        }
        // Nothing is wrote in input field
        else
        {
            // User had Email before
            if (mm.Email != null)
            {
                // Remove user Email
                mm.Email = null;
                emailChanged = true;
            }
        }

        ServiceWeb.Instance.UpdateProfile(fullNameChanged, ageChanged, cityChanged, emailChanged);
        IFSetActive(false);
    }
}
