using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WordLibraryManager : MonoBehaviour
{
    public static WordLibraryManager instance {get; private set;}
    private Dictionary<string, WordData> wordLibrary = new();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadWordLibrary();
    }

    private void LoadWordLibrary()
    {
        TextAsset jsonfile = Resources.Load<TextAsset>("WordLibrary_EN");
        if (jsonfile == null)
        {
            Debug.LogError("Не найден JSON-файл");
            return;
        }
        WordLibrary library = JsonUtility.FromJson<WordLibrary>(jsonfile.text);
        wordLibrary = library.words.ToDictionary(word => word.id);
        Debug.Log($"Библиотека загружена, в ней записанно слов: {wordLibrary.Count}");
    }

    public WordData GetWordData(string wordId)
    {
        if (wordLibrary.ContainsKey(wordId))
        {
            return wordLibrary[wordId];
        }
        Debug.LogError($"{wordId} не найден в библиотеке слов");
        return null;
    }
}