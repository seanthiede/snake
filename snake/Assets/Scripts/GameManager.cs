using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Snake snake;

    [Tooltip("Wird benutzt, wenn du die Snake-Szene direkt startest, ohne Menü.")]
    [SerializeField] private DifficultySettings fallbackDifficulty = new DifficultySettings();

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private TMP_Text newHighscoreText;

    [Header("Countdown")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private int countdownFrom = 3;
    [SerializeField] private float countdownInterval = 0.7f;

    [Header("Death: Screenshake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeMagnitude = 0.6f;

    [Header("Death: Flash")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField, Range(0f, 1f)] private float flashAlpha = 0.8f;

    [Header("Death: segments explosion")]
    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private float delaybeforePops = 0.2f;
    [SerializeField] private float timeBetweenPops = 0.05f;
    [SerializeField] private float delayAfterPops = 1f;

    [Header("Power-Ups")]
    [SerializeField] private TMP_Text powerUpText;
    [SerializeField] private float slowMotionDuration = 5f;
    [Tooltip("1.6 = jeder Schritt dauert 60% länger")]
    [SerializeField] private float slowMotionFactor = 1.6f;
    [SerializeField] private float ghostDuration = 5f;
    [SerializeField] private int poisonShrinkAmount = 5;

    [Header("Golden Food")]
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    [SerializeField] private int goldenFoodEvery = 5; // after every xth food

    private Coroutine slowMotionRoutine;
    private Coroutine ghostRoutine;
    private Coroutine powerUpTextRoutine;

    public bool IsPlaying => !isDying && snake.enabled;

    private DifficultySettings currentDifficulty;
    private bool isDying;
    private int score;
    private int highscore;
    private int foodEaten;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentDifficulty = GameSettings.difficulty ?? fallbackDifficulty;
        Time.fixedDeltaTime = currentDifficulty.moveInterval;

        SetFlashAlpha(0f);

        highscore = PlayerPrefs.GetInt(GameSettings.HighscoreKey(currentDifficulty), 0);
        score = 0;
        UpdateScoreTexts();
        newHighscoreText.gameObject.SetActive(false);
        powerUpText.gameObject.SetActive(false);

        snake.ResetState();
        snake.enabled = false;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = countdownFrom; i > 0; i--)
        {
            countdownText.text = i.ToString();
            StartCoroutine(PunchScale(countdownText.transform, 1.8f, 0.25f));
            yield return new WaitForSeconds(countdownInterval);
        }

        countdownText.text = "GO!";
        StartCoroutine(PunchScale(countdownText.transform, 1.8f, 0.25f));
        snake.enabled = true;

        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
    }

    public void AddPoint()
    {
        if (isDying) return;

        score++;
        UpdateScoreTexts();

        foodEaten++;
        if (foodEaten % goldenFoodEvery == 0)
        {
            powerUpSpawner.SpawnGoldenFood();
        }
    }

    public void AddBonusPoints(int amount)
    {
        if (isDying) return;

        score += amount;
        UpdateScoreTexts();
        ShowPowerUpText($"+{amount}!");
    }

    private void UpdateScoreTexts()
    {
        scoreText.text = $"POINTS: {score}";
        highscoreText.text = $"HIGHSCORE: {Mathf.Max(score, highscore)}";
    }

    private bool SaveHighscore()
    {
        if (score <= highscore) return false;

        highscore = score;
        PlayerPrefs.SetInt(GameSettings.HighscoreKey(currentDifficulty), highscore);
        PlayerPrefs.Save();
        return true;
    }

    public void OnSnakeDied()
    {
        if (isDying) return;
        isDying = true;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        snake.enabled = false;
        bool isNewHighscore = SaveHighscore();

        cameraShake.Shake(shakeDuration, shakeMagnitude);
        StartCoroutine(FlashRoutine());

        yield return new WaitForSeconds(delaybeforePops);

        foreach (Transform segment in snake.Segments)
        {
            SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();

            ParticleSystem particles = Instantiate(deathParticlesPrefab, segment.position, Quaternion.identity);
            var main = particles.main;
            main.startColor = sr.color;
            particles.Play();

            sr.enabled = false;
            yield return new WaitForSeconds(timeBetweenPops);
        }

        if (isNewHighscore)
        {
            newHighscoreText.gameObject.SetActive(true);
            StartCoroutine(PunchScale(newHighscoreText.transform, 2f, 0.3f));
        }

        yield return new WaitForSeconds(delayAfterPops);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator PunchScale(Transform target, float startScale, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, 1f, elapsed / duration);
            target.localScale = Vector3.one * scale;
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            SetFlashAlpha(Mathf.Lerp(flashAlpha, 0f, elapsed / flashDuration));
            yield return null;
        }
    }

    private void SetFlashAlpha(float alpha)
    {
        Color color = flashImage.color;
        color.a = alpha;
        flashImage.color = color;
    }

    // is called by PowerUp when collected
    public void ActivatePowerUp(PowerUpType type)
    {
        if (!IsPlaying) return;

        switch (type)
        {
            case PowerUpType.SlowMotion:
                // already running? stop it first, then start a new one
                if (slowMotionRoutine != null) StopCoroutine(slowMotionRoutine);
                slowMotionRoutine = StartCoroutine(SlowMotionRoutine());
                ShowPowerUpText("SLOW-MO!");
                break;
            case PowerUpType.Ghost:
                if (ghostRoutine != null) StopCoroutine(ghostRoutine);
                ghostRoutine = StartCoroutine(GhostRoutine());
                ShowPowerUpText("GHOST!");
                break;
            case PowerUpType.Poison:
                ShrinkSnake();
                ShowPowerUpText("POISON!");
                break;
        }
    }

    private IEnumerator SlowMotionRoutine()
    {
        Time.fixedDeltaTime = currentDifficulty.moveInterval * slowMotionFactor;
        yield return new WaitForSeconds(slowMotionDuration);
        Time.fixedDeltaTime = currentDifficulty.moveInterval;
        slowMotionRoutine = null;
    }

    private IEnumerator GhostRoutine()
    {
        snake.SetGhost(true);
        yield return new WaitForSeconds(ghostDuration);
        snake.SetGhost(false);
        ghostRoutine = null;
    }

    private void ShrinkSnake()
    {
        for (int i = 0; i < poisonShrinkAmount; i++)
        {
            Transform tail = snake.RemoveTail();
            if (tail == null) break; // snake has minimum length

            SpawnParticles(tail.position, tail.GetComponent<SpriteRenderer>().color);
            Destroy(tail.gameObject);
        }
    }

    // public so that powerup can produce particles 
    public void SpawnParticles(Vector3 position, Color color)
    {
        ParticleSystem particles = Instantiate(deathParticlesPrefab, position, Quaternion.identity);
        var main = particles.main;
        main.startColor = color;
        particles.Play();
    }

    private void ShowPowerUpText(string message)
    {
        if (powerUpTextRoutine != null) StopCoroutine(powerUpTextRoutine);
        powerUpTextRoutine = StartCoroutine(PowerUpTextRoutine(message));
    }

    private IEnumerator PowerUpTextRoutine(string message)
    {
        powerUpText.text = message;
        powerUpText.gameObject.SetActive(true);
        StartCoroutine(PunchScale(powerUpText.transform, 1.8f, 0.25f));

        yield return new WaitForSeconds(1f);

        powerUpText.gameObject.SetActive(false);
        powerUpTextRoutine = null;
    }
}
