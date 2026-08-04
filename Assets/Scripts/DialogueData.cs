using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerSprite;
    [TextArea(2, 5)]
    public string text;
    public bool isNarrator;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}