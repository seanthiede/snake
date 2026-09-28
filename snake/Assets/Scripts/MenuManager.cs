using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Difficulty")]
    [SerializeField] private DifficultySettings easy = new DifficultySettings { name = "Easy" , moveInterval = 0.12f};
    [SerializeField] private DifficultySettings normal = new DifficultySettings { name = "Normal", moveInterval = 0.08f};
    [SerializeField] private DifficultySettings hard = new DifficultySettings { name = "Hard", moveInterval = 0.06f};
    [SerializeField] private DifficultySettings insane = new DifficultySettings { name = "Insane", moveInterval = 0.04f};

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Snake";

    //[SerializeField] private TMP_Text highscoreText;

    private void Start()
    {
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void ShowDifficultyPanel()
    {
        startPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void StartEasy()
    {
        StartGame(easy);
    }

    public void StartNormal()
    {
        StartGame(normal);
    }

    public void StartHard()
    {
        StartGame(hard);
    }

    public void StartInsane()
    {
        StartGame(insane);
    }

    private void StartGame(DifficultySettings difficulty)
    {
        GameSettings.difficulty = difficulty;
        SceneManager.LoadScene(gameSceneName);
    }
}
