using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialogueBox;
    public GameObject continueButton;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI dialogue;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;
    private string currentRawText;
    
    public Color newWordColor = Color.yellow;
    public Color knowWordColor = Color.black;

    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    
    private DataManager _dataManager;
    private DialogueLibraryManager _dialogueLibraryManager;
    private WordLibraryManager _wordLibraryManager;
    private NotebookManager _notebookManager;
    
    [SerializeField] private DialogueTextClickHandler dialogueTextClickHandler;

    public void Initialize(DataManager dataManager, DialogueLibraryManager dialogueLibraryManager,
        WordLibraryManager wordLibraryManager, NotebookManager notebookManager)
    {
        _dataManager = dataManager;
        _dialogueLibraryManager = dialogueLibraryManager;
        _wordLibraryManager = wordLibraryManager;
        _notebookManager = notebookManager;
    }
    
    public void StartDialogue(string dialogueId, string startNodeId = "start")
    {
        currentDialogue = _dialogueLibraryManager.GetDialogue(dialogueId);
        if (currentDialogue == null)
        {
            Debug.LogError($"Ну удалось запустить диалог: {dialogueId} не найден");
            HideDialogueLine();
            return;
        }
        DialogueBox.SetActive(true);
        currentNode = currentDialogue.nodes.Find(note => note.nodeId == startNodeId);
        
        if (currentNode == null)
        {
            HideDialogueLine();
            return;
        }
        
        
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
            continueButton.SetActive(false);
            foreach (DialogueOption option in currentNode.options)
            {
                DialogueOption capturedOption = option;
                GameObject optionButton = Instantiate(optionButtonPrefab, optionsContainer);
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = capturedOption.text;
                optionButton.GetComponent<Button>().onClick.AddListener(() => OptionSelected(capturedOption));
            }
        }
        else
        {
            continueButton.SetActive(true);
        }
        
        ExecuteActions(currentNode.actions);
    }

    public void ContinueDialogue()
    {
        if (!string.IsNullOrEmpty(currentNode.nextNodeId))
        {
            currentNode = currentDialogue.nodes.Find(note => note.nodeId == currentNode.nextNodeId);
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
        
        currentNode = currentDialogue.nodes.Find(node => node.nodeId == option.nextNodeId);
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
                case "give_cosmetic":
                    Debug.Log($"Выдать косметику: {action.targetId}");
                    _dataManager.UnlockCosmeticItem(action.targetId);
                    break;
                case "learn_word":
                    Debug.Log($"Изучить слово: {action.targetId}");
                    _dataManager.AddWordToNotebook(action.targetId);
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
            _dataManager.SetDialogueState(currentDialogue.dialogueId, currentNode.nodeId);
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
            
            WordData wordData = _wordLibraryManager.GetWordData(wordId);
            if (wordData == null) return "Слово не было найдено";
            
            string wordToDisplay = wordData.text;
            bool isKnown = _dataManager.IsWordInNotebook(wordId);

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
        bool isKnown = _dataManager.IsWordInNotebook(wordId);
        if (isKnown)
        {
            _notebookManager.OpenAndShowWord(wordId);
            Debug.Log("Блокнот открыт");
        }
        else
        {
            _dataManager.AddWordToNotebook(wordId);
            RedRawCurrentLine();
            Debug.Log("Слово добавленно");
        }
    }
}