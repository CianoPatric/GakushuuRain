using TMPro;
using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    private Transform body;
    private DialogueManager DialogueWindow;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        body = transform;
        var window = Resources.Load<DialogueManager>("DialogueWindow");
        DialogueWindow = Instantiate(window, body.position, Quaternion.identity);
        DontDestroyOnLoad(DialogueWindow.gameObject);
        DialogueWindow.transform.SetParent(body, false);
        DialogueWindow.transform.position = body.position + new Vector3(-150, 180, 0);
        var text = DialogueWindow.GetComponentInChildren<TextMeshProUGUI>();
        DialogueWindow.ShowDialogueLine("{object_field} {creature_canis}");
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Destroy(DialogueWindow.gameObject);
    }
}