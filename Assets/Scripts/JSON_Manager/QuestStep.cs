using System;
using System.Collections.Generic;

[Serializable]
public class QuestStep
{
    public int stepId;
    public string description;
    public string completionCondition;
    public List<QuestAction> rewards;
}