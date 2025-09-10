using System;
using System.Collections.Generic;

[Serializable]
public class QuestData
{
    public string questId;
    public string title;
    public string description;
    public List<QuestStep> steps;
}