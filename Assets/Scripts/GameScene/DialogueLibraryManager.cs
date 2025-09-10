using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLibraryManager: MonoBehaviour
{
    public static DialogueLibraryManager instance {get; private set;}
    private Dictionary<string, DialogueData> dialogueLibrary = new();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllDialogue();
    }

    private void LoadAllDialogue()
    {
        TextAsset[] dialogueFile = Resources.LoadAll<TextAsset>("Dialogue");
        foreach (TextAsset file in dialogueFile)
        {
            try
            {
                DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(file.text);
                if (dialogueData != null && !string.IsNullOrEmpty(dialogueData.dialogueId))
                {
                    dialogueLibrary[dialogueData.dialogueId] = dialogueData;
                }
                else
                {
                    Debug.LogWarning($"Ошибка парсинга или пустой ID диалога в файле: {file.name}");
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Ошибка при загрузке диалога из файла {file.name}: {e.Message}");
            }
        }
        Debug.Log($"Загружено {dialogueLibrary.Count} диалогов");
    }

    public DialogueData GetDialogue(string dialogueId)
    {
        if (dialogueLibrary.ContainsKey(dialogueId))
        {
            return dialogueLibrary[dialogueId];
        }
        Debug.LogError($"Диалог с ID '{dialogueId}' не найден в библиотеке");
        return null;
    }
}