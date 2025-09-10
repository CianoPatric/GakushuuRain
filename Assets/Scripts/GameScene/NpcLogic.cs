using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    public GameObject PressEWindow;
    public string dialogueStartId;
    private bool playerInRange = false;
    
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
            string startNode = DataManager.Instance.GetDialogueState(dialogueStartId);
            DialogueManager.Instance.StartDialogue(dialogueStartId, startNode);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressEWindow.SetActive(false);
            playerInRange = false;
            DialogueManager.Instance.HideDialogueLine();
        }
    }
}