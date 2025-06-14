using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isBusy = false;

    public GameObject cardPrefab;
    public Transform cardContainer;
    public Sprite[] cardImages;
    public int rows = 2;
    public int cols = 2;

    private List<Card> flippedCards = new();
    private List<Card> allCards = new();
    private int score = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 3f;

    private int moveCount = 0;
    private bool canFlip = true;

    public Vector2 cardSize = new(115, 181); // settable via Inspector
    public Vector2 spacing = new(10, 10);     // match Grid spacing
    public Vector2 screenSize = new(1500, 800); // or use Screen.width/height

    void Awake() => Instance = this;

    void Start()
    {
        if (!ValidateGridSize())
        {
            Debug.LogWarning("❌ Grid too large for the screen. Choose fewer rows/columns.");
            return;
        }

        GenerateCards();
    }

    void GenerateCards()
    {
        int totalCards = rows * cols;
        int pairCount = totalCards / 2;
        List<int> ids = new();

        // Get unique IDs from available images
        int availableImageCount = cardImages.Length;

        // Fill with random IDs from available images
        for (int i = 0; i < pairCount; i++)
        {
            int id = i < availableImageCount ? i : Random.Range(0, availableImageCount);
            ids.Add(id);
            ids.Add(id);
        }

        // If odd number of cards, add one more
        if (totalCards % 2 != 0)
        {
            int id = Random.Range(0, availableImageCount);
            ids.Add(id);
        }

        // Shuffle IDs
        Shuffle(ids);

        // Instantiate cards
        for (int i = 0; i < ids.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            Card card = cardObj.GetComponent<Card>();
            card.cardID = ids[i];
            card.front.GetComponent<Image>().sprite = cardImages[ids[i]];
            allCards.Add(card);
        }
    }

    bool ValidateGridSize()
    {
        float requiredWidth = (cardSize.x * cols) + (spacing.x * (cols - 1));
        float requiredHeight = (cardSize.y * rows) + (spacing.y * (rows - 1));

        return requiredWidth <= screenSize.x && requiredHeight <= screenSize.y;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void OnCardFlipped(Card card)
    {
        if (!canFlip) return;

        flippedCards.Add(card);

        isBusy = true; // 🔒 Prevent further flips until this one is processed

        if (flippedCards.Count == 2)
        {
            canFlip = false; // 🔒 Lock further flips until reset completes
            CheckMatch();
        }
        else
        {
            isBusy = false; // 🔒 Prevent further flips until this one is processed
        }
    }

    void CheckForWin()
    {
        foreach (var card in allCards)
        {
            if (!card.IsMatched)
                return; // Still unmatched cards
        }
    }

    IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(1f);

        foreach (var card in flippedCards)
        {
            card.ResetFlipAnimated(); // Uses the coroutine for smooth flip-back
        }

        flippedCards.Clear(); // ✅ Now it's safe to clear
        canFlip = true; // ✅ Unlock flipping after mismatch reset
        isBusy = false; // 🔓 Allow further flips
    }

    void CheckMatch()
    {
        moveCount++;

        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];

        if (card1.cardID == card2.cardID)
        {
            card1.Match();
            card2.Match();

            comboCount++;
            comboTimer = comboResetTime;
            int points = 10 + (comboCount - 1) * 5;
            score += points;

            flippedCards.Clear(); // ✅ Only clear here if it's a match

            canFlip = true; // ✅ Unlock flipping after match

            isBusy = false; // 🔓 Allow further flips

            CheckForWin();
        }
        else
        {
            comboCount = 0;
            StartCoroutine(DelayedReset());
            // ❌ DO NOT clear here — wait until coroutine finishes
        }
    }

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboCount = 0;
            }
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        score = PlayerPrefs.GetInt("Score", 0);
    }

    public void ResetGame()
    {
        // Reset game state
        score = 0;
        moveCount = 0;
        comboCount = 0;
        comboTimer = 0f;

        // Clear all cards
        foreach (var card in allCards)
        {
            Destroy(card.gameObject);
        }
        allCards.Clear();
        flippedCards.Clear();

        GenerateCards(); // Regenerate cards
    }
}