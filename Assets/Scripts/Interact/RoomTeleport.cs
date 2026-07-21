using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomTeleport : MonoBehaviour
{
    public GameObject targetExitObject;
    public Image fadeImage;

    private RoomTeleport targetScript;
    private bool isPlayerInside = false;
    private Coroutine stayCoroutine;

    private static bool isTeleporting = false;

    void Start()
    {
        if (targetExitObject != null)
        {
            targetScript = targetExitObject.GetComponent<RoomTeleport>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;

            if (isTeleporting) return;

            StartCoroutine(TeleportRoutine(other.transform));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            if (stayCoroutine != null)
            {
                StopCoroutine(stayCoroutine);
                stayCoroutine = null;
            }
        }
    }

    public void OnPlayerArrived(Transform playerTransform)
    {
        isPlayerInside = true;

        if (stayCoroutine != null)
            StopCoroutine(stayCoroutine);

        stayCoroutine = StartCoroutine(StayTimerRoutine(playerTransform));
    }

    private IEnumerator StayTimerRoutine(Transform playerTransform)
    {
        yield return new WaitForSeconds(2f);

        if (isPlayerInside && !isTeleporting)
        {
            StartCoroutine(TeleportRoutine(playerTransform));
        }
    }

    private IEnumerator TeleportRoutine(Transform playerTransform)
    {
        isTeleporting = true;

        PlayerMovement playerMovement = playerTransform.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.currentState = PlayerState.Frozen;
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(1f));
        }

        playerTransform.position = targetExitObject.transform.position;

        yield return new WaitForSeconds(0.1f);

        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(0f));
        }

        if (playerMovement != null && playerMovement.currentState != PlayerState.Combat)
        {
            playerMovement.currentState = PlayerState.Free;
        }

        isTeleporting = false;

        targetScript.OnPlayerArrived(playerTransform);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        float speed = 1f / 0.25f;
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