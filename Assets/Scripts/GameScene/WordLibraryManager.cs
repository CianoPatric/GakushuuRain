using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WordLibraryManager : MonoBehaviour
{
    private Dictionary<string, WordData> _wordLibrary = new();

    void Awake()
    {
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
        _wordLibrary = library.words.ToDictionary(word => word.id);
        Debug.Log($"Библиотека загружена, в ней записанно слов: {_wordLibrary.Count}");
    }

    public WordData GetWordData(string wordId)
    {
        if (_wordLibrary.ContainsKey(wordId))
        {
            return _wordLibrary[wordId];
        }
        Debug.LogError($"{wordId} не найден в библиотеке слов");
        return null;
    }
}