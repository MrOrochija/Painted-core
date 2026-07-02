using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public GameObject circle;
    [SerializeField] private bool activeCircle;

    public GameObject square;
    [SerializeField] private bool activeSquare;

    public GameObject triangle;
    [SerializeField] private bool activeTriangle;

    public GameObject line;
    [SerializeField] private bool activeLine;

    private void Start()
    {
        RefreshObjects();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;

        RefreshObjects();
    }

    private void RefreshObjects()
    {
        if (circle != null) circle.SetActive(activeCircle);
        if (square != null) square.SetActive(activeSquare);
        if (triangle != null) triangle.SetActive(activeTriangle);
        if (line != null) line.SetActive(activeLine);
    }
}