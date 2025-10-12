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
    private AuthManager _authManager;
    private DataManager _dataManager;
    
    [Header("Main")]
    [SerializeField] private Button continueButton;

    [SerializeField] private Button newSaveButton;

    public void Initialize(AuthManager authManager, DataManager dataManager)
    {
        _authManager = authManager;
        _dataManager = dataManager;
        SetupInitialButtons();
    }

    private void SetupInitialButtons()
    {
        bool localSaveExists = LocalSaveManager.LoadProfile() != null;
        continueButton.gameObject.SetActive(localSaveExists);
        newSaveButton.gameObject.SetActive(!localSaveExists);
    }

    public async void OnSignUpButtonClicked()
    {
        SetRuiInteractable(false);
        rFeedbackText.text = "";
        string email = rEmailField.text;
        string password = rPasswordField.text;
        string passwordConfirm = passwordConfirmField.text;
        string nickname = nicknameField.text;

        if (password == passwordConfirm)
        {
            var (success, message) = await _authManager.SignUpAsync(email, password, nickname);
            aFeedbackText.text = message;
            if (success)
            {
                rWindow.SetActive(false);
                _authenticatedPlayerData = null;
            }

            SetRuiInteractable(true);
        }
        else
        {
            rFeedbackText.text = "Проверьте совпадение введёного пароля";
            SetRuiInteractable(true);
        }
    }

    public async void OnSignInButtonClicked()
    {
        SetAuiInteractable(false);
        aFeedbackText.text = "Вход...";
        string email = aEmailField.text;
        string password = aPasswordField.text;
        
        var (authSuccess, authMessage) = await _authManager.SignInAsync(email, password);
        aFeedbackText.text = authMessage;
        if (authSuccess)
        {
            aFeedbackText.text = "Загрузка данных...";
            _authenticatedPlayerData = await _dataManager.LoadData();
            if (_authenticatedPlayerData != null)
            {
                MMRootBinder.HandleGoToGameButtonClick();
            }
            else
            {
                aFeedbackText.text = "Не удалось загрузить данные пользователя";
                SetAuiInteractable(true);
            }
        }
        else
        {
            SetAuiInteractable(true);
        }

        SetAuiInteractable(true);
    }

    public async void OnContinueButtonClicked()
    {
        if (!_authManager.IsUserLoggedIn())
        {
            aWindow.SetActive(true);
            return;
        }
        
        _authenticatedPlayerData = await _dataManager.LoadData();
        if (_authenticatedPlayerData != null)
        {
            MMRootBinder.HandleGoToGameButtonClick();
        }
        else
        {
            Debug.LogError("Не удалось загрузить сохранение");
            continueButton.gameObject.SetActive(false);
            newSaveButton.gameObject.SetActive(true);
        }
    }

    public async void OnNewSaveButtonClicked()
    {
        if (!_authManager.IsUserLoggedIn())
        {
            aWindow.SetActive(true);
            return;
        }
        
        _authenticatedPlayerData = await _dataManager.LoadData();
        if (_authenticatedPlayerData != null)
        {
            MMRootBinder.HandleGoToGameButtonClick();
        }
        else
        {
            Debug.LogError("Не удалось создать новое сохранение");
        }
    }
    
    public PlayerData GetAuthenticatedPlayerData()
    {
        return _authenticatedPlayerData;
    }
    
    private void SetRuiInteractable(bool isInteractable)
    {
        rEmailField.interactable = isInteractable;
        rPasswordField.interactable = isInteractable;
        passwordConfirmField.interactable = isInteractable;
        nicknameField.interactable = isInteractable;
        rButton.interactable = isInteractable;
        rExitButton.interactable = isInteractable;
    }

    private void SetAuiInteractable(bool isInteractable)
    {
        aEmailField.interactable = isInteractable;
        aPasswordField.interactable = isInteractable;
        aButton.interactable = isInteractable;
        aExitButton.interactable = isInteractable;
    }
}