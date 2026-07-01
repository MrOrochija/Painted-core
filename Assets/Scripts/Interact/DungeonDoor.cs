using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DungeonDoor : InteractableObject 
{
    private Animator anim;
    private DungeonDoor targetScript;

    public Transform targetExitPoint;
    public Transform playerTransform;
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public bool coolDown = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (targetExitPoint != null)
        {
            targetScript = targetExitPoint.GetComponent<DungeonDoor>();
        }
    }

    public override void Interact()
    {
        if (!coolDown)
        {
            StartCoroutine(InteractRoutine());
        }
    }

    private IEnumerator InteractRoutine()
    {
        if (targetScript != null)
        {
            coolDown = true;
            targetScript.coolDown = true;
        }
        

        if (anim != null)
        {
            anim.SetBool("isOpen", true);
            yield return null;
            float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(1));
        }

        if (playerTransform != null && targetExitPoint != null)
        {
            playerTransform.position = targetExitPoint.position;
        }

        yield return new WaitForSeconds(0.2f);

        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(0));
        }

        if (anim != null) anim.SetBool("isOpen", false);
        
        yield return new WaitForSeconds(1.5f);
        
        if (targetScript != null)
        {
            coolDown = false;
            targetScript.coolDown = false;
        }
        
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float speed = 1f / fadeDuration;
        float currentAlpha = fadeImage.color.a;

        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, speed * Time.deltaTime);
            Color c = fadeImage.color;
            c.a = currentAlpha;
            fadeImage.color = c;
            yield return null;
        }
    }
}