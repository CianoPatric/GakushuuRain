using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager instance;
    
    public GameObject notebookCanvas;
    public GameObject wordButtonPrefab;

    public TextMeshProUGUI wordTitleText;
    public TMP_InputField userGuessInput;

    public GameObject peopleTabContent;
    public GameObject objectsTabContent;
    public GameObject locationsTabContent;
    public GameObject actionsTabContent;

    private string currentCategory;
    //private NotebookEntry currentSelectedEntry;

    private bool hasBeenPopulated = false;
    private Dictionary<string, WordData> wordLibrary;
    private PlayerData playerData;

    void Awake()
    {
        instance = this;
        notebookCanvas.SetActive(false);
    }

    void OnEnable()
    {
        DataManager.OnWordAddedToNotebook += HandleWordAdded;
    }

    void OnDisable()
    {
        DataManager.OnWordAddedToNotebook -= HandleWordAdded;
    }

    private void HandleWordAdded(NotebookEntry newEntry)
    {
        if (hasBeenPopulated)
        {
            CreateButtonForEntry(newEntry);
        }
    }

    private void CreateButtonForEntry(NotebookEntry newEntry)
    {
        WordData wordData = WordLibraryManager.instance.GetWordData(newEntry.wordId);
        if(wordData == null) return;
        
        Transform parentTab = GetParentTabForCategory(wordData.category);
        if (parentTab != null)
        {
            GameObject newButton = Instantiate(wordButtonPrefab, parentTab);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = wordData.text;
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectWord(newEntry));
        }
    }

    public void OpenNotebook()
    {
        Debug.Log("OpenNotebook");
        notebookCanvas.SetActive(true);
        PopulateAllTabs();
        SwitchToTab("people");
    }

    public void ClearAllTabs()
    {
        Debug.Log("ClearAllTabs");
        foreach (Transform child in peopleTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in objectsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in locationsTabContent.transform) {Destroy(child.gameObject);}
        foreach (Transform child in actionsTabContent.transform) {Destroy(child.gameObject);}
    }
    public void CloseNotebook()
    {
        notebookCanvas.SetActive(false);
    }

    private void PopulateAllTabs()
    {
        if(hasBeenPopulated) return;
        Debug.Log("PopulateAllTabs");
        ClearAllTabs();
        PlayerData playerData = DataManager.Instance.GetCurrentPlayerData();
        if (playerData == null){ Debug.Log("PlayerData is null in PopulateAllTabs"); return;}
        Debug.Log(playerData.notebookEntries.Count);
        foreach (var entry in playerData.notebookEntries)
        {
            CreateButtonForEntry(entry);
        }
        hasBeenPopulated = true;
    }

    private void SelectWord(NotebookEntry entry)
    {
        //currentSelectedEntry = entry;
        WordData wordData = WordLibraryManager.instance.GetWordData(entry.wordId);
        wordTitleText.text = wordData.text;
        userGuessInput.text = entry.userGuess;
    }

    public void SwitchToTab(string category)
    {
        peopleTabContent.transform.parent.parent.gameObject.SetActive(category == "people");
        objectsTabContent.transform.parent.parent.gameObject.SetActive(category == "objects");
        locationsTabContent.transform.parent.parent.gameObject.SetActive(category == "locations");
        actionsTabContent.transform.parent.parent.gameObject.SetActive(category == "actions");
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
        WordData wordData = WordLibraryManager.instance.GetWordData(wordId);
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
        PlayerData playerData = DataManager.Instance.GetCurrentPlayerData();
        NotebookEntry entryToShow = playerData.notebookEntries.Find(e => e.wordId == wordId);

        if (entryToShow != null)
        {
            SelectWord(entryToShow);
        }
    }
}