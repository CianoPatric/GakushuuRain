using System.Collections.Generic;
using System.Linq;

public class CustomizationService
{
    private readonly DataManager _dataManager;
    private readonly CosmeticsLibraryManager _library;
    private PlayerAppearance _playerAppearance;

    public CustomizationService(DataManager dataManager, CosmeticsLibraryManager library)
    {
        _dataManager = dataManager;
        _library = library;
    }

    public void BindPlayer(PlayerAppearance appearance)
    {
        _playerAppearance = appearance;
        RefreshAppearance();
    }

    public void EquipItem(string itemId, CosmeticSlot slot)
    {
        var data = _dataManager.GetCurrentPlayerData();
        var equipped = data.profile.equippedItems;

        switch (slot)
        {
            case CosmeticSlot.Hat: equipped.hatId = itemId; break;
            case CosmeticSlot.Neck: equipped.neckId = itemId; break;
            case CosmeticSlot.Accessory: equipped.accessoryId = itemId; break;
            case CosmeticSlot.Tors: equipped.torsId = itemId; break;
            case CosmeticSlot.Pants: equipped.pantsId = itemId; break;
            case CosmeticSlot.Boots: equipped.bootsId = itemId; break;
        }
        
        _dataManager.MarkDataAsDirty();
        RefreshAppearance();
    }

    public void RefreshAppearance()
    {
        if(_playerAppearance == null) return;
        var data = _dataManager.GetCurrentPlayerData();
        _playerAppearance.UpdateVisuals(data.profile.equippedItems);
    }

    public List<CosmeticItem> GetAvailableItems(CosmeticSlot slot)
    {
        var unlockedIds = _dataManager.GetCurrentPlayerData().unlockedCosmeticIds;
        return _library.AllCosmeticItems
            .Where(item => item.slot == slot && unlockedIds.Contains(item.id))
            .ToList();
    }

    public PlayerCustomization GetCurrentEquipped()
    {
        return _dataManager.GetCurrentPlayerData().profile.equippedItems;
    }

    public void SetDressingState(bool active) => _playerAppearance?.SetDressingActive(active);
    public void PlayAnimation(string trigger) => _playerAppearance?.TriggerAnimation(trigger);
}