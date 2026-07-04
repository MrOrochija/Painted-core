using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInteract : MonoBehaviour
{
    private PlayerMovement plrMovementScript;
    private bool canCatch = true;

    public Image fadeImage;
    public float fadeDuration = 0.1f;

    public GameObject player;
    public GameObject battleZone;

    void Start()
    {
        if (player == null || battleZone == null || fadeImage == null)
        {
            Debug.LogError($"[EnemyInteract] Забыли привязать объекты в инспекторе на {gameObject.name}!");
            return;
        }

        plrMovementScript = player.GetComponent<PlayerMovement>();
        if (plrMovementScript == null)
        {
            Debug.LogError("[EnemyInteract] На объекте Player нет компонента PlayerMovement!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canCatch && other.CompareTag("Player"))
        {
            StartCoroutine(InteractRoutine());
        }
    }

    private IEnumerator InteractRoutine()
    {
        canCatch = false;
        yield return StartCoroutine(Fade(1));
        
        plrMovementScript.canMove = false;
        player.transform.position = battleZone.transform.position;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0));

        HotBar hotBar = FindAnyObjectByType<HotBar>();
        if (hotBar != null)
        {
            hotBar.isBattleActive = true;
            hotBar.enemyScript = this;
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

    public IEnumerator leave()
    {
        yield return StartCoroutine(Fade(1));
        yield return new WaitForSeconds(1f);

        player.transform.position = gameObject.transform.position;
        plrMovementScript.canMove = true;

        yield return StartCoroutine(Fade(0));

        yield return new WaitForSeconds(5f);

        canCatch = true;
    }
}