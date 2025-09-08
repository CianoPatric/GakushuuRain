using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    public GameObject DialogueBox;
    public TextMeshProUGUI dialogue;
    private string currentRawText;
    
    public Color newWordColor = Color.yellow;
    public Color knowWordColor = Color.black;

    void Awake()
    {
        Instance = this;
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false);
        }
    }

    public void ShowDialogueLine(string rawText)
    {
        currentRawText = rawText;
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(true);
            string processedText = ProcessText(rawText);
            dialogue.text = processedText;
        }
        else
        {
            Debug.Log("Приложение не видит диалоговое окно");
        }
    }

    public void RedRawDL()
    {
        if (DialogueBox.activeSelf && !string.IsNullOrEmpty(currentRawText))
        {
            string processedText = ProcessText(currentRawText);
            dialogue.text = processedText;
        }
    }
    public void HideDialogueLine()
    {
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false);
        }
    }

    private string ProcessText(string rawText)
    {
        string pattern = @"\{([\w_]+)\}";
        string processed = Regex.Replace(rawText, pattern, match =>
        {
            string wordId = match.Groups[1].Value;
            
            WordData wordData = WordLibraryManager.instance.GetWordData(wordId);
            if (wordData == null) return "Слово не было найдено";
            
            string wordToDisplay = wordData.text;
            bool isKnown = DataManager.Instance.IsWordInNotebook(wordId);

            string colorHex;
            if (isKnown)
            {
                colorHex = ColorUtility.ToHtmlStringRGB(knowWordColor);
            }
            else
            {
                colorHex = ColorUtility.ToHtmlStringRGB(newWordColor);
            }

            return $"<link=\"{wordId}\"><color=#{colorHex}><u>{wordToDisplay}</u></color></link>";
        });
        return processed;
    }

    public void OnWordClicked(string wordId)
    {
        bool isKnown = DataManager.Instance.IsWordInNotebook(wordId);
        if (isKnown)
        {
            NotebookManager.instance.OpenAndShowWord(wordId);
            Debug.Log("Блокнот открыт");
        }
        else
        {
            DataManager.Instance.AddWordToNotebook(wordId);
            RedRawDL();
            Debug.Log("Слово добавленно");
        }
    }
}