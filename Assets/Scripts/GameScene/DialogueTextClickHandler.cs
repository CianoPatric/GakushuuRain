using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueTextClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI text;
    private Canvas canvas;

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
            
            Debug.Log(linkId);
        }
    }
}