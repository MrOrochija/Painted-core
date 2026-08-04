using System.Collections;
using UnityEngine;

public class DungeonDoor : InteractableObject
{
    private Animator anim;
    private DungeonDoor targetScript;

    public Transform targetExitPoint;
    public Transform playerTransform;
    private PlayerMovement playerMovement;
    public FadeModule fadeModule;
    [HideInInspector] public bool coolDown = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (targetExitPoint != null)
        {
            targetScript = targetExitPoint.GetComponent<DungeonDoor>();
        }

        if (playerTransform != null)
        {
            playerMovement = playerTransform.GetComponent<PlayerMovement>();
        }
    }

    public override void Interact()
    {
        if (!coolDown)
        {
            coolDown = true;
            
            playerMovement.currentState = PlayerState.Frozen;
            
            StartCoroutine(InteractRoutine());
        }
    }

    private IEnumerator InteractRoutine()
    {
        if (targetScript != null)
        {
            targetScript.coolDown = true;
        }
        
        if (anim != null)
        {
            anim.SetBool("isOpen", true);
            yield return null;
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
        }

        yield return StartCoroutine(fadeModule.Fade(1));

        if (playerTransform != null && targetExitPoint != null)
        {
            playerTransform.position = targetExitPoint.position;
        }

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(fadeModule.Fade(0));

        if (playerMovement.currentState != PlayerState.Combat)
        {
            playerMovement.currentState = PlayerState.Free;
        }

        if (anim != null) anim.SetBool("isOpen", false);
        
        yield return new WaitForSeconds(1.5f);
        
        if (targetScript != null)
        {
            coolDown = false;
            targetScript.coolDown = false;
        }
    }
}