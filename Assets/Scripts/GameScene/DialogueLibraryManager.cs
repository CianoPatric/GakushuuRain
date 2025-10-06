using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLibraryManager: MonoBehaviour
{
    private Dictionary<string, DialogueData> _dialogueLibrary = new();

    void Awake()
    {
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
                    _dialogueLibrary[dialogueData.dialogueId] = dialogueData;
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
        Debug.Log($"Загружено {_dialogueLibrary.Count} диалогов");
    }

    public DialogueData GetDialogue(string dialogueId)
    {
        if (_dialogueLibrary.ContainsKey(dialogueId))
        {
            return _dialogueLibrary[dialogueId];
        }
        Debug.LogError($"Диалог с ID '{dialogueId}' не найден в библиотеке");
        return null;
    }
}