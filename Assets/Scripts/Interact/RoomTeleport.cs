using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomTeleport : MonoBehaviour
{
    public Transform exitTarget;
    public Image fadeImage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MovePlayer(other.gameObject));
        }
    }

    private IEnumerator MovePlayer(GameObject player)
    {
        if (exitTarget != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            
            if (movement != null) 
            {
                movement.canMove = false;
            }

            yield return StartCoroutine(Fade(1));
            
            player.transform.position = exitTarget.position;

            yield return new WaitForSeconds(0.2f);
            
            yield return StartCoroutine(Fade(0));

            if (movement != null) 
            {
                movement.canMove = true;
            }
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float speed = 1f / 0.5f;
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