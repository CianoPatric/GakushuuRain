using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    public GameObject PressEWindow;
    [TextArea(3, 10)]
    public string dialogueText;
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
            DialogueManager.Instance.ShowDialogueLine(dialogueText);
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