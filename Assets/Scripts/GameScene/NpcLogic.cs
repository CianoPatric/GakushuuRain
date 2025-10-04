using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    public GameObject PressEWindow;
    public string dialogueStartId;
    private bool playerInRange = false;
    
    private DataManager _dataManager;
    private DialogueManager _dialogueManager;

    public void Initialize(DataManager dataManager, DialogueManager dialogueManager)
    {
        _dataManager = dataManager;
        _dialogueManager = dialogueManager;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressEWindow.SetActive(true);
            playerInRange = true;
        }
    }

    public void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            string startNode = _dataManager.GetDialogueState(dialogueStartId);
            _dialogueManager.StartDialogue(dialogueStartId, startNode);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressEWindow.SetActive(false);
            playerInRange = false;
            _dialogueManager.HideDialogueLine();
        }
    }
}