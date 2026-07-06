using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject battle;
    public GameObject player;
    public Image fadeImage;
    public GameObject zone;

    private BattleSystem battleSystem;
    private PlayerMovement plrMovement;
    private GameObject enemy;
    private Collider2D enemyCollider;
    private bool inBattle = false;

    void Start()
    {
        if (battle != null) battleSystem = battle.GetComponent<BattleSystem>();
        if (player != null) plrMovement = player.GetComponent<PlayerMovement>();
    
        Transform enemyTransform = transform.parent;

        if (enemyTransform != null)
        {
            enemy = enemyTransform.gameObject;
            
            enemyCollider = GetComponent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartBattle(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryStartBattle(other);
    }

    private void TryStartBattle(Collider2D other)
    {
        if (other.CompareTag("Player") && !inBattle)
        {
            inBattle = true;
            StartCoroutine(InteractRoutine());
        }
    }

    private IEnumerator InteractRoutine()
    {
        plrMovement.canMove = false;

        yield return StartCoroutine(Fade(1));

        if (zone != null)
        {
            player.transform.position = zone.transform.position;
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(0));

        battleSystem.StartBattle(this);
    }

    public IEnumerator RunAway()
    {
        yield return StartCoroutine(Fade(1));

        player.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(0));

        plrMovement.canMove = true;

        yield return new WaitForSeconds(5f);

        inBattle = false;
    }

    private IEnumerator Fade(float targetAlpha)
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
