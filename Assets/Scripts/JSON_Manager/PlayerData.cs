using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string userId;
    public string lastSaveTimestamp;

    public PlayerProfile profile;
    public PlayerState state;
    public List<InventoryItem> inventory;
    public List<QuestStatus> quests;
    public List<NotebookEntry> notebookEntries;

    public List<string> unlockedCosmeticIds;
}