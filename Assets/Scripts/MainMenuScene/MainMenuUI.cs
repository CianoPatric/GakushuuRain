using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("RegistrationWindow")]
    public TMP_InputField RemailField;
    public TMP_InputField RpasswordField;
    public TMP_InputField passwordConfirmField;
    public TMP_InputField nicknameField;
    public TMP_Text RfeedbackText;

    public GameObject RWindow;
    public Button Rbutton;
    public Button RexitButton;
    
    [Header("AuthorizationWindow")]
    public TMP_InputField AemailField;
    public TMP_InputField ApasswordField;
    public TMP_Text AfeedbackText;
    
    public GameObject AWindow;
    public Button Abutton;
    public Button AexitButton;
    
    private PlayerData _authenticatedPlayerData;

    public async void OnSignUpButtonClicked()
    {
        SetRUIInteractable(false);
        RfeedbackText.text = "";
        string email = RemailField.text;
        string password = RpasswordField.text;
        string passwordConfirm = passwordConfirmField.text;
        string nickname = nicknameField.text;

        if (password == passwordConfirm)
        {
            var (success, message) = await AuthManager.Instance.SignUpAsync(email, password, nickname);
            RfeedbackText.text = message;
            if (success)
            {
                _authenticatedPlayerData = null;
            }

            SetRUIInteractable(true);
        }
        else
        {
            RfeedbackText.text = "Проверьте совпадение введёного пароля";
            SetRUIInteractable(true);
        }
    }

    public async void OnSignInButtonClicked()
    {
        SetAUIInteractable(false);
        AfeedbackText.text = "";
        string email = AemailField.text;
        string password = ApasswordField.text;
        
        var (success, message, data) = await AuthManager.Instance.SignInAsync(email, password);
        AfeedbackText.text = message;
        if (success)
        {
            _authenticatedPlayerData = data;
            AWindow.SetActive(false);
        }

        SetAUIInteractable(true);
    }

    public PlayerData GetAuthenticatedPlayerData()
    {
        if (_authenticatedPlayerData == null)
        {
            AWindow.SetActive(true);
        }
        
        return _authenticatedPlayerData;
    }
    
    private void SetRUIInteractable(bool isInteractable)
    {
        RemailField.interactable = isInteractable;
        RpasswordField.interactable = isInteractable;
        passwordConfirmField.interactable = isInteractable;
        nicknameField.interactable = isInteractable;
        Rbutton.interactable = isInteractable;
        RexitButton.interactable = isInteractable;
    }

    private void SetAUIInteractable(bool isInteractable)
    {
        AemailField.interactable = isInteractable;
        ApasswordField.interactable = isInteractable;
        Abutton.interactable = isInteractable;
        AexitButton.interactable = isInteractable;
    }
}