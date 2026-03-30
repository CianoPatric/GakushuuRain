using System.Threading.Tasks;
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
    
    [Header("SaveSlotsWindow")]
    [SerializeField] private GameObject saveSlotsWindow;
    [SerializeField] private SaveSlotUI[] saveSlotUIs;

    public void Initialize(AuthManager authManager, DataManager dataManager)
    {
        _authManager = authManager;
        _dataManager = dataManager;
    }

    public async void OnStartButtonClicked()
    {
        if (!_authManager.IsUserLoggedIn())
        {
            aWindow.SetActive(true);
            return;
        }

        await ShowSaveSlotsWindow();
    }

    private async Task ShowSaveSlotsWindow()
    {
        var allSlotsData = await _dataManager.GetAllSaveSlots();

        for (int i = 0; i < saveSlotUIs.Length; i++)
        {
            if (allSlotsData.TryGetValue(i, out PlayerData data))
            {
                saveSlotUIs[i].Display(i, data);
            }
            else
            {
                saveSlotUIs[i].DisplayEmpty(i);
            }
            int slotIndex = i;
            saveSlotUIs[i].OnSelect.RemoveAllListeners();
            saveSlotUIs[i].OnSelect.AddListener(() => OnSlotSelected(slotIndex));
            saveSlotUIs[i].OnDelete.RemoveAllListeners();
            saveSlotUIs[i].OnDelete.AddListener(() => OnSlotDeleted(slotIndex));
        }
        saveSlotsWindow.SetActive(true);
    }

    private async void OnSlotSelected(int slotIndex)
    {
        saveSlotsWindow.SetActive(false);
        _authenticatedPlayerData = await _dataManager.LoadData(slotIndex);
        if (_authenticatedPlayerData != null)
        {
            MMRootBinder.HandleGoToGameButtonClick();
        }
        else
        {
            Debug.LogError($"Не удалось загрузить или создать данные для слота {slotIndex}");
            saveSlotsWindow.SetActive(true);
        }
    }

    private async void OnSlotDeleted(int slotIndex)
    {
        await _dataManager.DeleteSaveSlot(slotIndex);
        await ShowSaveSlotsWindow();
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
            aWindow.SetActive(false);
            await ShowSaveSlotsWindow();
        }
        else
        {
            SetAuiInteractable(true);
        }

        SetAuiInteractable(true);
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