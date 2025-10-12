using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Constants = Supabase.Postgrest.Constants;

public class DataManager : MonoBehaviour
{
    private PlayerData _currentPlayerData;
    private bool _isDataDirty = false;
    
    private AuthManager _authManager;

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
        if (_currentPlayerData == null)
        {
            Debug.Log("Данных для сохранения нет");
            return;
        }

        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            _currentPlayerData.state.posX = player.transform.position.x;
            _currentPlayerData.state.posY = player.transform.position.y;
        }
        LocalSaveManager.SaveProfile(_currentPlayerData);
        
        var supabaseClient = _authManager.GetClient();
        if (supabaseClient.Auth.CurrentUser == null)
        {
            Debug.LogError("Пользователь не авторизован, данные сохранены локально");
            return;
        }

        if (!_isDataDirty && false)
        {
            return;
        }
        _currentPlayerData.userId = supabaseClient.Auth.CurrentUser.Id;
        _currentPlayerData.lastSaveTimestamp = DateTime.UtcNow.ToString("o");

        var response = await supabaseClient.From<PlayerDataModel>()
            .Upsert(new PlayerDataModel
            {
                Id = supabaseClient.Auth.CurrentUser.Id,
                Data = _currentPlayerData
            });
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

    public async Task<PlayerData> LoadData()
    {
        var supabaseClient = _authManager.GetClient();
        if (!_authManager.IsUserLoggedIn())
        {
            Debug.LogError("Нет онлайн-сессии. Загрузка из локального сохранения");
            _currentPlayerData = LocalSaveManager.LoadProfile();
            return _currentPlayerData;
        }

        var response = await supabaseClient.From<PlayerDataModel>()
            .Filter("id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
            .Single();
        
        PlayerData serverData = (response != null && response.Data != null) ? response.Data : null;
        PlayerData localData = LocalSaveManager.LoadProfile();

        if (serverData != null)
        {
            DateTime serverTime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(serverData.lastSaveTimestamp))
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
                LocalSaveManager.SaveProfile(serverData);
            }
            else
            {
                Debug.Log("Загружаются локальные данные");
                _currentPlayerData = localData;
                await SaveData();
            }
            return _currentPlayerData;
        }
        else if (localData != null)
        {
            Debug.Log("Серверные данные не найдены");
            _currentPlayerData = localData;
            await SaveData();
            return _currentPlayerData;
        }
        else
        {
            Debug.Log("Сохранение не найдено, создание нового...");
            
            var currentUser = supabaseClient.Auth.CurrentUser;
            string nickname = "New player";
            if (currentUser.UserMetadata.ContainsKey("nickname"))
            {
                nickname = currentUser.UserMetadata["nickname"].ToString();
            }
            _currentPlayerData = new PlayerData
            {
                userId = supabaseClient.Auth.CurrentUser.Id,
                lastSaveTimestamp = DateTime.UtcNow.ToString("o"),
                profile = new PlayerProfile
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
    }

    public async Task<PlayerData> NewSaveData()
    {
        var supabaseClient = _authManager.GetClient();
        if (supabaseClient.Auth.CurrentUser == null)
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
        MarkDataAsDirty();
        await SaveData();
        return _currentPlayerData;
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
        if (_isDataDirty)
        {
            _ = SaveData();
        }
    }
}