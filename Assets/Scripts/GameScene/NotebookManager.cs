using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager instance;
    
    public GameObject notebookCanvas;
    public GameObject wordPanel;
    public GameObject wordButtonPrefab;

    public Sprite NullItemSprite;
    public PlayerMovement player;

    [Header("Интерфейс кастомизации")]
    public GameObject customizationPanel;

    public GameObject itemScrollView;
    public Transform itemsContainer;
    public GameObject itemButtonPrefab;

    [Header("Иконки слотов")] 
    public Image hatSlot;
    public Image shirtSlot;
    public Image pantsSlot;
    public Image accessorySlot;
    
    private PlayerCustomization tempEquippedItems;
    
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

    private string currentCategory;
    private NotebookEntry currentSelectedEntry;
    
    private Dictionary<string, WordData> wordLibrary;

    void Awake()
    {
        instance = this;
        notebookCanvas.SetActive(false);
        wordPanel.SetActive(false);
    }

    public void Initialize()
    {
        Debug.Log("NotebookManager: Инициализация и заполнение из сохранения");
        ClearAllTabs();
        PlayerData playerData = DataManager.Instance.GetCurrentPlayerData();
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

    public void OpenCustomizationPanel()
    {
        customizationPanel.SetActive(true);
        itemScrollView.SetActive(false);
        UpdateAllSlotIcons();
    }

    public void CloseCustomizationPanel()
    {
        customizationPanel.SetActive(false);
        itemScrollView.SetActive(false);
    }

    private void UpdateAllSlotIcons()
    {
        var equippedItems = DataManager.Instance.GetCurrentPlayerData().profile.equippedItems;
        UpdateSlotIcons(hatSlot, equippedItems.hatId);
        UpdateSlotIcons(shirtSlot, equippedItems.shirtId);
        UpdateSlotIcons(pantsSlot, equippedItems.pantsId);
        UpdateSlotIcons(accessorySlot, equippedItems.accessoryId);
    }
    private void UpdateSlotIcons(Image iconImage, string itemId)
    {
        CosmeticItem itemData = CosmeticsLibraryManager.Instance.GetCosmeticItem(itemId);
        if (itemData != null && itemData.sprite != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = itemData.sprite;
        }
        else
        {
            iconImage.enabled = false;
        }
    }

    public void ShowItemForSlot(CosmeticSlot slot)
    {
        itemScrollView.SetActive(true);
        foreach (Transform child in itemsContainer) { Destroy(child.gameObject); }
        
        GameObject unequipButton = Instantiate(itemButtonPrefab, itemsContainer);
        unequipButton.GetComponentInChildren<Image>().sprite = NullItemSprite;
        unequipButton.GetComponentInChildren<Button>().onClick.AddListener(() => EquipItem(null, slot ));

        var unlockedIds = DataManager.Instance.GetCurrentPlayerData().unlockedCosmeticIds;
        var availableItems = CosmeticsLibraryManager.Instance.allCosmeticItems
            .Where(item => item.slot == slot && unlockedIds.Contains(item.id))
            .ToList();

        foreach (var item in availableItems)
        {
            GameObject buttonGo = Instantiate(itemButtonPrefab, itemsContainer);
            buttonGo.GetComponentInChildren<Image>().sprite = item.sprite;
            buttonGo.GetComponentInChildren<Button>().onClick.AddListener(() => EquipItem(item.id, slot));
        }
    }
    
    public void ShowHatItems(){ShowItemForSlot(CosmeticSlot.Hat);}
    public void ShowShirtItems() {ShowItemForSlot(CosmeticSlot.Shirt);}
    public void ShowPantsItems() {ShowItemForSlot(CosmeticSlot.Pants);}
    public void ShowAccessoryItems() {ShowItemForSlot(CosmeticSlot.Accessory);}

    private void EquipItem(string itemId, CosmeticSlot slot)
    {
        var playerData = DataManager.Instance.GetCurrentPlayerData();
        var equippedItems = playerData.profile.equippedItems;
        switch (slot)
        {
            case CosmeticSlot.Hat: equippedItems.hatId = itemId; break;
            case CosmeticSlot.Shirt: equippedItems.shirtId = string.IsNullOrEmpty(itemId) ? "shirt_default": itemId; break;
            case CosmeticSlot.Pants: equippedItems.pantsId = string.IsNullOrEmpty(itemId) ? "pants_default": itemId; break;
            case CosmeticSlot.Accessory: equippedItems.accessoryId = itemId; break;
        }
        
        DataManager.Instance.MarkDataAsDirty();
        player.UpdateAppearance(equippedItems);
        UpdateAllSlotIcons();
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
        currentSelectedEntry = entry;
        WordData wordData = WordLibraryManager.instance.GetWordData(entry.wordId);
        wordTitleText.text = wordData.text;
        translationText.text = wordData.translation;
        definitionText.text = wordData.definition;
        userGuessInput.text = entry.userGuess;
        wordPanel.SetActive(true);
    }

    private void SaveChangesForCurrentEntry()
    {
        if (currentSelectedEntry != null)
        {
            if (currentSelectedEntry.userGuess != userGuessInput.text)
            {
                currentSelectedEntry.userGuess = userGuessInput.text;
                DataManager.Instance.MarkDataAsDirty();
                Debug.Log($"Изменения для слова '{currentSelectedEntry.wordId}' сохранены");
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
        currentSelectedEntry = null;
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