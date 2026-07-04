using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject battleZone;
    private GameObject zone;
    public GameObject selectFigure;
    
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

        if (battleZone != null)
        {
            Transform zoneTransform = battleZone.transform.Find("Zone");
            
            if (zoneTransform != null)
            {
                zone = zoneTransform.gameObject; 
            }
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
        selectFigure.SetActive(false);
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

        float minX = bounds.min.x + 0.5f;
        float maxX = bounds.max.x - 0.5f;
        float minY = bounds.min.y + 0.5f;
        float maxY = bounds.max.y - 0.5f;

        if (minX > maxX) minX = maxX = bounds.center.x;
        if (minY > maxY) minY = maxY = bounds.center.y;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY); 
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}