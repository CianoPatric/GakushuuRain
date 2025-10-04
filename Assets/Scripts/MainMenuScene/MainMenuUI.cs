using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("RegistrationWindow")]
    public TMP_InputField rEmailField;
    public TMP_InputField rPasswordField;
    public TMP_InputField passwordConfirmField;
    public TMP_InputField nicknameField;
    public TMP_Text rFeedbackText;

    public GameObject rWindow;
    public Button rButton;
    public Button rExitButton;
    
    [Header("AuthorizationWindow")]
    public TMP_InputField aEmailField;
    public TMP_InputField aPasswordField;
    public TMP_Text aFeedbackText;
    
    public GameObject aWindow;
    public Button aButton;
    public Button aExitButton;
    
    private PlayerData _authenticatedPlayerData;
    
    private AuthManager saveAuthManager;
    private DataManager saveDataManager;

    public void Initialize(AuthManager authManager, DataManager dataManager)
    {
        saveAuthManager = authManager;
        saveDataManager = dataManager;
    }

    public async void OnSignUpButtonClicked()
    {
        SetRUIInteractable(false);
        rFeedbackText.text = "";
        string email = rEmailField.text;
        string password = rPasswordField.text;
        string passwordConfirm = passwordConfirmField.text;
        string nickname = nicknameField.text;

        if (password == passwordConfirm)
        {
            var (success, message) = await saveAuthManager.SignUpAsync(email, password, nickname);
            aFeedbackText.text = message;
            if (success)
            {
                rWindow.SetActive(false);
                _authenticatedPlayerData = null;
            }

            SetRUIInteractable(true);
        }
        else
        {
            rFeedbackText.text = "Проверьте совпадение введёного пароля";
            SetRUIInteractable(true);
        }
    }

    public async void OnSignInButtonClicked()
    {
        SetAUIInteractable(false);
        aFeedbackText.text = "Вход...";
        string email = aEmailField.text;
        string password = aPasswordField.text;
        
        var (authSuccess, authMessage, offlineData) = await saveAuthManager.SignInAsync(email, password);
        aFeedbackText.text = authMessage;
        if (authSuccess)
        {
            aFeedbackText.text = "Загрузка данных...";
            PlayerData playerData;
            if (offlineData != null)
            {
                playerData = offlineData;
            }
            else
            {
                playerData = await saveDataManager.LoadData();
            }

            if (playerData != null)
            {
                _authenticatedPlayerData = playerData;
                aWindow.SetActive(false);
            }
            else
            {
                aFeedbackText.text = "Не удалось загрузить данные пользователя";
            }
        }

        SetAUIInteractable(true);
    }

    public void OnContinueButtonClicked()
    {
        MMRootBinder.HandleGoToGameButtonClick();
    }

    public async void OnNewSaveButtonClicked()
    {
        var newSave = await saveDataManager.NewSaveData();
        if (newSave != null)
        {
            _authenticatedPlayerData = newSave;
            MMRootBinder.HandleGoToGameButtonClick();
        }
        else
        {
            Debug.LogError("Не удалось создать новое сохранение");
        }
    }
    
    public PlayerData GetAuthenticatedPlayerData()
    {
        if (_authenticatedPlayerData == null)
        {
            aWindow.SetActive(true);
        }
        
        return _authenticatedPlayerData;
    }
    
    private void SetRUIInteractable(bool isInteractable)
    {
        rEmailField.interactable = isInteractable;
        rPasswordField.interactable = isInteractable;
        passwordConfirmField.interactable = isInteractable;
        nicknameField.interactable = isInteractable;
        rButton.interactable = isInteractable;
        rExitButton.interactable = isInteractable;
    }

    private void SetAUIInteractable(bool isInteractable)
    {
        aEmailField.interactable = isInteractable;
        aPasswordField.interactable = isInteractable;
        aButton.interactable = isInteractable;
        aExitButton.interactable = isInteractable;
    }
}