using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static int Score = 0;
    private static ScoreController instance;

    [Header("Références UI")]
    [SerializeField] private TMP_Text scoreText;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public static void AddPoints(int points)
    {
        Score += points;
            instance.UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
            scoreText.text = "O: " + Score;
    }

    public static void ResetScore()
    {
        Score = 0;
            instance.UpdateScoreUI();
    }
}
