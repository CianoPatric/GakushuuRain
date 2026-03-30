using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public GameObject notebookCanvas;
    public GameObject wordPanel;
    public GameObject wordButtonPrefab;

    [FormerlySerializedAs("NullItemSprite")] public Sprite nullItemSprite;

    [Header("Интерфейс кастомизации")]
    public GameObject customizationPanel;

    public GameObject itemScrollView;
    public Transform itemsContainer;
    public GameObject itemButtonPrefab;
    
    [Header("Элементы слов")]
    public TextMeshProUGUI wordTitleText;
    public TextMeshProUGUI translationText;
    public TextMeshProUGUI definitionText;
    public TMP_InputField userGuessInput;

    [Header("Вкладки")]
    public GameObject peopleTabContent;
    public GameObject objectsTabContent;
    public GameObject locationsTabContent;
    public GameObject actionsTabContent;

    private string _currentCategory;
    private NotebookEntry _currentSelectedEntry;
    
    private Dictionary<string, WordData> _wordLibrary;

    private DataManager _dataManager;
    private WordLibraryManager _wordLibraryManager;
    private PlayerMovement _player;
    public void Initialize(DataManager dataManager, WordLibraryManager wordLibraryManager, PlayerMovement player)
    {
        _dataManager = dataManager;
        _wordLibraryManager = wordLibraryManager;
        _player = player;
        notebookCanvas.SetActive(false);
        wordPanel.SetActive(false);
    }

    public void PopulateFromSave()
    {
        Debug.Log("NotebookManager: Инициализация и заполнение из сохранения");
        ClearAllTabs();
        PlayerData playerData = _dataManager.GetCurrentPlayerData();
        if (playerData == null || playerData.notebookEntries == null)
        {
            Debug.Log("NotebookManager: Данные игрока или список слов пусты при инициализации");
            return;
        }
        
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

    public void OpenCustomizationPanel()
    {
        customizationPanel.SetActive(true);
        itemScrollView.SetActive(false);
    }

    public void CloseCustomizationPanel()
    {
        customizationPanel.SetActive(false);
        itemScrollView.SetActive(false);
    }

    public void OpenNotebook()
    {
        notebookCanvas.SetActive(true);
        SwitchToTab("people");
    }

    public void ClearAllTabs()
    {
        foreach (Transform child in peopleTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in objectsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in locationsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in actionsTabContent.transform) {Destroy(child.gameObject);}
    }
    public void CloseNotebook()
    {
        SaveChangesForCurrentEntry();
        notebookCanvas.SetActive(false);
        CloseWordPanel();
        CloseCustomizationPanel();
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

    private void SaveChangesForCurrentEntry()
    {
        if (_currentSelectedEntry != null)
        {
            if (_currentSelectedEntry.userGuess != userGuessInput.text)
            {
                _currentSelectedEntry.userGuess = userGuessInput.text;
                _dataManager.MarkDataAsDirty();
                Debug.Log($"Изменения для слова '{_currentSelectedEntry.wordId}' сохранены");
            }
        }
    }

    public void SwitchToTab(string category)
    {
        peopleTabContent.transform.parent.parent.gameObject.SetActive(category == "people");
        objectsTabContent.transform.parent.parent.gameObject.SetActive(category == "objects");
        locationsTabContent.transform.parent.parent.gameObject.SetActive(category == "locations");
        actionsTabContent.transform.parent.parent.gameObject.SetActive(category == "actions");
    }

    public void CloseWordPanel()
    {
        SaveChangesForCurrentEntry();
        _currentSelectedEntry = null;
        wordPanel.SetActive(false);
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
}