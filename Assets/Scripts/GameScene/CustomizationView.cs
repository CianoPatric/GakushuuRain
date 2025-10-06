using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationView : MonoBehaviour
{
    [Header("Интерфейс кастомизации")]
    [SerializeField] private GameObject customizationPanel;

    [SerializeField] private GameObject itemScrollView;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Sprite nullItemSprite;
    
    [Header("Иконки слотов")] 
    [SerializeField] private Image hatSlot;
    [SerializeField] private Image shirtSlot;
    [SerializeField] private Image pantsSlot;
    [SerializeField] private Image accessorySlot;
    
    private DataManager _dataManager;
    private CosmeticsLibraryManager _cosmeticsLibraryManager;
    private PlayerMovement _player;

    public void Initialize(DataManager dataManager, CosmeticsLibraryManager cosmeticsLibraryManager,
        PlayerMovement player)
    {
        _dataManager = dataManager;
        _cosmeticsLibraryManager = cosmeticsLibraryManager;
        _player = player;
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
    
    public void ShowHatItems(){ShowItemForSlot(CosmeticSlot.Hat);}
    public void ShowShirtItems() {ShowItemForSlot(CosmeticSlot.Shirt);}
    public void ShowPantsItems() {ShowItemForSlot(CosmeticSlot.Pants);}
    public void ShowAccessoryItems() {ShowItemForSlot(CosmeticSlot.Accessory);}
    
    public void ShowItemForSlot(CosmeticSlot slot)
    {
        itemScrollView.SetActive(true);
        foreach (Transform child in itemsContainer) { Destroy(child.gameObject); }
        
        GameObject unequipButton = Instantiate(itemButtonPrefab, itemsContainer);
        unequipButton.GetComponentInChildren<Image>().sprite = nullItemSprite;
        unequipButton.GetComponentInChildren<Button>().onClick.AddListener(() => EquipItem(null, slot ));

        var unlockedIds = _dataManager.GetCurrentPlayerData().unlockedCosmeticIds;
        var availableItems = _cosmeticsLibraryManager.AllCosmeticItems
            .Where(item => item.slot == slot && unlockedIds.Contains(item.id))
            .ToList();

        foreach (var item in availableItems)
        {
            GameObject buttonGo = Instantiate(itemButtonPrefab, itemsContainer);
            buttonGo.GetComponentInChildren<Image>().sprite = item.sprite;
            buttonGo.GetComponentInChildren<Button>().onClick.AddListener(() => EquipItem(item.id, slot));
        }
    }
    
    private void EquipItem(string itemId, CosmeticSlot slot)
    {
        var equippedItems = _dataManager.GetCurrentPlayerData().profile.equippedItems;
        switch (slot)
        {
            case CosmeticSlot.Hat: equippedItems.hatId = itemId; break;
            case CosmeticSlot.Shirt: equippedItems.shirtId = string.IsNullOrEmpty(itemId) ? "shirt_default": itemId; break;
            case CosmeticSlot.Pants: equippedItems.pantsId = string.IsNullOrEmpty(itemId) ? "pants_default": itemId; break;
            case CosmeticSlot.Accessory: equippedItems.accessoryId = itemId; break;
        }
        
        _dataManager.MarkDataAsDirty();
        _player.UpdateAppearance(equippedItems);
        UpdateAllSlotIcons();
        itemScrollView.SetActive(false);
    }
    
    private void UpdateAllSlotIcons()
    {
        var equippedItems = _dataManager.GetCurrentPlayerData().profile.equippedItems;
        UpdateSlotIcons(hatSlot, equippedItems.hatId);
        UpdateSlotIcons(shirtSlot, equippedItems.shirtId);
        UpdateSlotIcons(pantsSlot, equippedItems.pantsId);
        UpdateSlotIcons(accessorySlot, equippedItems.accessoryId);
    }
    
    private void UpdateSlotIcons(Image iconImage, string itemId)
    {
        CosmeticItem itemData = _cosmeticsLibraryManager.GetCosmeticItem(itemId);
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
}