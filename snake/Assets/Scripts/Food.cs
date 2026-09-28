using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private ParticleSystem eatParticlePrefab;

    public BoxCollider2D gridArea;

    private void Start()
    {
        RandomizedPositions();
    }

    private void RandomizedPositions()
    {
        Bounds bounds = this.gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
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
