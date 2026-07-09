using System.Collections;
using UnityEngine;

public enum FigureType { None, Line, Circle, Triangle, Square }

public class EnemyFigure : MonoBehaviour
{
    public GameObject battle;
    public GameObject player;
    
    [SerializeField] private FigureType enemyType;

    private BattleSystem battleSystem;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (battle != null)
        {
            battleSystem = battle.GetComponent<BattleSystem>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (enemyType == FigureType.None)
            enemyType = ParseFigureType(spriteRenderer.sprite?.name);

        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(FlyToPlayer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerAttack")) return;

        SpriteRenderer playerRenderer = other.GetComponent<SpriteRenderer>();
        FigureType playerType = ParseFigureType(playerRenderer?.sprite?.name);

        EvaluateCombat(playerType);
    }

    private void EvaluateCombat(FigureType playerType)
    {
        if (enemyType == FigureType.Line && playerType == FigureType.Square)
        {
            Destroy(gameObject); 
            return;
        }
        if (enemyType == FigureType.Line && playerType == FigureType.Circle)
        {
            PlayerGetDamage(10);
            RestoreMana(100);
            return;
        }
        if (enemyType == FigureType.Circle)
        {
            if (playerType == FigureType.Line) { EnemyGetDamage(10); return; }
            if (playerType == FigureType.Triangle) { EnemyGetDamage(20); return; }
        }
        if (enemyType == FigureType.Triangle)
        {
            if (playerType == FigureType.Circle) { PlayerGetDamage(20); RestoreMana(100); return; }
            if (playerType == FigureType.Square) { PlayerGetDamage(10); RestoreMana(100); return; }
        }
        if (enemyType == FigureType.Square && playerType == FigureType.Triangle)
        {
            EnemyGetDamage(10);
            return;
        }

        StartCoroutine(FlyToPlayer());
    }

    private IEnumerator FlyToPlayer()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            if (player == null) yield break; 

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; 
            float acceleratedT = t * t * t;

            transform.position = Vector3.Lerp(startPosition, player.transform.position, acceleratedT);
            yield return null;
        }

        if (player != null)
        {
            transform.position = player.transform.position;
            PlayerGetDamage(20);
        }
    }

    private void PlayerGetDamage(int damage)
    {
        battleSystem?.PlayerGetDamage(damage);
        Destroy(gameObject);
    }

    private void EnemyGetDamage(int damage)
    {
        battleSystem?.EnemyGetDamage(damage);
        Destroy(gameObject);
    }

    private void UseMana(int mana)
    {
        battleSystem?.UseMana(mana);
        Destroy(gameObject);
    }

    private void RestoreMana(int mana)
    {
        battleSystem?.RestoreMana(mana);
        Destroy(gameObject);
    }

    private FigureType ParseFigureType(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return FigureType.None;
        
        if (spriteName.Contains("line")) return FigureType.Line;
        if (spriteName.Contains("circle")) return FigureType.Circle;
        if (spriteName.Contains("triangle")) return FigureType.Triangle;
        if (spriteName.Contains("square")) return FigureType.Square;
        
        return FigureType.None;
    }
}