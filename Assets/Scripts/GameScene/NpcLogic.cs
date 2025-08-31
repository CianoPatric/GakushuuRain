using System;
using TMPro;
using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    private Transform body;
    private DialogueManager DialogueWindow;
    public GameObject PressEWindow;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        PressEWindow.SetActive(true);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKey(KeyCode.E))
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
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        PressEWindow.SetActive(false);
    }
}