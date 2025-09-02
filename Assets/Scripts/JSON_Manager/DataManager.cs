using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Postgrest;
using UnityEngine;
using Client = Supabase.Client;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private Client supabaseClient;
    private PlayerData currentPlayerData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        var url = ClientInfo.CLIENT_URL;
        var key = ClientInfo.CLIENT_KEY;
        supabaseClient = new Client(url, key);
    }

    public async Task SaveData()
    {
        if (currentPlayerData == null || supabaseClient.Auth.CurrentUser == null)
        {
            Debug.LogError("Нет данных для сохранения или пользователь не авторизован");
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
        }
        else
        {
            Debug.LogError("Ошибка сохранения: " + response.ResponseMessage.ReasonPhrase);
        }
    }

    public async Task<PlayerData> LoadData()
    {
        if (supabaseClient.Auth.CurrentUser == null)
        {
            Debug.LogError("Пользователь не авторизован");
            return null;
        }

        var response = await supabaseClient.From<PlayerDataModel>()
            .Filter("Id", Constants.Operator.Equals, supabaseClient.Auth.CurrentUser.Id)
            .Single();

        if (response != null && response.Data != null)
        {
            Debug.Log("Данные успешно сохранены");
            currentPlayerData = response.Data;
            return currentPlayerData;
        }
        else
        {
            Debug.Log("Сохранение не найдено, создание нового...");
            currentPlayerData = new PlayerData
            {
                userId = supabaseClient.Auth.CurrentUser.Id,
                profile = new PlayerProfile { playerName = "New Player" },
                state = new PlayerState { posX = 0f, posY = 0f },
                inventory = new List<InventoryItem>(),
                quests = new List<QuestStatus>(),
                notebookEntries = new List<NotebookEntry>()
            };
            return currentPlayerData;
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
    }
}