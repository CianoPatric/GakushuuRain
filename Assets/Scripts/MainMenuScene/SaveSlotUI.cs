using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Button selectButton;
    public Button deleteButton;
    public TextMeshProUGUI infoText;

    public UnityEvent OnSelect;
    public UnityEvent OnDelete;

    void Awake()
    {
        selectButton.onClick.AddListener(() => OnSelect?.Invoke());
        deleteButton.onClick.AddListener(() => OnDelete?.Invoke());
    }

    public void Display(int slotIndex, PlayerData data)
    {
        infoText.text = $"Слот {slotIndex + 1}\n{data.profile.playerName}";
        selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Продолжить";
        deleteButton.gameObject.SetActive(true);
    }

    public void DisplayEmpty(int slotIndex)
    {
        infoText.text = $"Слот {slotIndex + 1}\n(Пусто)";
        selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Новая игра";
        deleteButton.gameObject.SetActive(false);
    }
}