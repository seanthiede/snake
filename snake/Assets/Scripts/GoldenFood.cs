using UnityEngine;

public class GoldenFood : MonoBehaviour
{
    [SerializeField] private float lifetime = 6f;
    [SerializeField] private float blinkTime = 2f; // blinks the last x seconds
    [SerializeField] private int maxPoints = 10;
    [SerializeField] private int minPoints = 3;

    private SpriteRenderer sr;
    private float timeLeft;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        timeLeft = lifetime;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        if (timeLeft < blinkTime)
        {
            sr.enabled = Mathf.Repeat(timeLeft * 8f, 1f) > 0.5f; // blink effect
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Snake")) return;

        float t = timeLeft / lifetime;
        int points = Mathf.RoundToInt(Mathf.Lerp(minPoints, maxPoints, t));

        GameManager.Instance.SpawnParticles(transform.position, sr.color);
        GameManager.Instance.AddBonusPoints(points);
        Destroy(gameObject);
    }
}
