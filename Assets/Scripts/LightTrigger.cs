using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightTrigger : MonoBehaviour
{
    public Light2D mainLight;
    public Light2D playerLight;

    public enum LightingMode 
    { 
        SetDark, 
        SetSunny,
        LightOff
    }

    public LightingMode mode;
    
    private LightingMode modeBeforeEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            modeBeforeEnter = LightModule.currentLightMode;

            LightModule.ChangeLight(this, mode, mainLight, playerLight);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LightModule.ChangeLight(this, modeBeforeEnter, mainLight, playerLight);
        }
    }
}