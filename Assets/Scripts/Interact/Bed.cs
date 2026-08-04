using UnityEngine;

public class Bed : InteractableObject
{
    public DialogueData dialogueData;
    public DialogueModule dialogueModule;

    private bool coolDown;

    public override void Interact()
    {
        if (!coolDown)
        {
            coolDown = true;
            
            dialogueModule.OnDialogueFinished += OnDialogueEnd;
            dialogueModule.StartDialogue(dialogueData);
        }
    }

    private void OnDialogueEnd()
    {
        coolDown = false;
    }
}
