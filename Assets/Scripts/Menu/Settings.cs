using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject settings;

    public void OnButtonClick()
    {
        settings.SetActive(true);
    }
}
