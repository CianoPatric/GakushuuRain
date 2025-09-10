using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    public GameObject DialogueBox;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI dialogue;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;
    private string currentRawText;
    
    public Color newWordColor = Color.yellow;
    public Color knowWordColor = Color.black;

    private DialogueData currentDialogue;
    private DialogueNote currentNode;
    void Awake()
    {
        Instance = this;
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false);
        }
    }

    void Update()
    {
        if (DialogueBox.activeSelf && currentNode != null &&
            (currentNode.options == null || currentNode.options.Count == 0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ContinueDialogue();
            }
        }
    }
    public void StartDialogue(string dialogueId, string startNoteId = "start")
    {
        currentDialogue = DialogueLibraryManager.instance.GetDialogue(dialogueId);
        if (currentDialogue == null)
        {
            Debug.LogError($"Ну удалось запустить диалог: {dialogueId} не найден");
            HideDialogueLine();
            return;
        }
        
        DialogueBox.SetActive(true);
        currentNode = currentDialogue.nodes.Find(note => note.noteId == startNoteId);
        DisplayCurrentNode();
    }

    public void DisplayCurrentNode()
    {
        if (currentNode == null)
        {
            HideDialogueLine();
            return;
        }

        if (speakerName != null)
        {
            speakerName.text = currentNode.speaker;
        }
        string processedText = ProcessText(currentNode.text);
        dialogue.text = processedText;

        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentNode.options != null && currentNode.options.Count > 0)
        {
            foreach (DialogueOption option in currentNode.options)
            {
                GameObject optionButton = Instantiate(optionButtonPrefab, optionsContainer);
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = option.text;
                optionButton.GetComponent<Button>().onClick.AddListener(() => OptionSelected(option));
            }
        }
        
        ExecuteActions(currentNode.actions);
    }

    private void ContinueDialogue()
    {
        if (!string.IsNullOrEmpty(currentNode.nextNoteId))
        {
            currentNode = currentDialogue.nodes.Find(note => note.noteId == currentNode.nextNoteId);
            DisplayCurrentNode();
        }
        else
        {
            HideDialogueLine();
        }
    }

    private void OptionSelected(DialogueOption option)
    {
        ExecuteActions(option.actions);
        
        currentNode = currentDialogue.nodes.Find(note => note.noteId == option.nextNoteId);
        DisplayCurrentNode();
    }

    private void ExecuteActions(List<QuestAction> actions)
    {
        if(actions == null) return;
        foreach (QuestAction action in actions)
        {
            switch (action.type)
            {
                case "start_dialogue":
                    Debug.Log($"Начать квест {action.targetId}");
                    break;
                case "complete_quest_step":
                    Debug.Log($"Завершить шаг квеста {action.targetId}, шаг: {action.value}");
                    break;
                case "give_item":
                    Debug.Log($"Выдать предмет: {action.targetId}, количество: {action.value}");
                    break;
                case "learn_word":
                    Debug.Log($"Изучить слово: {action.targetId}");
                    DataManager.Instance.AddWordToNotebook(action.targetId);
                    break;
                case "end_dialogue":
                    HideDialogueLine();
                    break;
                default:
                    Debug.Log($"Незвестное слово в диалоге: {action.type}");
                    break;
            }
        }
    }

    public void RedRawCurrentLine()
    {
        if (DialogueBox.activeSelf && currentNode != null)
        {
            string processedText = ProcessText(currentNode.text);
            dialogue.text = processedText;
        }
    }
    public void HideDialogueLine()
    {
        if (currentDialogue != null && currentNode != null)
        {
            DataManager.Instance.SetDialogueState(currentDialogue.dialogueId, currentNode.noteId);
        }

        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false);
        }
        currentDialogue = null;
        currentNode = null;
    }

    private string ProcessText(string rawText)
    {
        string pattern = @"\{([\w_]+)\}";
        string processed = Regex.Replace(rawText, pattern, match =>
        {
            string wordId = match.Groups[1].Value;
            
            WordData wordData = WordLibraryManager.instance.GetWordData(wordId);
            if (wordData == null) return "Слово не было найдено";
            
            string wordToDisplay = wordData.text;
            bool isKnown = DataManager.Instance.IsWordInNotebook(wordId);

            string colorHex;
            if (isKnown)
            {
                colorHex = ColorUtility.ToHtmlStringRGB(knowWordColor);
            }
            else
            {
                colorHex = ColorUtility.ToHtmlStringRGB(newWordColor);
            }

            return $"<link=\"{wordId}\"><color=#{colorHex}><u>{wordToDisplay}</u></color></link>";
        });
        return processed;
    }

    public void OnWordClicked(string wordId)
    {
        bool isKnown = DataManager.Instance.IsWordInNotebook(wordId);
        if (isKnown)
        {
            NotebookManager.instance.OpenAndShowWord(wordId);
            Debug.Log("Блокнот открыт");
        }
        else
        {
            DataManager.Instance.AddWordToNotebook(wordId);
            RedRawCurrentLine();
            Debug.Log("Слово добавленно");
        }
    }
}