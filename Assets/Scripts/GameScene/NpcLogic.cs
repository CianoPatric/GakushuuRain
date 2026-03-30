using System.Collections.Generic;
using UnityEngine;

public class NpcLogic : MonoBehaviour, IInteractable
{
    public string defaultDialogueId;
    public List<ConditionalDialogue> conditionalDialogues;
    public GameObject pressEIndicator;
    
    private DataManager _dataManager;
    private DialogueManager _dialogueManager;

    public void Initialize(DataManager dataManager, DialogueManager dialogueManager)
    {
        _dataManager = dataManager;
        _dialogueManager = dialogueManager;
    }

    public void Interact()
    {
        string dialogueToStart = GetCorrectDialogueId();
        string startNode = _dataManager.GetDialogueState(dialogueToStart);
        _dialogueManager.StartDialogue(dialogueToStart, startNode);
    }

    public void ShowIndicator(bool show)
    {
        if(pressEIndicator != null) pressEIndicator.SetActive(show);
    }
    private string GetCorrectDialogueId()
    {
        foreach (var condition in conditionalDialogues)
        {
            if (!string.IsNullOrEmpty(condition.requiredWorldFlag))
            {
                if (_dataManager.GetWorldFlag(condition.requiredWorldFlag))
                {
                    return condition.dialogueId;
                }
            }   
        }
        
        foreach (var condition in conditionalDialogues)
        {
            var questStatus = _dataManager.GetQuestStatus(condition.questId);
            if (questStatus != null && !questStatus.isCompleted &&
                questStatus.currentStepIndex == condition.requiredStep)
            {
                return condition.dialogueId;
            }
        }
        return defaultDialogueId;
    }
}