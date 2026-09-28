using UnityEngine;

public enum PowerUpType
{
    SlowMotion,
    Ghost,
    Poison
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType type;
    [SerializeField] private float lifetime = 7f; // Lifetime of the power-up in seconds
    [SerializeField] private float blinkTime = 2f; // The last x seconds of the lifetime will be spent blinking

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

        // before vanishing, blink (8x per second)
        if (timeLeft < blinkTime)
        {
            sr.enabled = Mathf.Repeat(timeLeft * 8f, 1f) > 0.5f; // Blink 8 times per second
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Snake")) return;
        
        GameManager.Instance.SpawnParticles(transform.position, sr.color);
        GameManager.Instance.ActivatePowerUp(type);
        Destroy(gameObject);
    }
}
