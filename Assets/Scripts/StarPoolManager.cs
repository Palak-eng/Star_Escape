using System.Collections.Generic;
using UnityEngine;

public class StarPoolManager : MonoBehaviour
{
    public GameObject starPrefab;
    public int poolSize = 10;
    public int maxActiveStars = 5;

    public Vector2 minSpawnArea = new Vector2(-7f, -3.5f);
    public Vector2 maxSpawnArea = new Vector2(7f, 3.5f);

    public float minDistanceFromPlayer = 2.5f;
    public float minDistanceBetweenStars = 1.5f;
    public int maxSpawnAttempts = 30;

    private List<GameObject> pool = new List<GameObject>();
    private Transform player;

    void Start()
    {
        CreatePool();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        for (int i = 0; i < maxActiveStars; i++)
        {
            SpawnStar();
        }
    }

    void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject star = Instantiate(starPrefab, transform);
            star.SetActive(false);

            StarCollectible collectible = star.GetComponent<StarCollectible>();
            if (collectible != null)
                collectible.SetPoolManager(this);

            pool.Add(star);
        }
    }

    public void SpawnStar()
    {
        GameObject star = GetInactiveStar();
        if (star == null) return;

        Vector2 spawnPos;
        bool foundValidPosition = TryGetValidSpawnPosition(out spawnPos);

        if (foundValidPosition)
        {
            star.transform.position = spawnPos;
            star.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Could not find a good spawn position for star.");
        }
    }

    GameObject GetInactiveStar()
    {
        foreach (GameObject star in pool)
        {
            if (!star.activeInHierarchy)
                return star;
        }
        return null;
    }

    bool TryGetValidSpawnPosition(out Vector2 validPosition)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector2 randomPos = GetRandomPosition();

            if (IsFarFromPlayer(randomPos) && IsFarFromOtherStars(randomPos))
            {
                validPosition = randomPos;
                return true;
            }
        }

        validPosition = Vector2.zero;
        return false;
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(minSpawnArea.x, maxSpawnArea.x);
        float y = Random.Range(minSpawnArea.y, maxSpawnArea.y);
        return new Vector2(x, y);
    }

    bool IsFarFromPlayer(Vector2 pos)
    {
        if (player == null) return true;

        return Vector2.Distance(pos, player.position) >= minDistanceFromPlayer;
    }

    bool IsFarFromOtherStars(Vector2 pos)
    {
        foreach (GameObject star in pool)
        {
            if (star.activeInHierarchy)
            {
                if (Vector2.Distance(pos, star.transform.position) < minDistanceBetweenStars)
                    return false;
            }
        }

        return true;
    }

    public void ReturnStar(GameObject star)
    {
        star.SetActive(false);
        SpawnStar();
    }
}