using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueTextClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private Canvas _canvas;
    private DialogueManager _dialogueManager;

    public void Initialize(DialogueManager dialogueManager)
    {
        _dialogueManager = dialogueManager;
        _text = GetComponent<TextMeshProUGUI>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, _canvas.worldCamera);
        
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            
            _dialogueManager.OnWordClicked(linkId);
        }
    }
}