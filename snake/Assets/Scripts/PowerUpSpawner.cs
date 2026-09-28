using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private PowerUp[] powerUpPrefabs;
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField] private Snake snake;
    [SerializeField] private Transform food;

    [SerializeField] private float minSpawnDelay = 8f;
    [SerializeField] private float maxSpawnDelay = 15f;

    [Header("Golden Food")]
    [SerializeField] private GoldenFood goldenFoodPrefab;

    private GoldenFood currentGolden;
    private PowerUp currentPowerUp; // max one power-up at a time
    private float timer;

    private void Start()
    {
        timer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (currentPowerUp != null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Spawn();
            timer = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    private void Spawn()
    {
        PowerUp prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        currentPowerUp = Instantiate(prefab, GetFreePosition(), Quaternion.identity);
    }

    private Vector3 GetFreePosition()
    {
        Bounds b = spawnArea.bounds;
        int x, y;

        do
        {
            x = Mathf.RoundToInt(Random.Range(b.min.x, b.max.x));
            y = Mathf.RoundToInt(Random.Range(b.min.y, b.max.y));
        }
        while (snake.Occupies(x, y) ||
           IsOnField(food, x, y) ||
           IsOnField(currentPowerUp, x, y) ||
           IsOnField(currentGolden, x, y));

        return new Vector3(x, y, 0f);
    }

    public void SpawnGoldenFood()
    {
        if (currentGolden != null) return;

        currentGolden = Instantiate(goldenFoodPrefab, GetFreePosition(), Quaternion.identity);
    }

    private bool IsOnField(Component obj, int x, int y)
    {
        return obj != null && 
            Mathf.RoundToInt(obj.transform.position.x) == x && 
            Mathf.RoundToInt(obj.transform.position.y) == y;
    }
}
