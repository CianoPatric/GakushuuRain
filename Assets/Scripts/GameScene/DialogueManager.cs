using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogue;
    
    public Color newWordColor = Color.yellow;
    public Color knowWordColor = Color.gray;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDialogueLine(string rawText)
    {
        string processedText = ProcessText(rawText);
        dialogue.text = processedText;
    }

    private string ProcessText(string rawText)
    {
        string pattern = @"\{(\w+)}";
        string processed = Regex.Replace(rawText, pattern, match =>
        {
            string wordId = match.Groups[1].Value;
            string wordToDisplay = wordId.Split('_')[1];
            bool isKnown = (wordId == "object_field");

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
}