using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText; // Assign via Inspector
    public TextMeshProUGUI moveText; // Assign in Inspector

    public GameObject replayBt;

    void Awake()
    {
        Instance = this;

        replayBt.SetActive(false);
    }

    public void UpdateScoreUI(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateMoveUI(int moveCount)
    {
        moveText.text = "Moves: " + moveCount;
    }

    public void ReplayGame()
    {
        GameManager.Instance.ResetGame();
        replayBt.SetActive(false);
    }
}