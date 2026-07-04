using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject zone;
    public GameObject battleZone;
    
    public GameObject HotBar;
    private HotBar HotBarScript;

    public GameObject Line;
    public GameObject Circle;
    public GameObject Triangle;
    public GameObject Square;

    private Collider2D zoneCollider;

    void Start()
    {
        if (Line == null || Circle == null || Triangle == null || Square == null)
        {
            return;
        }

        if (zone != null)
        {
            zoneCollider = zone.GetComponent<Collider2D>();
        }

        if (HotBar != null)
        {
            HotBarScript = HotBar.GetComponent<HotBar>();
        }
    }

    public IEnumerator startBattle()
    {
        yield return new WaitForSeconds(2f); 

        for (int i = 0; i < 5; i++)
        {

            GameObject currentFigure = spawnFigure();

            yield return new WaitForSeconds(5f);

            if (currentFigure != null)
            {
                Destroy(currentFigure);
            }

            if (i < 4) 
            {
                yield return new WaitForSeconds(1f);
            }
        }
        
        yield return new WaitForSeconds(1f);

        battleZone.SetActive(false);
        HotBarScript.isFighting = false;
    }

    public GameObject spawnFigure()
    {
        if (zoneCollider == null) return null;

        Vector3 spawnPosition = GetRandomPositionInZone();
        int randomInt = Random.Range(0, 4);

        GameObject newFigure = null;

        switch (randomInt)
        {
            case 0:
                newFigure = Instantiate(Line, spawnPosition, Quaternion.identity);
                break;
            case 1:
                newFigure = Instantiate(Circle, spawnPosition, Quaternion.identity);
                break;
            case 2:
                newFigure = Instantiate(Triangle, spawnPosition, Quaternion.identity);
                break;
            case 3:
                newFigure = Instantiate(Square, spawnPosition, Quaternion.identity);
                break;
        }

        return newFigure;
    }

    private Vector3 GetRandomPositionInZone()
    {
        Bounds bounds = zoneCollider.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y); 
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}