using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class LightModule
{
    public static LightTrigger.LightingMode currentLightMode = LightTrigger.LightingMode.SetSunny;
    
    private static Coroutine currentRoutine;
    private static MonoBehaviour currentHost;

    public static void ChangeLight(MonoBehaviour host, LightTrigger.LightingMode mode, Light2D mainLight, Light2D playerLight)
    {
        if (currentRoutine != null && currentHost != null)
        {
            currentHost.StopCoroutine(currentRoutine);
        }

        currentLightMode = mode;
        currentHost = host;

        IEnumerator targetRoutine = mode switch
        {
            LightTrigger.LightingMode.SetDark => SetDark(mainLight, playerLight),
            LightTrigger.LightingMode.SetSunny => SetSunny(mainLight, playerLight),
            LightTrigger.LightingMode.LightOff => LightOff(mainLight, playerLight),
            _ => null
        };

        if (targetRoutine != null)
        {
            currentRoutine = currentHost.StartCoroutine(targetRoutine);
        }
    }

    public static IEnumerator SetDark(Light2D mainLight, Light2D playerLight, float duration = 1.0f)
    {
        if (mainLight == null || playerLight == null) yield break;

        playerLight.enabled = true;

        float startMain = mainLight.intensity;
        float targetMain = 0.01f;

        float startPlayer = playerLight.intensity;
        float targetPlayer = 1.0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            mainLight.intensity = Mathf.Lerp(startMain, targetMain, t);
            playerLight.intensity = Mathf.Lerp(startPlayer, targetPlayer, t);

            yield return null;
        }

        mainLight.intensity = targetMain;
        playerLight.intensity = targetPlayer;
    }

    public static IEnumerator SetSunny(Light2D mainLight, Light2D playerLight, float duration = 1.0f)
    {
        if (mainLight == null || playerLight == null) yield break;

        float startMain = mainLight.intensity;
        float targetMain = 1.0f;

        float startPlayer = playerLight.intensity;
        float targetPlayer = 0f;

        playerLight.enabled = true; 

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            mainLight.intensity = Mathf.Lerp(startMain, targetMain, t);
            playerLight.intensity = Mathf.Lerp(startPlayer, targetPlayer, t);

            yield return null;
        }

        mainLight.intensity = targetMain;
        playerLight.intensity = targetPlayer;
        playerLight.enabled = false;
    }

    public static IEnumerator LightOff(Light2D mainLight, Light2D playerLight, float duration = 1.0f)
    {
        if (mainLight == null || playerLight == null) yield break;

        float startMain = mainLight.intensity;
        float targetMain = 0f;

        float startPlayer = playerLight.intensity;
        float targetPlayer = 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            mainLight.intensity = Mathf.Lerp(startMain, targetMain, t);
            playerLight.intensity = Mathf.Lerp(startPlayer, targetPlayer, t);

            yield return null;
        }

        mainLight.intensity = targetMain;
        playerLight.intensity = targetPlayer;
        playerLight.enabled = false;
    }
}