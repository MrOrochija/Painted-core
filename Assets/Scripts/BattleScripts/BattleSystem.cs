using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class BattleFigure
{
    public string name;
    public Sprite sprite;
    public int manaCost;
}

public enum BattleActionType 
{ 
    Fight = 1, 
    ToggleUI = 2, 
    Flee = 3 
}

public class BattleSystem : SoundsModule
{
    public GameObject player;
    public GameObject bPlayer;
    public GameObject checkpointSystem;
    public GameObject invSystem;
    public GameObject battleZone;
    public GameObject zone; 
    public FadeModule fadeModule;

    public Canvas UI;
    public Canvas dialogue;
    public RectTransform playerManaBarRect;
    public RectTransform playerHealthBarRect;
    public RectTransform enemyHealthBarRect;
    public TextMeshProUGUI playerHP; 
    public TextMeshProUGUI enemyHP;

    [HideInInspector] public BattleFigure[] figures;

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private EnemyTrigger enemyTrigger;
    private EnemyHealth enemyHealth;
    private SelectAction selectAction;
    private SelectFigure selectFigure;
    private FigureSpawner figureSpawner;
    private SetCheckpoint setCheckpoint;
    private InventorySystem inventorySystem;
    
    private bool coolDown = false;
    private Animator bPlayerAnim;

    private LayerMask zoneLayerMask;
    private Camera mainCamera;
    private Dictionary<RectTransform, Coroutine> barAnimations = new Dictionary<RectTransform, Coroutine>();

    void Awake()
    {
        figureSpawner = GetComponent<FigureSpawner>();
        selectAction = GetComponent<SelectAction>();
        selectFigure = GetComponent<SelectFigure>();
        
        if (checkpointSystem) setCheckpoint = checkpointSystem.GetComponent<SetCheckpoint>();
        if (invSystem) inventorySystem = invSystem.GetComponent<InventorySystem>();

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
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        if (bPlayer != null) bPlayerAnim = bPlayer.GetComponent<Animator>();
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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) 
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Pointer.current.position.ReadValue()
                };
                
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0 && results[0].gameObject != zone) return;
            }

            DetectZoneClick();
        }
    }

    private void DetectZoneClick()
    {
        if (coolDown || figures == null || figures.Length == 0 || zone == null) return;

        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z));

        Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPosition, zoneLayerMask);

        if (hitCollider != null && hitCollider.gameObject == zone)
        {
            int index = Mathf.Clamp(selectFigure.currentFigureIndex, 0, figures.Length - 1);
            BattleFigure selectedFigure = figures[index];

            if (selectedFigure.manaCost > 0 && !UseMana(selectedFigure.manaCost)) return;

            coolDown = true;
            StartCoroutine(CoolDown(1f));

            if (selectedFigure.name == "Line" || selectedFigure.name == "Triangle")
            {
                StartCoroutine(TriggerAnim("Attack"));
            } 
            else
            {
                StartCoroutine(TriggerAnim("Block"));
            }

            PlaySound(sounds[index]);
            figureSpawner.SpawnFigure(selectedFigure, mouseWorldPosition, "Player");
        }
    }

    private IEnumerator TriggerAnim(string paramName)
    {
        bPlayerAnim.SetBool(paramName, true);
        yield return new WaitForSeconds(0.1f);
        bPlayerAnim.SetBool(paramName, false);
    }

    private IEnumerator CoolDown(float delay)
    {
        yield return new WaitForSeconds(delay);
        coolDown = false;
    }

    public void StartBattle(EnemyTrigger script, EnemyHealth script2)
    {
        enemyTrigger = script;
        enemyHealth = script2;

        if (dialogue) dialogue.enabled = false;

        inventorySystem.Deactivate();
        setCheckpoint.Deactivate();
        selectAction.Activate();
        InitializeBars();
    }

    public void SelectAction(BattleActionType action)
    {
        switch (action)
        {
            case BattleActionType.Fight:
                UI.enabled = false;
                dialogue.enabled = false;
                selectAction.Deactivate();
                battleZone.SetActive(true);
                selectFigure.Activate();
                figureSpawner.Activate();
                break;

            case BattleActionType.ToggleUI:
                UI.enabled = !UI.enabled;
                break;

            case BattleActionType.Flee:
                UI.enabled = false;
                dialogue.enabled = false;
                selectAction.Deactivate();

                if (Random.value <= 0.35f)
                {
                    StartCoroutine(enemyTrigger.RunAway());
                    setCheckpoint.Activate();
                    inventorySystem.Activate();
                    if (playerMovement != null) playerMovement.currentState = PlayerState.Free;
                } 
                else
                {
                    PlayerGetDamage(20);
                    battleZone.SetActive(true);
                    selectFigure.Activate();
                    figureSpawner.Activate();
                }
                break;
        }
    }

    public void PlayerGetDamage(int damage)
    {
        if (playerHealth == null) return;

        playerHealth.GetDamage(damage);
        SafeAnimateBar(playerHealthBarRect, playerHealth.CurrentHealth, playerHealth.MaxHealth, playerHP);

        if (playerHealth.CurrentHealth <= 0)
        {
            if (figureSpawner != null) figureSpawner.Deactivate();

            PlayerDeath();
            if (enemyTrigger != null)
            {
                selectAction.Deactivate();
                selectFigure.Deactivate();
                figureSpawner.Deactivate();
                enemyHealth.HealMax();
                
                playerHealth.HealMax();
                playerHealth.UseManaMax();
                
                setCheckpoint.Activate();
                inventorySystem.Activate();

                if (playerMovement != null) playerMovement.currentState = PlayerState.Free;
            }
        }
    }

    public void EnemyGetDamage(int damage)
    {
        if (enemyHealth == null) return;

        enemyHealth.GetDamage(damage);
        SafeAnimateBar(enemyHealthBarRect, enemyHealth.currentHealth, enemyHealth.maxHealth, enemyHP);

        if (enemyHealth.currentHealth <= 0)
        {
            if (figureSpawner != null) figureSpawner.Deactivate();

            if (enemyTrigger != null)
            {
                StartCoroutine(enemyTrigger.EnemyDead());
                setCheckpoint.Activate();
                inventorySystem.Activate();
                if (playerMovement != null) playerMovement.currentState = PlayerState.Free;
            }
        }
    }

    public void PlayerHeal(int healAmount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount); 
            SafeAnimateBar(playerHealthBarRect, playerHealth.CurrentHealth, playerHealth.MaxHealth, playerHP);
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

    private void PlayerDeath()
    {
        StartCoroutine(playerHealth.Death());
        if (enemyTrigger != null) enemyTrigger.inBattle = false;
    }

    public bool UseMana(int amount)
    {
        if (playerHealth.UseMana(amount))
        {
            SafeAnimateBar(playerManaBarRect, playerHealth.CurrentMana, playerHealth.MaxMana, null);
            return true;
        }
        return false;
    }

    public void RestoreMana(int amount)
    {
        playerHealth.RestoreMana(amount);
        SafeAnimateBar(playerManaBarRect, playerHealth.CurrentMana, playerHealth.MaxMana, null);
    }

    private void InitializeBars()
    {
        if (playerHealth != null && playerHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)playerHealth.CurrentHealth / playerHealth.MaxHealth);
            playerHealthBarRect.localScale = new Vector3(targetXScale, playerHealthBarRect.localScale.y, 1f);
            if (playerHP != null) playerHP.text = playerHealth.CurrentHealth.ToString();
        }

        if (enemyHealth != null && enemyHealthBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)enemyHealth.currentHealth / enemyHealth.maxHealth);
            enemyHealthBarRect.localScale = new Vector3(targetXScale, enemyHealthBarRect.localScale.y, 1f);
            if (enemyHP != null) enemyHP.text = enemyHealth.currentHealth.ToString();
        }

        if (playerManaBarRect != null)
        {
            float targetXScale = Mathf.Clamp01((float)playerHealth.CurrentMana / playerHealth.MaxMana);
            playerManaBarRect.localScale = new Vector3(targetXScale, playerManaBarRect.localScale.y, 1f);
        }
    }

    private void SafeAnimateBar(RectTransform barRect, float currentValue, float maxValue, TextMeshProUGUI textComponent = null)
    {
        if (barRect == null) return;

        if (barAnimations.TryGetValue(barRect, out Coroutine activeAnimation) && activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }

        barAnimations[barRect] = StartCoroutine(AnimateBar(barRect, currentValue, maxValue, textComponent));
    }

    private IEnumerator AnimateBar(RectTransform barRect, float currentValue, float maxValue, TextMeshProUGUI textComponent)
    {
        Vector3 initialScale = barRect.localScale;
        float targetXScale = Mathf.Clamp01(currentValue / maxValue);
        Vector3 finalScale = new Vector3(targetXScale, initialScale.y, initialScale.z);

        float startValue = initialScale.x * maxValue; 
        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            barRect.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            
            if (textComponent != null)
            {
                textComponent.text = Mathf.RoundToInt(Mathf.Lerp(startValue, currentValue, progress)).ToString();
            }

            yield return null;
        }

        barRect.localScale = finalScale;
        if (textComponent != null) textComponent.text = Mathf.RoundToInt(currentValue).ToString();
        
        barAnimations[barRect] = null;
    }
}