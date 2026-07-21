using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class LightModule
{
    public static void SetDark(Light2D mainLight, Light2D playerLight)
    {
        mainLight.intensity = 0.01f;
        playerLight.enabled = true;
    }

    public static void SetSunny(Light2D mainLight, Light2D playerLight)
    {
        
    }

    public static void LightOff(Light2D mainLight, Light2D playerLight)
    {
        mainLight.intensity = 1f;
        playerLight.enabled = false;
    }
}
