using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private ParticleSystem eatParticlePrefab;
    [SerializeField] private Snake snake;

    public BoxCollider2D gridArea;

    private void Start()
    {
        RandomizedPositions();
    }

    private void RandomizedPositions()
    {
        Bounds bounds = this.gridArea.bounds;
        int x, y;

        do
        {
            x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
            y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        }
        while (snake != null && snake.Occupies(x, y));

        this.transform.position = new Vector3(x, y, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Snake")
        {

            if (eatParticlePrefab != null)
            {
                Instantiate(eatParticlePrefab, transform.position, Quaternion.identity);
            }

            RandomizedPositions();
        }
    }
}
