using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueTextClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI text;
    private Canvas canvas;
    private DialogueManager saveDialogueManager;

    public void Initialize(DialogueManager dialogueManager)
    {
        saveDialogueManager = dialogueManager;
    }
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, canvas.worldCamera);
        
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            
            saveDialogueManager.OnWordClicked(linkId);
        }
    }
}