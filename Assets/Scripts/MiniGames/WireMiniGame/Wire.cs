using System;
using UnityEngine;

public class Wire: MonoBehaviour
{
    public SpriteRenderer wireSpot;
    private Vector3 startPos;

    public void Start()
    {
        startPos = transform.parent.position;
    }
    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        
        transform.position = pos;
        
        Vector3 direction = pos - startPos;
        transform.right = direction * transform.lossyScale.x;
        
        float dist = Vector2.Distance(startPos, pos);
        wireSpot.size = new Vector2(dist, wireSpot.size.y);
    }
}