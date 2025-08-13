using UnityEngine;
using UnityEngine.EventSystems;

public class WireDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public WireColor wireColor;
    
    private RectTransform wireRect;
    private Canvas parentCanvas;
    private Vector2 initialPosition;
    private bool isConnected = false;
    private Transform connectedPoint = null;

    void Awake()
    {
        wireRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isConnected) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected) return;
        if (parentCanvas != null) Debug.Log("CANVAS FOUND");

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            null,
            out Vector2 localPoint
        );
        UpdateWire(localPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isConnected) return;
        
        GameObject droppedOnObject = eventData.pointerCurrentRaycast.gameObject;
        if (droppedOnObject != null)
        {
            WirePoint endPoint = droppedOnObject.GetComponent<WirePoint>();
            if (endPoint != null && !endPoint.isConnected && endPoint.pointColor == this.wireColor)
            {
                endPoint.isConnected = true;
                this.isConnected = true;
                connectedPoint = endPoint.transform;
                UpdateWire(connectedPoint.localPosition);
                Debug.Log("Connected " + wireColor + " wire!");
                return;
            }
        }

        ResetWire();
    }

    private void UpdateWire(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - wireRect.anchoredPosition;
        wireRect.sizeDelta = new Vector2(direction.magnitude, wireRect.sizeDelta.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        wireRect.localEulerAngles = new Vector3(0, 0, angle);
    }

    private void ResetWire()
    {
        wireRect.localEulerAngles = Vector3.zero;
        wireRect.sizeDelta = new Vector2(0, wireRect.sizeDelta.y);
    }

    private void Update()
    {
        if (isConnected && connectedPoint != null)
        {
            UpdateWire(connectedPoint.localPosition);
        }
    }
}