using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class BattleFigure
{
    public string name;
    public Sprite sprite;
    public int manaCost;
}

public class BattleSystem : MonoBehaviour
{
    public GameObject player;
    private PlayerHealth playerHealth;
    private LayerMask zoneLayerMask;

    [HideInInspector] public BattleFigure[] figures;

    private EnemyTrigger enemyTrigger;
    private EnemyHealth enemyHealth;
    private SelectAction selectAction;
    private SelectFigure selectFigure;
    private FigureSpawner figureSpawner;
    public GameObject checkpointSystem;
    private SetCheckpoint setCheckpoint;

    public RectTransform playerManaBarRect;
    public RectTransform playerHealthBarRect;
    public RectTransform enemyHealthBarRect;
    
    public TextMeshProUGUI playerHP; 
    public TextMeshProUGUI enemyHP;

    private GameObject battleZone;
    [HideInInspector] public GameObject zone;

    private Camera mainCamera;
    private bool coolDown = false;

    private Dictionary<RectTransform, Coroutine> barAnimations = new Dictionary<RectTransform, Coroutine>();

    void Awake()
    {
        figureSpawner = gameObject.GetComponent<FigureSpawner>();
        selectAction = gameObject.GetComponent<SelectAction>();
        selectFigure = gameObject.GetComponent<SelectFigure>();
        setCheckpoint = checkpointSystem.GetComponent<SetCheckpoint>();

        figures = new BattleFigure[]
        {
            new BattleFigure { name = "Line", sprite = selectFigure.Line, manaCost = 0 },
            new BattleFigure { name = "Circle", sprite = selectFigure.Circle, manaCost = 0 },
            new BattleFigure { name = "Triangle", sprite = selectFigure.Triangle, manaCost = 50 },
            new BattleFigure { name = "Square", sprite = selectFigure.Square, manaCost = 50 }
        };

        zoneLayerMask = LayerMask.GetMask("BattleZone");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        Transform battleZoneTransform = transform.Find("BattleZone");
        if (battleZoneTransform != null)
        {
            battleZone = battleZoneTransform.gameObject;
            Transform zoneTransform = battleZone.transform.Find("Zone");

            if (zoneTransform != null)
            {
                zone = zoneTransform.gameObject;
            }
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitializeBars();
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            DetectZoneClick();
        }
    }

    private void DetectZoneClick()
    {
        if (coolDown || figures == null || figures.Length == 0 || zone == null) return;
        coolDown = true;

        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        
        Vector3 targetScreenPos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z);
        Vector3 mouseWorldPosition3D = mainCamera.ScreenToWorldPoint(targetScreenPos);
        Vector2 mouseWorldPosition = new Vector2(mouseWorldPosition3D.x, mouseWorldPosition3D.y);

        Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPosition, zoneLayerMask);

        if (hitCollider != null && hitCollider.gameObject == zone)
        {
            int index = Mathf.Clamp(selectFigure.currentFigureIndex, 0, figures.Length - 1);
            BattleFigure selectedFigure = figures[index];
            
            if (selectedFigure.manaCost > 0)
            {
                if (!UseMana(selectedFigure.manaCost))
                {
                    StartCoroutine(ResetCoolDown(1f));
                    return;
                }
            }

            figureSpawner.SpawnFigure(selectedFigure.sprite, mouseWorldPosition, "Player");
        }

        StartCoroutine(ResetCoolDown(1f));
    }

    private IEnumerator ResetCoolDown(float delay)
    {
        yield return new WaitForSeconds(delay);
        coolDown = false;
    }

    public void StartBattle(EnemyTrigger script, EnemyHealth script2)
    {
        if (script != null) enemyTrigger = script;
        if (script2 != null) enemyHealth = script2;

        setCheckpoint.Deactivate();
        selectAction.Activate();
        InitializeBars();
    }

    public void SelectAction(int action)
    {
        switch (action)
        {
            case 1:
                selectAction.Deactivate();
                battleZone.SetActive(true);
                selectFigure.Activate();
                figureSpawner.Activate();
                break;
            case 2:
                break;
            case 3:
                float randomValue = Random.value; 

                if (randomValue <= 0.35f)
                {
                    StartCoroutine(enemyTrigger.RunAway());
                    setCheckpoint.Activate();
                } else
                {
                    PlayerGetDamage(20);
                    selectAction.Deactivate();
                    battleZone.SetActive(true);
                    selectFigure.Activate();
                    figureSpawner.Activate();
                }
                break;
        }
    }

    public void PlayerGetDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.GetDamage(damage);
            SafeAnimateBar(playerHealthBarRect, playerHealth.currentHealth, playerHealth.maxHealth, playerHP);

            if (playerHealth.currentHealth <= 0)
            {
                if (figureSpawner != null) figureSpawner.Deactivate();

                if (enemyTrigger != null)
                {
                    StartCoroutine(enemyTrigger.PlayerDead()); 
                    selectAction.Deactivate();
                    selectFigure.Deactivate();
                    figureSpawner.Deactivate();
                    enemyHealth.HealMax();
                    playerHealth.HealMax();
                    playerHealth.UseManaMax();
                    setCheckpoint.Activate();
                }
            }
        }
    }

    public void EnemyGetDamage(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.GetDamage(damage);
            SafeAnimateBar(enemyHealthBarRect, enemyHealth.currentHealth, enemyHealth.maxHealth, enemyHP);

            if (enemyHealth.currentHealth <= 0)
            {
                if (figureSpawner != null) figureSpawner.Deactivate();

                if (enemyTrigger != null)
                {
                    StartCoroutine(enemyTrigger.EnemyDead());
                }
            }
        }
    }

    public void PlayerHeal(int healAmount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount); 
            SafeAnimateBar(playerHealthBarRect, playerHealth.currentHealth, playerHealth.maxHealth, playerHP);
        }
    }

    public void EnemyHeal(int healAmount)
    {
        if (enemyHealth != null)
        {
            enemyHealth.Heal(healAmount);
            SafeAnimateBar(enemyHealthBarRect, enemyHealth.currentHealth, enemyHealth.maxHealth, enemyHP);
        }
    }

    public bool UseMana(int amount)
    {
        if (playerHealth.currentMana >= amount)
        {
            playerHealth.UseMana(amount);
            SafeAnimateBar(playerManaBarRect, playerHealth.currentMana, playerHealth.maxMana, null);
            return true;
        }
        return false;
    }

    public void RestoreMana(int amount)
    {
        playerHealth.RestoreMana(amount);
        SafeAnimateBar(playerManaBarRect, playerHealth.currentMana, playerHealth.maxMana, null);
    }

    private void InitializeBars()
    {
        if (playerHealth != null && playerHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)playerHealth.currentHealth / playerHealth.maxHealth);
            playerHealthBarRect.localScale = new Vector3(targetXScale, playerHealthBarRect.localScale.y, playerHealthBarRect.localScale.z);
            if (playerHP != null) playerHP.text = $"{playerHealth.currentHealth}";
        }

        if (enemyHealth != null && enemyHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)enemyHealth.currentHealth / enemyHealth.maxHealth);
            enemyHealthBarRect.localScale = new Vector3(targetXScale, enemyHealthBarRect.localScale.y, enemyHealthBarRect.localScale.z);
            if (enemyHP != null) enemyHP.text = $"{enemyHealth.currentHealth}";
        }

        if (playerManaBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)playerHealth.currentMana / playerHealth.maxMana);
            playerManaBarRect.localScale = new Vector3(targetXScale, playerManaBarRect.localScale.y, playerManaBarRect.localScale.z);
        }
    }

    private void SafeAnimateBar(RectTransform barRect, float currentValue, float maxValue, TextMeshProUGUI textComponent = null)
    {
        if (barRect == null) return;

        if (barAnimations.TryGetValue(barRect, out Coroutine activeAnimation))
        {
            if (activeAnimation != null)
            {
                StopCoroutine(activeAnimation);
            }
        }

        barAnimations[barRect] = StartCoroutine(AnimateBar(barRect, currentValue, maxValue, textComponent));
    }

    private IEnumerator AnimateBar(RectTransform barRect, float currentValue, float maxValue, TextMeshProUGUI textComponent)
    {
        Vector3 initialScale = barRect.localScale;
        float targetXScale = Mathf.Clamp01(currentValue / maxValue);
        Vector3 finalScale = new Vector3(targetXScale, initialScale.y, initialScale.z);

        float startHealthValue = initialScale.x * maxValue; 

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            barRect.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            
            if (textComponent != null)
            {
                int animatedHealth = Mathf.RoundToInt(Mathf.Lerp(startHealthValue, currentValue, progress));
                textComponent.text = $"{animatedHealth}";
            }

            yield return null;
        }

        barRect.localScale = finalScale;
        
        if (textComponent != null)
        {
            textComponent.text = $"{Mathf.RoundToInt(currentValue)}";
        }
        
        barAnimations[barRect] = null;
    }
}