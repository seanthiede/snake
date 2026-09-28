using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int initialSize = 4;
    [SerializeField] private Gradient bodyGradient = new Gradient();
    [SerializeField] private int minLength = 2;
    [SerializeField, Range(0f, 1f)] private float ghostAlpha = 0.35f;

    private Vector2 direction = Vector2.right;
    private List<Transform> segments = new List<Transform>();
    private bool isGhost;

    public IReadOnlyList<Transform> Segments => segments;

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

        UpdateColors();
    }

    public void ResetState()
    {
        isGhost = false;
        direction = Vector2.right;

        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(this.transform);

        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(this.segmentPrefab).transform;
            segment.position = this.transform.position - (Vector3)(direction * i);
            segments.Add(segment);
        }

        UpdateColors();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;

        if (other.tag == "Food")
        {
            Grow();
            GameManager.Instance.AddPoint();
        }
        else if (other.CompareTag("Obstacle"))
        {
            // in ghostmode ignores own segments, but not walls
            if (isGhost && segments.Contains(other.transform)) return;

            GameManager.Instance.OnSnakeDied();
        }
    }

    private void UpdateColors()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            float t = segments.Count > 1 ? (float)i / (segments.Count - 1) : 0f;

            Color c = bodyGradient.Evaluate(t);
            if (isGhost)
            {
                c.a = ghostAlpha;
            }

            segments[i].GetComponent<SpriteRenderer>().color = bodyGradient.Evaluate(t);
        }
    }

    public void SetGhost(bool ghost)
    {
        isGhost = ghost;
        UpdateColors();
    }

    // removes last segments from list and returns it (null if too short)
    public Transform RemoveTail()
    {
        if (segments.Count <= minLength) return null;

        Transform tail = segments[segments.Count - 1];
        segments.RemoveAt(segments.Count - 1);
        UpdateColors();
        return tail;
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x && Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }
}
