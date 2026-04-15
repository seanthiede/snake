using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int initialSize = 4;

    private Vector2 direction = Vector2.right;
    private List<Transform> segments = new List<Transform>();

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0.0f && direction.x == 0.0f)
        {
            direction = new Vector2(horizontal, 0.0f);
        }
        else if (vertical != 0.0f && direction.y == 0.0f)
        {
            direction = new Vector2(0.0f, vertical);
        }
    }

    private void FixedUpdate()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + direction.x, 
            Mathf.Round(this.transform.position.y) + direction.y,
            0.0f);
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab).transform;
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    private void ResetState()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(this.transform);

        for (int i = 1; i < initialSize; i++)
        {
            Grow();
        }

        this.transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Obstacle")
        {
            ResetState();
        }
    }
}
