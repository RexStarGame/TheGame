using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider;
    public int MaxHealth = 100;
    public int CurrentHealth;

    [Header("Respawn Settings")]
    private Vector3 respawnPosition;
    private bool hasFallen = false; // Tilf�jet variabel
    // Reference til PlayerHeartCounter
    private PlayerHeartCounter heartCounter;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        SetMaxHealth(MaxHealth);

        // F� reference til PlayerHeartCounter
        heartCounter = GetComponent<PlayerHeartCounter>();
        if (heartCounter == null)
        {
            Debug.LogError("PlayerHeartCounter-komponenten blev ikke fundet p� spilleren!");
        }

        // S�t initial respawn-position til spillerens startposition
        respawnPosition = transform.position;
    }
    private void Update()
    {
        // Tjek om spilleren er faldet under en bestemt h�jde
        if (!hasFallen && transform.position.y < -50f) // Juster v�rdien efter behov
        {
            hasFallen = true; // Sikrer, at skade kun p�f�res �n gang per fald
            // P�f�r 100 skade eller dr�b spilleren
            TakeDamage(100);
            Debug.Log("Spilleren faldt under -50 og tog 100 skade.");
        }
    }
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        SetHealth(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Reducer hjerter via PlayerHeartCounter
        if (heartCounter != null)
        {
            heartCounter.LoseHeart();

            if (heartCounter.HasHeartsLeft())
            {
                // Respawn ved sidste checkpoint
                Respawn();
            }
            else
            {
                // Ingen flere hjerter, spillet resetter
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            Debug.LogError("Ingen PlayerHeartCounter fundet! Kan ikke h�ndtere hjerter.");
        }
    }

    void Respawn()
    {
        hasFallen = false;
        // Gendan liv
        CurrentHealth = MaxHealth;
        SetHealth(CurrentHealth);

        // Flyt spilleren til respawn-positionen
        transform.position = respawnPosition;

        // Nulstil spillerens hastighed, hvis der er en Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void SetRespawnPosition(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
