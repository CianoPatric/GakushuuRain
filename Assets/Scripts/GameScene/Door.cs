using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string targetSceneName;
    public Vector2 targetPosition;

    public string doorId;
    public GameObject pressEIndicator;

    private GameEntryPoint _gameEntryPoint;

    public void Initialize(GameEntryPoint gameEntryPoint)
    {
        _gameEntryPoint = gameEntryPoint;
    }

    public void Interact()
    {
        Debug.Log($"Взаимодействие с дверью, которая ведёт в {targetSceneName}");
        _gameEntryPoint.TransitionToScene(targetSceneName, targetPosition);
    }

    public void ShowIndicator(bool show)
    {
        if(pressEIndicator != null) pressEIndicator.SetActive(show);
    }
}