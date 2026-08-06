using UnityEngine;

public class Settings : MonoBehaviour
{
    public MenuSystem menuSystem;

    public void OnButtonClick()
    {
        menuSystem.Settings();
    }
}
