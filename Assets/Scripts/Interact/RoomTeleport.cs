using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class RoomTeleport : MonoBehaviour
{
    public string targetSceneName; 
    public string targetSpawnPointName;
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
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null) 
            {
                movement.currentState = PlayerState.Frozen;
            }

            yield return StartCoroutine(Fade(1));
            
            PlayerPrefs.SetString("NextSpawnPoint", targetSpawnPointName);
            PlayerPrefs.Save();

            SceneManager.LoadScene(targetSceneName);
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