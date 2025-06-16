using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    public Canvas mainMenuCanvas;
    public Canvas difficultyMenuCanvas;
    public Canvas gamePlayCanvas;
    public Canvas gameOverCanvas;


    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI scoreTextSummary;
    public TextMeshProUGUI moveTextSummary;
    public TextMeshProUGUI comboTextSummary;

    public GameObject gridButtonPrefab;
    public Transform gridButtonParent;
    public int gridButtonCount;
    public List<Vector2> gridValues;

    public string savedGridValue;

    private GameObject currentBUtton;


    void Awake()
    {
        Instance = this;
        savedGridValue = PlayerPrefs.GetString("rowCol", "2,3");
    }

    private void Start()
    {
        GenerateGridButtons();
    }

    public void UpdateScoreUI(int score) //update score text accordingly
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateMoveUI(int moveCount) //update move text accordingly
    {
        moveText.text = "Moves: " + moveCount;
    }

    public void UpdateComboText(int comboCount) //update the combo text on combo increase
    {
        comboText.text = "Combo: " + comboCount;
    }

    public void ShowGameOverUI()
    {
        gameOverCanvas.enabled = true;
        scoreTextSummary.text = scoreText.text;
        comboTextSummary.text = comboText.text;
        moveTextSummary.text = moveText.text;
    }

    public void PlayGame()
    {
        mainMenuCanvas.enabled = false;
        gamePlayCanvas.enabled = true;
        GameManager.Instance.StartGame();
    }

    public void DifficultyPanel(bool isOpen)
    {
        mainMenuCanvas.enabled = !isOpen;
        difficultyMenuCanvas.enabled = isOpen;
        if (isOpen)
            EventSystem.current.SetSelectedGameObject(currentBUtton);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReplayGame() //reset game and hide reset button to avoid two triggers
    {
        GameManager.Instance.ResetGame();
        gameOverCanvas.enabled = false;
    }

    public void GenerateGridButtons()
    {
        string[] splitString = savedGridValue.Split(',');
        Vector2 loadedVector = new Vector2(float.Parse(splitString[0]), float.Parse(splitString[1]));
        for (int i = 0; i < gridButtonCount; i++)
        {
            int index = i;
            GameObject go = Instantiate(gridButtonPrefab, gridButtonParent);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.Instance.SetRowCol((int)gridValues[index].x, (int)gridValues[index].y);
                currentBUtton = go;
            });
            go.GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)gridValues[index].x} X {(int)gridValues[index].y}";
            if (loadedVector == gridValues[index])
            {
                //EventSystem.current.SetSelectedGameObject(go);
                currentBUtton = go;
            }
        }
    }
}