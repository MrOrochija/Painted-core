using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeModule : MonoBehaviour
{
    private Image fadeImage;

    void Start()
    {
        fadeImage = gameObject.GetComponent<Image>();
    }
    
    public IEnumerator Fade(float targetAlpha)
    {
        float speed = 1f / 0.1f;
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
