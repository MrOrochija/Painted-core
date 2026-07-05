using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFigures : MonoBehaviour
{
    public Image plrHeartFrame;
    public Image enemyHeartFrame;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer srOther = other.GetComponent<SpriteRenderer>();

            if (sr != null && srOther != null && sr.sprite != null && srOther.sprite != null)
            {
                string myName = sr.sprite.name;
                string otherName = srOther.sprite.name;

                bool damageDealt = false;
                bool shouldDestroy = false;
                Image targetHeart = null;

                if (myName == "line")
                {
                    if (otherName == "circle")
                    {
                        targetHeart = plrHeartFrame;
                        damageDealt = true;
                    }
                    else if (otherName == "square")
                    {
                        shouldDestroy = true;
                    }
                }

                else if (myName == "circle")
                {
                    if (otherName == "line" || otherName == "triangle")
                    {
                        targetHeart = enemyHeartFrame;
                        damageDealt = true;
                    }
                }

                else if (myName == "triangle")
                {
                    if (otherName == "circle" || otherName == "square")
                    {
                        targetHeart = plrHeartFrame;
                        damageDealt = true;
                    }
                }

                else if (myName == "square")
                {
                    if (otherName == "line")
                    {
                        shouldDestroy = true;
                    }
                    else if (otherName == "triangle")
                    {
                        targetHeart = enemyHeartFrame;
                        damageDealt = true;
                    }
                }

                if (damageDealt && targetHeart != null)
                {
                    TakeDamage(targetHeart);
                    StartCoroutine(HideAndDestroy(true));
                }
                else if (shouldDestroy)
                {
                    StartCoroutine(HideAndDestroy(false));
                }
            }
        }
    }

    private void TakeDamage(Image heartFrame)
    {
        if (heartFrame != null)
        {
            float targetX = Mathf.Max(0f, heartFrame.transform.localScale.x - 0.1f);
            StartCoroutine(SmoothDecreaseHeart(heartFrame, targetX));
        }
    }

    private IEnumerator HideAndDestroy(bool waitForUi)
    {
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        if (TryGetComponent<SpriteRenderer>(out var sr)) sr.enabled = false;

        if (waitForUi)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator SmoothDecreaseHeart(Image heartFrame, float targetX)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        
        Vector3 startScale = heartFrame.transform.localScale;
        Vector3 targetScale = new Vector3(targetX, startScale.y, startScale.z);

        while (elapsedTime < duration)
        {
            if (heartFrame == null) yield break;

            elapsedTime += Time.deltaTime;
            heartFrame.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            
            yield return null;
        }

        if (heartFrame != null)
        {
            heartFrame.transform.localScale = targetScale;
        }
    }
}