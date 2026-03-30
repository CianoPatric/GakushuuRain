using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Postgrest;
using UnityEngine;
using Constants = Supabase.Postgrest.Constants;

public class DataManager : MonoBehaviour
{
    private PlayerData _currentPlayerData;
    private bool _isDataDirty = false;
    private int _activeSlotIndex = -1;
    
    private AuthManager _authManager;
    
    private Dictionary<int, string> _databaseIdCashe = new();

    public void Initialize(AuthManager authManager)
    {
        _authManager = authManager;
    }
    public static event Action<NotebookEntry> OnWordAddedToNotebook;
    
    public void MarkDataAsDirty()
    {
        _isDataDirty = true;
    }
    public async Task SaveData()
    {
        if (_currentPlayerData == null || _activeSlotIndex == -1)
        {
            Debug.Log("Нет активного слота для сохранения");
            return;
        }
        
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            _currentPlayerData.state.posX = player.transform.position.x;
            _currentPlayerData.state.posY = player.transform.position.y;
        }
        LocalSaveManager.SaveProfile(_currentPlayerData, _activeSlotIndex);
        
        var supabaseClient = _authManager.GetClient();
        if (!_authManager.IsUserLoggedIn())
        {
            Debug.LogError("Пользователь не авторизован, данные сохранены локально");
            return;
        }
        
        _currentPlayerData.lastSaveTimestamp = DateTime.UtcNow.ToString("o");

        string databaseId = null;
        if (_databaseIdCashe.ContainsKey(_activeSlotIndex))
        {
            databaseId = _databaseIdCashe[_activeSlotIndex];
        }

        var modelToSave = new PlayerDataModel()
        {
            Id = databaseId,
            UserId = supabaseClient.Auth.CurrentUser.Id,
            SlotIndex = _activeSlotIndex,
            Data = _currentPlayerData
        };
        
        var response = await supabaseClient.From<PlayerDataModel>()
            .Upsert(modelToSave, new QueryOptions{OnConflict = "user_id, slot_index"});
        
        if (response.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Данные успешно сохранены");
            _isDataDirty = false;
        }
        else
        {
            Debug.LogError("Ошибка сохранения: " + response.ResponseMessage.ReasonPhrase);
        }
    }

    public async Task<PlayerData> LoadData(int slotIndex)
    {
        _activeSlotIndex = slotIndex;
        var supabaseClient = _authManager.GetClient();
        if (!_authManager.IsUserLoggedIn())
        {
            Debug.LogError("Нет онлайн-сессии. Загрузка из локального сохранения");
            _currentPlayerData = LocalSaveManager.LoadProfile(_activeSlotIndex);
            return _currentPlayerData;
        }

        var response = await supabaseClient.From<PlayerDataModel>()
            .Filter("user_id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
            .Filter("slot_index", Constants.Operator.Equals, slotIndex)
            .Single();
        
        PlayerData serverData = response?.Data;

        if (response != null && !string.IsNullOrEmpty(response.Id))
        {
            _databaseIdCashe[slotIndex] = response.Id;
        }
        
        PlayerData localData = LocalSaveManager.LoadProfile(slotIndex);

        if (serverData != null || localData != null)
        {
            DateTime serverTime = DateTime.MinValue;
            if (serverData != null && !string.IsNullOrEmpty(serverData.lastSaveTimestamp))
            {
                DateTime.TryParse(serverData.lastSaveTimestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out serverTime);
            }
            DateTime localTime = DateTime.MinValue;

            if (localData != null && !string.IsNullOrEmpty(localData.lastSaveTimestamp))
            {
                DateTime.TryParse(localData.lastSaveTimestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out localTime);
            }

            if (serverTime >= localTime)
            {
                Debug.Log("Серверные данные новее, загружаются серверные данные");
                _currentPlayerData = serverData;
                LocalSaveManager.SaveProfile(serverData, _activeSlotIndex);
            }
            else
            {
                Debug.Log("Загружаются локальные данные");
                _currentPlayerData = localData;
                await SaveData();
            }
            return _currentPlayerData;
        }
        else
        {
            Debug.Log($"Сохранение для слота {slotIndex} не найдено, создание нового...");
            return await NewSaveData(slotIndex);
        }
    }

    public async Task<PlayerData> NewSaveData(int slotIndex)
    {
        _activeSlotIndex = slotIndex;
        var supabaseClient = _authManager.GetClient();
        if (!_authManager.IsUserLoggedIn())
        {
            Debug.LogError("Невозможно создать новые сохранения, пользователь не авторизован");
            return null;
        }
        
        var currentUser = supabaseClient.Auth.CurrentUser;
        string nickname = null;
        if (currentUser.UserMetadata.ContainsKey("nickname"))
        {
            nickname = currentUser.UserMetadata["nickname"].ToString();
        }

        _currentPlayerData = new PlayerData
        {
            userId = currentUser.Id,
            lastSaveTimestamp = DateTime.UtcNow.ToString("o"),
            profile = new PlayerProfile()
            {
                playerName = nickname,
                equippedItems = new PlayerCustomization()
            },
            state = new PlayerState { posX = -1100f, posY = -450f },
            inventory = new List<InventoryItem>(),
            quests = new List<QuestStatus>(),
            notebookEntries = new List<NotebookEntry>(),
            dialogueStates = new List<DialogueState>(),
            unlockedCosmeticIds = new List<string>()
        };
        await SaveData();
        return _currentPlayerData;
    }

    public async Task DeleteSaveSlot(int slotIndex)
    {
        LocalSaveManager.DeleteProfile(slotIndex);
        if (_authManager.IsUserLoggedIn())
        {
            var supabaseClient = _authManager.GetClient();
            await supabaseClient.From<PlayerDataModel>()
                .Filter("user_id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
                .Filter("slot_index", Constants.Operator.Equals, slotIndex)
                .Delete();
        }
    }

    public async Task<Dictionary<int, PlayerData>> GetAllSaveSlots()
    {
        var slots = new Dictionary<int, PlayerData>();
        if (_authManager.IsUserLoggedIn())
        {
            var supabaseClient = _authManager.GetClient();
            var responce = await supabaseClient.From<PlayerDataModel>()
                .Filter("user_id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
                .Get();
            if (responce.Models != null)
            {
                foreach (var model in responce.Models)
                {
                    slots[model.SlotIndex] = model.Data;
                    _databaseIdCashe[model.SlotIndex] = model.Id;
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (!slots.ContainsKey(i))
            {
                var localData = LocalSaveManager.LoadProfile(i);
                if (localData != null)
                {
                    slots[i] = localData;
                }
            }
        }
        return slots;
    }
    public void InitializeWithData(PlayerData data)
    {
        _currentPlayerData = data;
        Debug.Log("InitializeWithData выполнено");
    }

    public void SetDialogueState(string dialogueId, string lastNodeId)
    {
        if (_currentPlayerData == null) return;
        
        DialogueState existingState = _currentPlayerData.dialogueStates.Find(x => x.dialogueId == dialogueId);
        if (existingState != null)
        {
            existingState.lastNodeId = lastNodeId;
        }
        else
        {
            _currentPlayerData.dialogueStates.Add(new DialogueState{dialogueId = dialogueId, lastNodeId = lastNodeId});
        }
        Debug.Log($"Состояние диалога {dialogueId} сохранено на узле {lastNodeId}");
    }

    public string GetDialogueState(string dialogueId)
    {
        if (_currentPlayerData == null) return "start";
        DialogueState existingState = _currentPlayerData.dialogueStates.Find(x => x.dialogueId == dialogueId);
        if (existingState != null)
        {
            return existingState.lastNodeId;
        }
        else
        {
            return "start";
        }
    }
    public PlayerData GetCurrentPlayerData()
    {
        return _currentPlayerData;
    }

    public bool IsWordInNotebook(string wordId)
    {
        if(_currentPlayerData == null || _currentPlayerData.notebookEntries == null) return false;
        return _currentPlayerData.notebookEntries.Any(entry => entry.wordId == wordId);
    }

    public void UnlockCosmeticItem(string cosmeticId)
    {
        if(_currentPlayerData == null) return;
        if (!_currentPlayerData.unlockedCosmeticIds.Contains(cosmeticId))
        {
            _currentPlayerData.unlockedCosmeticIds.Add(cosmeticId);
            Debug.Log("Игрок разблокировал предмет: " + cosmeticId);
        }
    }

    public void AddWordToNotebook(string wordId)
    {
        if (IsWordInNotebook(wordId))
        {
            Debug.LogWarning($"Попытка добавить уже существующее слово: {wordId}");
            return;
        }

        NotebookEntry newEntry = new NotebookEntry
        {
            wordId = wordId,
            userGuess = "",
            status = WordStatus.Hypothesis,
            encounteredContext = new List<string>()
        };
        _currentPlayerData.notebookEntries.Add(newEntry);
        Debug.Log($"Слово {wordId} добавлено в блокнот");
        OnWordAddedToNotebook?.Invoke(newEntry);
    }

    private void OnApplicationQuit()
    {
        if (_isDataDirty && _currentPlayerData != null && _activeSlotIndex != -1)
        {
            LocalSaveManager.SaveProfile(_currentPlayerData, _activeSlotIndex);
        }
    }

    public void StartQuest(string questId)
    {
        var existingQuest = _currentPlayerData.quests.Find(q => q.questId == questId);
        if (existingQuest == null)
        {
            _currentPlayerData.quests.Add(new QuestStatus{questId = questId, currentStepIndex = 0, isCompleted = false});
            MarkDataAsDirty();
            Debug.Log($"Квест {questId} начат");
        }
    }

    public void AdvanceQuest(string questId)
    {
        var quest = _currentPlayerData.quests.Find(q => q.questId == questId);
        if (quest != null && !quest.isCompleted)
        {
            quest.currentStepIndex++;
            MarkDataAsDirty();
            Debug.Log($"Квест {questId} продвинут до шага {quest.currentStepIndex}");
        }
    }

    public void CompleteQuest(string questId)
    {
        var quest = _currentPlayerData.quests.Find(q => q.questId == questId);
        if (quest != null)
        {
            quest.isCompleted = true;
            MarkDataAsDirty();
            Debug.Log($"Квест {questId} завершён");
        }
    }

    public QuestStatus GetQuestStatus(string questId)
    {
        return _currentPlayerData.quests.Find(q => q.questId == questId);
    }

    public void SetWorldFlag(string key, bool value)
    {
        if(_currentPlayerData == null) return;
        _currentPlayerData.worldStateFlags[key] = value;
        MarkDataAsDirty();
        Debug.Log($"Ключевой момент '{key}' изменён на {value}");
    }

    public bool GetWorldFlag(string key)
    {
        if(_currentPlayerData == null) return false;
        return _currentPlayerData.worldStateFlags.TryGetValue(key, out bool value) && value;
    }
}