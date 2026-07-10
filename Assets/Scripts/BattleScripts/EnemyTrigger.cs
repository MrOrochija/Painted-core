using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject battle;
    public GameObject player;
    public Image fadeImage;
    private GameObject battleZone;
    private GameObject zone;

    private BattleSystem battleSystem;
    private PlayerMovement plrMovement;
    private GameObject enemy;
    private bool inBattle = false;

    private SpriteRenderer enemySpriteRenderer;

    void Start()
    {
        if (battle != null) battleSystem = battle.GetComponent<BattleSystem>();
        if (player != null) plrMovement = player.GetComponent<PlayerMovement>();

        Transform battleZoneTransform = battle.transform.Find("BattleZone");

        if (battleZoneTransform != null)
        {
            battleZone = battleZoneTransform.gameObject;

            Transform zoneTransform = battleZone.transform.Find("Zone");

            if (zoneTransform != null)
            {
                zone = zoneTransform.gameObject;
            }
        }

        Transform enemyTransform = transform.parent;

        if (enemyTransform != null)
        {
            enemy = enemyTransform.gameObject;

            enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
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
        battleZone.SetActive(false);

        yield return StartCoroutine(Fade(1));

        if (zone != null)
        {
            player.transform.position = zone.transform.position;
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(0));

        Transform enemyTransform = transform.parent;

        if (enemyTransform != null)
        {
            EnemyHealth enemyHealth = enemyTransform.GetComponent<EnemyHealth>();

            battleSystem.StartBattle(this, enemyHealth);
        }
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

    public IEnumerator EnemyDead()
    {
        yield return StartCoroutine(Fade(1));

        player.transform.position = enemy.transform.position;
        SetEnemyAlpha(0);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(0));

        plrMovement.canMove = true;

        Destroy(enemy);
    }

    public IEnumerator PlayerDead()
    {
        yield return StartCoroutine(Fade(1));

        player.transform.position = enemy.transform.position;

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(0));

        plrMovement.canMove = true;

        yield return new WaitForSeconds(5f);

        inBattle = false;
    }

    private void SetEnemyAlpha(float alpha)
    {
        if (enemySpriteRenderer != null)
        {
            Color color = enemySpriteRenderer.color;
            color.a = alpha;
            enemySpriteRenderer.color = color;
        }
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
