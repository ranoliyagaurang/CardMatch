using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the core game logic for a memory card matching game, handling card generation, matching, scoring, and game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isBusy = false;

    [Header("Game Components")]
    public GridAutoScaler gridAutoScaler;
    public GameObject cardPrefab;
    public Transform cardContainer;
    public Sprite[] cardImages;

    [Header("Grid Settings")]
    public int rows = 2;
    public int cols = 2;
    public Vector2 cardSize = new(115, 181);
    public Vector2 spacing = new(10, 10);
    public Vector2 screenSize = new(1500, 800);

    [Header("Game Settings")]
    public float comboResetTime = 3f;

    private readonly List<Card> flippedCards = new();
    private readonly List<Card> allCards = new();
    private int score;
    private int comboCount;
    private float comboTimer;
    private int moveCount;

    private Queue<Card> cardFlipQueue = new();
    private bool isChecking = false;


    void Awake() => Instance = this;


    public void SetRowCol(int row, int col)
    {
        rows = row; cols = col;
        PlayerPrefs.SetString("rowCol", $"{row},{col}");
        UIManager.Instance.savedGridValue = PlayerPrefs.GetString("rowCol");
    }

    public void StartGame()
    {
        if (!ValidateGridSize())
        {
            Debug.LogWarning("Grid too large for the screen. Choose fewer rows/columns.");
            return;
        }

        gridAutoScaler.SetGridSize(rows, cols);
        GenerateCards();
        UpdateUI();
    }

    /// <summary>
    /// Creates and arranges cards in the grid, ensuring proper pairing and randomization.
    /// </summary>
    void GenerateCards()
    {
        int totalCards = rows * cols;
        int pairCount = totalCards / 2;
        var ids = new List<int>(totalCards);
        int availableImageCount = cardImages.Length;

        for (int i = 0; i < pairCount; i++)
        {
            int id = i < availableImageCount ? i : Random.Range(0, availableImageCount);
            ids.Add(id);
            ids.Add(id);
        }

        if (totalCards % 2 != 0)
        {
            ids.Add(Random.Range(0, availableImageCount));
        }

        Shuffle(ids);

        for (int i = 0; i < ids.Count; i++)
        {
            var cardObj = Instantiate(cardPrefab, cardContainer);
            var card = cardObj.GetComponent<Card>();
            card.cardID = ids[i];
            card.front.GetComponent<Image>().sprite = cardImages[ids[i]];
            allCards.Add(card);
        }
    }

    /// <summary>
    /// Verifies if the current grid configuration fits within the screen boundaries.
    /// </summary>
    bool ValidateGridSize()
    {
        float requiredWidth = (cardSize.x * cols) + (spacing.x * (cols - 1));
        float requiredHeight = (cardSize.y * rows) + (spacing.y * (rows - 1));
        return requiredWidth <= screenSize.x && requiredHeight <= screenSize.y;
    }

    /// <summary>
    /// Randomizes the order of elements in a list using the Fisher-Yates shuffle algorithm.
    /// </summary>
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    /// <summary>
    /// Handles the logic when a card is flipped, managing game state and initiating matching checks.
    /// </summary>
    public void OnCardFlipped(Card card)
    {
        if (card.IsMatched || flippedCards.Contains(card) || isBusy)
            return;

        flippedCards.Add(card);
        SoundManager.Instance.PlayFlipSound();

        if (!isChecking && flippedCards.Count >= 2)
        {
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    IEnumerator CheckMatchCoroutine()
    {
        isChecking = true;

        while (flippedCards.Count >= 2)
        {
            var card1 = flippedCards[0];
            var card2 = flippedCards[1];

            moveCount++;
            UIManager.Instance.UpdateMoveUI(moveCount);

            if (card1.cardID == card2.cardID)
            {
                card1.Match();
                card2.Match();

                comboCount++;
                comboTimer = comboResetTime;
                score += 1 + (comboCount - 1) * 5;
                UIManager.Instance.UpdateComboText(comboCount);
                UIManager.Instance.UpdateScoreUI(score);

                flippedCards.RemoveAt(1);
                flippedCards.RemoveAt(0);

                yield return new WaitForSeconds(0.15f); //this is the duration of card flip animation set in card.cs Script
                SoundManager.Instance.PlayMatchSound();
                CheckForWin();

            }
            else
            {
                comboCount = 0;
                UIManager.Instance.UpdateComboText(comboCount);
                yield return new WaitForSeconds(1f);
                card1.ResetFlipAnimated();
                card2.ResetFlipAnimated();
                flippedCards.RemoveAt(1);
                flippedCards.RemoveAt(0);

                yield return new WaitForSeconds(0.15f); //this is the duration of card flip animation set in card.cs Script
                SoundManager.Instance.PlayMismatchSound();
            }

            yield return null;
        }

        isChecking = false;
    }


    /// <summary>
    /// Checks if all cards have been matched, triggering win condition if true.
    /// </summary>
    void CheckForWin()
    {
        if (allCards.TrueForAll(card => card.IsMatched))
        {
            SoundManager.Instance.PlayGameWinSound();
            UIManager.Instance.ShowGameOverUI();
        }
    }


    /// <summary>
    /// Updates the combo timer and resets combo count when timer expires.
    /// </summary>
    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboCount = 0;
                UIManager.Instance.UpdateComboText(comboCount);
            }
        }
    }

    /// <summary>
    /// Updates the UI elements with current score and move count.
    /// </summary>
    void UpdateUI()
    {
        UIManager.Instance.UpdateScoreUI(score);
        UIManager.Instance.UpdateMoveUI(moveCount);
        UIManager.Instance.UpdateComboText(comboCount);
    }

    /// <summary>
    /// Saves the current game score to persistent storage.
    /// </summary>
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the previously saved game score from persistent storage.
    /// </summary>
    public void LoadProgress()
    {
        score = PlayerPrefs.GetInt("Score", 0);
    }

    /// <summary>
    /// Resets the game state, clearing all cards and regenerating a new game board.
    /// </summary>
    public void ResetGame()
    {
        score = 0;
        moveCount = 0;
        comboCount = 0;
        UIManager.Instance.UpdateComboText(comboCount);
        comboTimer = 0f;

        foreach (var card in allCards)
        {
            Destroy(card.gameObject);
        }
        allCards.Clear();
        flippedCards.Clear();

        UpdateUI();
        GenerateCards();
    }
}