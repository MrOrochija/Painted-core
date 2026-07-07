using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    private BattleSystem battleSystem;
    private Sprite[] figures;

    private GameObject plrFigure;
    private GameObject enemyFigure;
    private GameObject zone;
    private Collider2D zoneCollider;

    private bool active = false;
    
    private float spawnCooldown = 5.0f;
    private float spawnTimer = 0f;

    void Start()
    {
        battleSystem = gameObject.GetComponent<BattleSystem>();

        figures = battleSystem.figures;
        zone = battleSystem.zone;

        if (zone != null)
        {
            zoneCollider = zone.GetComponent<Collider2D>();
        }

        Transform plrFigureTransform = transform.Find("PlrFigure");
        if (plrFigureTransform != null) plrFigure = plrFigureTransform.gameObject;

        Transform enemyFigureTransform = transform.Find("EnemyFigure");
        if (enemyFigureTransform != null) enemyFigure = enemyFigureTransform.gameObject;
    }

    void Update()
    {
        if (!active) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnRandomFigureInZone();
            spawnTimer = spawnCooldown;
        }
    }

    private void SpawnRandomFigureInZone()
    {
        int randomSpriteIndex = Random.Range(0, figures.Length);
        Sprite randomSprite = figures[randomSpriteIndex];

        Bounds bounds = zoneCollider.bounds;

        float minX = bounds.min.x + 0.5f;
        float maxX = bounds.max.x - 0.5f;
        float minY = bounds.min.y + 0.5f;
        float maxY = bounds.max.y - 0.5f;

        if (minX > maxX) minX = maxX = bounds.center.x;
        if (minY > maxY) minY = maxY = bounds.center.y;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector2 randomCoordinates = new Vector2(randomX, randomY);

        SpawnFigure(randomSprite, randomCoordinates, "Enemy");
    }

    public void SpawnFigure(Sprite figureSprite, Vector2 coordinates, string owner)
    {
        GameObject prefab = owner == "Player" ? plrFigure : enemyFigure;

        GameObject newFigure = Instantiate(prefab, coordinates, Quaternion.identity);
        SpriteRenderer spriteRenderer = newFigure.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = figureSprite;
        }

        newFigure.SetActive(true);
    }

    public void Activate()
    {
        active = true;
        spawnTimer = 0f;
    }

    public void Deactivate()
    {
        active = false;
    }
}