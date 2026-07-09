using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneBootstrapper : MonoBehaviour
{
    public Image fadeImage;

    void Start()
    {
        if (PlayerPrefs.HasKey("NextSpawnPoint"))
        {
            string pointName = PlayerPrefs.GetString("NextSpawnPoint");

            GameObject spawnPoint = GameObject.Find(pointName);
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (spawnPoint != null && player != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }

            PlayerPrefs.DeleteKey("NextSpawnPoint");
        }

        if (fadeImage != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        float speed = 1f / 0.5f;
        while (!Mathf.Approximately(fadeImage.color.a, 0f))
        {
            c.a = Mathf.MoveTowards(fadeImage.color.a, 0f, speed * Time.deltaTime);
            fadeImage.color = c;
            yield return null;
        }
    }
}