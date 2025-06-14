using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardID;
    public GameObject front;
    public GameObject back;

    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isAnimating = false;
    
    public bool IsMatched => isMatched;

    public void OnClick()
    {
        if (GameManager.Instance.isBusy) return;
        if (isMatched || isFlipped || isAnimating) return;
        StartCoroutine(Flip(true)); // true = forward flip
        GameManager.Instance.OnCardFlipped(this);
    }

    public IEnumerator Flip(bool forward)
    {
        isAnimating = true;

        float duration = 0.3f;
        float halfTime = duration / 2f;
        float elapsed = 0f;

        // First half rotation
        while (elapsed < halfTime)
        {
            float angle = Mathf.Lerp(0, 90, elapsed / halfTime);
            transform.localRotation = Quaternion.Euler(0, forward ? angle : -angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Swap faces
        isFlipped = forward;
        front.SetActive(forward);
        back.SetActive(!forward);

        // Second half rotation
        elapsed = 0f;
        while (elapsed < halfTime)
        {
            float angle = Mathf.Lerp(90, 180, elapsed / halfTime);
            transform.localRotation = Quaternion.Euler(0, forward ? angle : -angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0); // reset rotation
        isAnimating = false;
    }

    public void Match()
    {
        isMatched = true;
    }

    public void ResetFlipAnimated()
    {
        if (!isFlipped || isMatched || isAnimating) return;
        StartCoroutine(Flip(false)); // false = backward flip
    }
}