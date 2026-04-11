using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    private StarPoolManager poolManager;
    private AudioSource audioSource;

    public void SetPoolManager(StarPoolManager manager)
    {
        poolManager = manager;
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (other.CompareTag("Player"))
        {
            // 🎵 Play sound
            if (audioSource != null)
            {
                audioSource.Play();
            }

            GameManager.Instance.AddScore(1);

            // Delay deactivation so sound can play
            Invoke(nameof(DeactivateStar), 0.2f);
        }
    }

    void DeactivateStar()
    {
        if (poolManager != null)
            poolManager.ReturnStar(gameObject);
        else
            gameObject.SetActive(false);
    }
}