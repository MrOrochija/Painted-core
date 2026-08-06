using UnityEngine;

public class Play : MonoBehaviour
{
    public MenuSystem menuSystem;

    public void OnButtonClick()
    {
        menuSystem.Play();
    }
}
