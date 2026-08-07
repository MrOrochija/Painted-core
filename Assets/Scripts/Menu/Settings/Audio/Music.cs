using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour
{
    public Slider slider;
    public TMP_Text value;

    public void ChangeValue()
    {
        float currentValue = Mathf.Round(slider.value);
        value.text = currentValue + "%";
    }
}
