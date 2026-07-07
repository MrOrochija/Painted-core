using System.Collections;
using UnityEngine;

public class PlrFigure : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Destruction());
    }

    private IEnumerator Destruction()
    {
        yield return new WaitForSeconds(1.0f);

        Destroy(gameObject);
    }
}
