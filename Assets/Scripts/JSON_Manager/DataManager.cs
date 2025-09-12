using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Constants = Supabase.Postgrest.Constants;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private PlayerData currentPlayerData;
    private bool isDataDirty = false;

    public static event Action<NotebookEntry> OnWordAddedToNotebook;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkDataAsDirty()
    {
        isDataDirty = true;
    }
    public async Task SaveData()
    {
        var supabaseClient = AuthManager.Instance.GetClient();
        if (currentPlayerData == null || supabaseClient.Auth.CurrentUser == null)
        {
            Debug.LogError("Нет данных для сохранения или пользователь не авторизован");
            return;
        }

        if (!isDataDirty && false)
        {
            return;
        }
        currentPlayerData.userId = supabaseClient.Auth.CurrentUser.Id;
        currentPlayerData.lastSaveTimestamp = DateTime.UtcNow.ToString("o");

        var response = await supabaseClient.From<PlayerDataModel>()
            .Upsert(new PlayerDataModel
            {
                Id = supabaseClient.Auth.CurrentUser.Id,
                Data = currentPlayerData
            });
        if (response.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Данные успешно сохранены");
            isDataDirty = false;
        }
        else
        {
            Debug.LogError("Ошибка сохранения: " + response.ResponseMessage.ReasonPhrase);
        }
    }

    public async Task<PlayerData> LoadData()
    {
        var supabaseClient = AuthManager.Instance.GetClient();
        if (supabaseClient.Auth.CurrentUser == null)
        {
            Debug.LogError("Пользователь не авторизован");
            return null;
        }

        var response = await supabaseClient.From<PlayerDataModel>()
            .Filter("id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
            .Single();

        if (response != null && response.Data != null)
        {
            Debug.Log("Данные успешно загружены");
            currentPlayerData = response.Data;
            return currentPlayerData;
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
            currentPlayerData = new PlayerData
            {
                userId = supabaseClient.Auth.CurrentUser.Id,
                profile = new PlayerProfile
                {
                    playerName = nickname,
                    equippedItems = new PlayerCustomization()
                },
                state = new PlayerState { posX = 0f, posY = 0f },
                inventory = new List<InventoryItem>(),
                quests = new List<QuestStatus>(),
                notebookEntries = new List<NotebookEntry>(),
                dialogueStates = new List<DialogueState>(),
                unlockedCosmeticIds = new List<string>()
            };
            await SaveData();
            return currentPlayerData;
        }
    }

    public void InitializeWithData(PlayerData data)
    {
        currentPlayerData = data;
        Debug.Log("InitializeWithData выполнено");
    }

    public void SetDialogueState(string dialogueId, string lastNodeId)
    {
        if (currentPlayerData == null) return;
        
        DialogueState existingState = currentPlayerData.dialogueStates.Find(x => x.dialogueId == dialogueId);
        if (existingState != null)
        {
            existingState.lastNodeId = lastNodeId;
        }
        else
        {
            currentPlayerData.dialogueStates.Add(new DialogueState{dialogueId = dialogueId, lastNodeId = lastNodeId});
        }
        Debug.Log($"Состояние диалога {dialogueId} сохранено на узле {lastNodeId}");
    }

    public string GetDialogueState(string dialogueId)
    {
        if (currentPlayerData == null) return "start";
        DialogueState existingState = currentPlayerData.dialogueStates.Find(x => x.dialogueId == dialogueId);
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
        return currentPlayerData;
    }

    public bool IsWordInNotebook(string wordId)
    {
        if(currentPlayerData == null || currentPlayerData.notebookEntries == null) return false;
        return currentPlayerData.notebookEntries.Any(entry => entry.wordId == wordId);
    }

    public void UnlockCosmeticItem(string cosmeticId)
    {
        if(currentPlayerData == null) return;
        if (!currentPlayerData.unlockedCosmeticIds.Contains(cosmeticId))
        {
            currentPlayerData.unlockedCosmeticIds.Add(cosmeticId);
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
        currentPlayerData.notebookEntries.Add(newEntry);
        Debug.Log($"Слово {wordId} добавлено в блокнот");
        OnWordAddedToNotebook?.Invoke(newEntry);
    }

    private void OnApplicationQuit()
    {
        if (isDataDirty)
        {
            _ = SaveData();
        }
    }
}