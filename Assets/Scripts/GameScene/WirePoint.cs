using UnityEngine;

public class WirePoint : MonoBehaviour
{
    public WireColor pointColor;
    public bool isConnected = false;
}

public enum WireColor
{
    Red, Blue, Green, Yellow
}