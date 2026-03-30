using System;
using System.Collections.Generic;

[Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public List<QuestStep> steps;
}