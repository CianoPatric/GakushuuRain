using System;
using System.Collections.Generic;

[Serializable]
public class DialogueOption
{
    public string optionId;
    public string text;
    public string nextNoteId;
    public List<QuestAction> actions;
}