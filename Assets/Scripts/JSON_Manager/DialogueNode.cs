using System;
using System.Collections.Generic;

[Serializable]
public class DialogueNode
{
    public string nodeId;
    public string speaker;
    public string text;
    public List<DialogueOption> options;
    public string nextNodeId;
    public List<QuestAction> actions;
}