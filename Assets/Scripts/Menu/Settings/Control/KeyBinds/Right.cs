using TMPro;
using UnityEngine;

public class Right : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public AnyKey anyKeyScript;
    public TMP_Text text;

    public void OnButtonClick()
    {
        if (anyKeyScript != null)
        {
            anyKeyScript.OnKeyPressed -= UpdateTextAndHealth;
            
            anyKeyScript.OnKeyPressed += UpdateTextAndHealth;

            anyKeyScript.gameObject.SetActive(true);
        }
    }

    private void UpdateTextAndHealth(char pressedChar)
    {
        anyKeyScript.OnKeyPressed -= UpdateTextAndHealth;

        text.text = pressedChar.ToString();
        playerHealth.right = pressedChar;
    }

    private void OnDestroy()
    {
        if (anyKeyScript != null)
        {
            anyKeyScript.OnKeyPressed -= UpdateTextAndHealth;
        }
    }
}