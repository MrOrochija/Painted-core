using UnityEngine;

public class SettingSystem : MonoBehaviour
{
    public GameObject contentAudio;
    public GameObject contentGraphic;
    public GameObject contentControl;

    public void Audio()
    {
        contentAudio.SetActive(true);
        contentGraphic.SetActive(false);
        contentControl.SetActive(false);
    }

    public void Graphic()
    {
        contentAudio.SetActive(false);
        contentGraphic.SetActive(true);
        contentControl.SetActive(false);
    }

    public void Control()
    {
        contentAudio.SetActive(false);
        contentGraphic.SetActive(false);
        contentControl.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
