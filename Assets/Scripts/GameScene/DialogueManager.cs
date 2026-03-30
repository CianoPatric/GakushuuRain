using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public GameObject continueButton;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI dialogue;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;
    private string _currentRawText;
    
    public Color newWordColor = Color.yellow;
    public Color knowWordColor = Color.black;

    private DialogueData _currentDialogue;
    private DialogueNode _currentNode;
    
    private DataManager _dataManager;
    private DialogueLibraryManager _dialogueLibraryManager;
    private WordLibraryManager _wordLibraryManager;
    private NotebookView _notebookView;
    private PlayerAppearance _playerAppearance;
    
    [SerializeField] private DialogueTextClickHandler dialogueTextClickHandler;

    public void Initialize(DataManager dataManager, DialogueLibraryManager dialogueLibraryManager,
        WordLibraryManager wordLibraryManager, NotebookView notebookView, PlayerAppearance playerAppearance)
    {
        _dataManager = dataManager;
        _dialogueLibraryManager = dialogueLibraryManager;
        _wordLibraryManager = wordLibraryManager;
        _notebookView = notebookView;
        _playerAppearance = playerAppearance;
        dialogueBox.SetActive(false);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void StartDialogue(string dialogueId, string startNodeId = "start")
    {
        _currentDialogue = _dialogueLibraryManager.GetDialogue(dialogueId);
        if (_currentDialogue == null)
        {
            Debug.LogError($"Ну удалось запустить диалог: {dialogueId} не найден");
            HideDialogueLine();
            return;
        }
        dialogueBox.SetActive(true);
        _playerAppearance.SetDialogueActive(true);
        _currentNode = _currentDialogue.nodes.Find(note => note.nodeId == startNodeId);
        
        if (_currentNode == null)
        {
            HideDialogueLine();
            return;
        }
        
        
        DisplayCurrentNode();
    }

    public void DisplayCurrentNode()
    {
        if (_currentNode == null)
        {
            HideDialogueLine();
            return;
        }

        if (speakerName != null)
        {
            speakerName.text = _currentNode.speaker;
        }
        string processedText = ProcessText(_currentNode.text);
        dialogue.text = processedText;

        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        if (_currentNode.options != null && _currentNode.options.Count > 0)
        {
            continueButton.SetActive(false);
            foreach (DialogueOption option in _currentNode.options)
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
        
        ExecuteActions(_currentNode.actions);
    }

    public void ContinueDialogue()
    {
        if (!string.IsNullOrEmpty(_currentNode.nextNodeId))
        {
            _currentNode = _currentDialogue.nodes.Find(note => note.nodeId == _currentNode.nextNodeId);
            DisplayCurrentNode();
        }
        else
        {
            HideDialogueLine();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OptionSelected(DialogueOption option)
    {
        ExecuteActions(option.actions);
        
        _currentNode = _currentDialogue.nodes.Find(node => node.nodeId == option.nextNodeId);
        DisplayCurrentNode();
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
                case "start_quest":
                    _dataManager.StartQuest(action.targetId);
                    break;
                case "advance_quest":
                    _dataManager.AdvanceQuest(action.targetId);
                    break;
                case "complete_quest":
                    _dataManager.CompleteQuest(action.targetId);
                    break;
                default:
                    Debug.Log($"Незвестное слово в диалоге: {action.type}");
                    break;
            }
        }
    }

    public void RedRawCurrentLine()
    {
        if (dialogueBox.activeSelf && _currentNode != null)
        {
            string processedText = ProcessText(_currentNode.text);
            dialogue.text = processedText;
        }
    }
    public void HideDialogueLine()
    {
        if (_currentDialogue != null && _currentNode != null)
        {
            _dataManager.SetDialogueState(_currentDialogue.dialogueId, _currentNode.nodeId);
        }
        
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        _playerAppearance.SetDialogueActive(false);
        _currentDialogue = null;
        _currentNode = null;
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
            _notebookView.OpenAndShowWord(wordId);
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