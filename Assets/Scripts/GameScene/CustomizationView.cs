using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationView : MonoBehaviour
{
    [Header("Интерфейс кастомизации")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject itemListView;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Sprite emptySlotSprite;
    
    [Header("Иконки слотов")] 
    [SerializeField] private Image hatSlot;
    [SerializeField] private Image neckSlot;
    [SerializeField] private Image accessorySlot;
    [SerializeField] private Image shirtSlot;
    [SerializeField] private Image pantsSlot;
    [SerializeField] private Image bootsSlot;
    
    private CustomizationService _service;
    private CosmeticsLibraryManager _library;

    public void Initialize(CustomizationService service, CosmeticsLibraryManager library)
    {
        _service = service;
        _library = library;
    }
    
    public void Open()
    {
        mainPanel.SetActive(true);
        itemListView.SetActive(false);
        _service.SetDressingState(true);
        _service.PlayAnimation("openCustomization");
        UpdateUI();
    }
    
    public void Close()
    {
        mainPanel.SetActive(false);
        _service.SetDressingState(false);
    }

    public void ShowHatItems() => OnSlotClicked(CosmeticSlot.Hat);
    public void ShowShirtItems() => OnSlotClicked(CosmeticSlot.Tors);
    public void ShowPantsItems() => OnSlotClicked(CosmeticSlot.Pants);
    public void ShowAccessoryItems() => OnSlotClicked(CosmeticSlot.Accessory);
    
    public void OnSlotClicked(CosmeticSlot slot)
    {
        itemListView.SetActive(true);
        
        foreach (Transform child in itemsContainer) Destroy(child.gameObject);
        
        CreateSelectionButton(null, slot, emptySlotSprite);

        var items = _service.GetAvailableItems(slot);
        foreach (var item in items)
        {
            CreateSelectionButton(item.id, slot, item.sprite);
        }
    }

    private void CreateSelectionButton(string id, CosmeticSlot slot, Sprite icon)
    {
        var btnGo = Instantiate(itemButtonPrefab, itemsContainer);
        var btn = btnGo.GetComponent<Button>();
        
        var img = btnGo.GetComponentInChildren<Image>();
        if(img != null) img.sprite = icon;
        
        btn.onClick.AddListener(() =>
        {
            _service.EquipItem(id, slot);
            UpdateUI();
            itemListView.SetActive(false);
        });
    }

    private void UpdateUI()
    {
        var equipped = _service.GetCurrentEquipped();

        UpdateSlotIcon(hatSlot, equipped.hatId);
        UpdateSlotIcon(neckSlot, equipped.neckId);
        UpdateSlotIcon(accessorySlot, equipped.accessoryId);
        UpdateSlotIcon(shirtSlot, equipped.torsId);
        UpdateSlotIcon(pantsSlot, equipped.pantsId);
        UpdateSlotIcon(bootsSlot, equipped.bootsId);
    }

    private void UpdateSlotIcon(Image slot, string id)
    {
        if(slot == null) return;
        
        CosmeticItem item = _library.GetCosmeticItem(id);
        if (item != null && item.sprite != null)
        {
            slot.sprite = item.sprite;
            slot.enabled = true;
        }
        else
        {
            slot.sprite = emptySlotSprite;
        }
    }
}