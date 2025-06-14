using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText; // Assign via Inspector
    public TextMeshProUGUI moveText; // Assign in Inspector

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScoreUI(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateMoveUI(int moveCount)
    {
        moveText.text = "Moves: " + moveCount;
    }
}