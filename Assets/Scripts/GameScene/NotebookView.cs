using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookView : MonoBehaviour
{
    [Header("Интерфейс блокнота")]
    [SerializeField] private GameObject notebookCanvas;
    [SerializeField] private GameObject wordPanel;
    [SerializeField] private GameObject wordButtonPrefab;
    
    [Header("Элементы страницы слова")]
    [SerializeField] private TextMeshProUGUI wordTitleText;
    [SerializeField] private TextMeshProUGUI translationText;
    [SerializeField] private TextMeshProUGUI definitionText;
    [SerializeField] private TMP_InputField userGuessInput;
    
    [Header("Вкладки блокнота")]
    [SerializeField] private GameObject peopleTabContent;
    [SerializeField] private GameObject objectsTabContent;
    [SerializeField] private GameObject locationsTabContent;
    [SerializeField] private GameObject actionsTabContent;
    
    private DataManager _dataManager;
    private WordLibraryManager _wordLibraryManager;
    
    private NotebookEntry _currentSelectedEntry;

    public void Initialize(DataManager dataManager, WordLibraryManager wordLibraryManager)
    {
        _dataManager = dataManager;
        _wordLibraryManager = wordLibraryManager;
        
        notebookCanvas.SetActive(false);
        wordPanel.SetActive(false);
    }
    
    public void PopulateFromSave()
    {
        Debug.Log("NotebookManager: Инициализация и заполнение из сохранения");
        ClearAllTabs();
        PlayerData playerData = _dataManager.GetCurrentPlayerData();
        if (playerData?.notebookEntries == null) return;
        
        Debug.Log($"NotebookManager: Найдено {playerData.notebookEntries.Count} слов в сохранении");
        foreach (var entry in playerData.notebookEntries)
        {
            CreateButtonForEntry(entry);
        }
    }
    
    void OnEnable()
    {
        DataManager.OnWordAddedToNotebook += HandleWordAdded;
    }

    void OnDisable()
    {
        DataManager.OnWordAddedToNotebook -= HandleWordAdded;
    }
    
    public void OpenNotebook()
    {
        notebookCanvas.SetActive(true);
        SwitchToTab("people");
    }
    
    public void CloseNotebook()
    {
        SaveChangesForCurrentEntry();
        notebookCanvas.SetActive(false);
        CloseWordPanel();
    }
    
    public void OpenAndShowWord(string wordId)
    {
        WordData wordData = _wordLibraryManager.GetWordData(wordId);
        if (wordData == null)
        {
            Debug.LogError("Такого слова нет в блокноте");
            return;
        }

        if (!notebookCanvas.activeSelf)
        {
            OpenNotebook();
        }
        SwitchToTab(wordData.category);
        PlayerData playerData = _dataManager.GetCurrentPlayerData();
        NotebookEntry entryToShow = playerData.notebookEntries.Find(e => e.wordId == wordId);

        if (entryToShow != null)
        {
            SelectWord(entryToShow);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleWordAdded(NotebookEntry newEntry)
    {
        CreateButtonForEntry(newEntry);
    }
    
    private void CreateButtonForEntry(NotebookEntry newEntry)
    {
        WordData wordData = _wordLibraryManager.GetWordData(newEntry.wordId);
        if(wordData == null) return;
        
        Transform parentTab = GetParentTabForCategory(wordData.category);
        if (parentTab != null)
        {
            GameObject newButton = Instantiate(wordButtonPrefab, parentTab);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = wordData.text;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectWord(newEntry));
        }
    }
    
    private void SelectWord(NotebookEntry entry)
    {
        SaveChangesForCurrentEntry();
        _currentSelectedEntry = entry;
        WordData wordData = _wordLibraryManager.GetWordData(entry.wordId);
        wordTitleText.text = wordData.text;
        translationText.text = wordData.translation;
        definitionText.text = wordData.definition;
        userGuessInput.text = entry.userGuess;
        wordPanel.SetActive(true);
    }
    
    public void CloseWordPanel()
    {
        SaveChangesForCurrentEntry();
        _currentSelectedEntry = null;
        wordPanel.SetActive(false);
    }
    
    private void SaveChangesForCurrentEntry()
    {
        if (_currentSelectedEntry != null && _currentSelectedEntry.userGuess != userGuessInput.text)
        {
            _currentSelectedEntry.userGuess = userGuessInput.text;
            _dataManager.MarkDataAsDirty();
            Debug.Log($"Изменения для слова '{_currentSelectedEntry.wordId}' сохранены");
        }
    }
    
    public void SwitchToTab(string category)
    {
        peopleTabContent.transform.parent.parent.gameObject.SetActive(category == "people");
        objectsTabContent.transform.parent.parent.gameObject.SetActive(category == "objects");
        locationsTabContent.transform.parent.parent.gameObject.SetActive(category == "locations");
        actionsTabContent.transform.parent.parent.gameObject.SetActive(category == "actions");
    }

    private void ClearAllTabs()
    {
        foreach (Transform child in peopleTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in objectsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in locationsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in actionsTabContent.transform) {Destroy(child.gameObject);}
    }
    
    private Transform GetParentTabForCategory(string category)
    {
        switch (category)
        {
            case "people": return peopleTabContent.transform;
            case "objects": return objectsTabContent.transform;
            case "locations": return locationsTabContent.transform;
            case "actions": return actionsTabContent.transform;
            default: return null;
        }
    }
}