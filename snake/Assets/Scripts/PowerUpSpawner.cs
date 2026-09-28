using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private PowerUp[] powerUpPrefabs;
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField] private Snake snake;
    [SerializeField] private Transform food;

    [SerializeField] private float minSpawnDelay = 8f;
    [SerializeField] private float maxSpawnDelay = 15f;

    private PowerUp current; // max one power-up at a time
    private float timer;

    private void Start()
    {
        timer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        if (current != null) return;

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
        current = Instantiate(prefab, GetFreePosition(), Quaternion.identity);
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
        while (snake.Occupies(x, y) || Mathf.RoundToInt(food.position.x) == x && Mathf.RoundToInt(food.position.y) == y);

        return new Vector3(x, y, 0f);
    }
}
