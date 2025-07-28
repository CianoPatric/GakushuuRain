using System;
using System.Collections.Generic;

[Serializable]
public class NotebookEntry
{
    public string wordId;
    public string userGuess;
    public WordStatus status;
    public List<string> encounteredContext;
}