using System;
using System.Collections.Generic;

[Serializable]
public class DialogueNote
{
    public string noteId;
    public string speaker;
    public string text;
    public List<DialogueOption> options;
    public string nextNoteId;
    public List<QuestAction> actions;
}