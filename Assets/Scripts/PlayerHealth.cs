using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public char up = 'w';
    public char down = 's';
    public char left = 'a';
    public char right = 'd';
    public char interact = 'e';
    
    private int maxHealth = 200;
    private int maxMana = 100;

    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }
    public int MaxHealth => maxHealth;
    public int MaxMana => maxMana;

    public FadeModule fadeModule;
    public GameObject currentCheckpoint;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        CurrentHealth = maxHealth;
        CurrentMana = 0;
    }

    public void GetDamage(int damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
    }

    public void Heal(int heal)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + heal);
    }

    public void HealMax() => CurrentHealth = maxHealth;

    public bool UseMana(int amount)
    {
        if (CurrentMana < amount) return false;
        
        CurrentMana -= amount;
        return true;
    }

    public void UseManaMax() => CurrentMana = 0;

    public void RestoreMana(int amount)
    {
        CurrentMana = Mathf.Min(maxMana, CurrentMana + amount);
    }

    public IEnumerator Death()
    {
        yield return StartCoroutine(fadeModule.Fade(1));
        
        transform.position = currentCheckpoint.transform.position;
        HealMax();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(fadeModule.Fade(0));

        playerMovement.currentState = PlayerState.Free;
    }
}