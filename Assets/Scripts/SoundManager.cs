using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;
    public AudioClip flipSfx, matchSfx, mismatchSfx, gameOverSfx, gameWinSfx;

    void Awake()
    {
        Instance = this;
    }

    public void PlayFlipSound()
    {
        if (flipSfx == null || audioSource == null) return;

        audioSource.PlayOneShot(flipSfx);
    }

    public void PlayMatchSound()
    {
        if (matchSfx == null || audioSource == null) return;

        audioSource.PlayOneShot(matchSfx);
    }

    public void PlayMismatchSound()
    {
        if (mismatchSfx == null || audioSource == null) return;

        audioSource.PlayOneShot(mismatchSfx);
    }

    public void PlayGameOverSound()
    {
        if (gameOverSfx == null || audioSource == null) return;

        audioSource.PlayOneShot(gameOverSfx);
    }

    public void PlayGameWinSound()
    {
        if (gameWinSfx == null || audioSource == null) return;

        audioSource.PlayOneShot(gameWinSfx);
    }
}