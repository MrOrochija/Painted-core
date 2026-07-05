using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject playerModel;
    public GameObject battleZone;
    public GameObject selectFigure;
    public GameObject heartBar;
    public GameObject HotBar;

    public GameObject Line;
    public GameObject Circle;
    public GameObject Triangle;
    public GameObject Square;

    private EnemyInteract enemyScript;
    private GameObject zone;
    private HotBar HotBarScript;
    private Collider2D zoneCollider;
    private GameObject heartFrame;

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

        if (heartBar != null)
        {
            Transform heartFrameTransform = heartBar.transform.Find("Image");

            if (heartFrameTransform != null)
            {
                heartFrame = heartFrameTransform.gameObject;
            }
        }
    }

    public IEnumerator startBattle(EnemyInteract eScript)
    {
        enemyScript = eScript;

        yield return new WaitForSeconds(2f); 

        for (int i = 0; i < 5; i++)
        {
            GameObject currentFigure = spawnFigure();

            yield return new WaitForSeconds(3f);

            if (currentFigure != null)
            {
                yield return StartCoroutine(MoveAndDestroyFigure(currentFigure));

                if (heartFrame != null)
                {
                    float targetX = Mathf.Max(0f, heartFrame.transform.localScale.x - 0.1f);
                    
                    yield return StartCoroutine(SmoothDecreaseHeart(targetX));
                }
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
        HotBarScript.setIsFightingForHeartMonitoring(false);
    }

    private IEnumerator SmoothDecreaseHeart(float targetX)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        
        Vector3 startScale = heartFrame.transform.localScale;
        Vector3 targetScale = new Vector3(targetX, startScale.y, startScale.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            heartFrame.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            
            yield return null;
        }

        heartFrame.transform.localScale = targetScale;
    }

    private IEnumerator MoveAndDestroyFigure(GameObject figure)
    {
        float currentSpeed = 1f;
        float acceleration = 100f;
        float closeEnoughDistance = 0.2f;

        while (figure != null && playerModel != null && 
               Vector3.Distance(figure.transform.position, playerModel.transform.position) > closeEnoughDistance)
        {
            currentSpeed += acceleration * Time.deltaTime;

            figure.transform.position = Vector3.MoveTowards(
                figure.transform.position, 
                playerModel.transform.position, 
                currentSpeed * Time.deltaTime
            );

            yield return null; 
        }

        if (figure != null)
        {
            Destroy(figure);
        }
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