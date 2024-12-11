using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    public Slider slider;
    public int MaxHealth = 100;
    public int CurrentHealth;

    [SerializeField] private float fallLimit = 50;
    [SerializeField] private int fallDamage = 100;

    [Header("Animations")]
    public Animator animator;

    [Header("Respawn Settings")]
    private Vector3 respawnPosition;
    private bool hasFallen = false; // Tilføjet variabel
    // Reference til PlayerHeartCounter
    private PlayerHeartCounter heartCounter;
    private bool diedFromFall = false;
    private void Start()
    {
        animator = GetComponent<Animator>(); // Hent Animator-komponenten fra GameObjectet
        CurrentHealth = MaxHealth;
        SetMaxHealth(MaxHealth);

        // Få reference til PlayerHeartCounter
        heartCounter = GetComponent<PlayerHeartCounter>();
        if (heartCounter == null)
        {
            Debug.LogError("PlayerHeartCounter-komponenten blev ikke fundet på spilleren!");
        }

        // Sæt initial respawn-position til spillerens startposition
        respawnPosition = transform.position;
    }
    private void Update()
    {
        // Tjek om spilleren er faldet under en bestemt højde
        if (!hasFallen && transform.position.y < -fallLimit && CurrentHealth > 0) // Juster værdien efter behov
        {
            hasFallen = true; // Sikrer, at skade kun påføres én gang per fald
            diedFromFall = true; // Marker, at døden skyldes fald
            TakeDamage(fallDamage);
            Debug.Log("Spilleren faldt under " + fallLimit + " og tog " + fallDamage + " skade.");
        }
    }
    public void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0) return; // Spilleren er allerede død, undgå yderligere skade
        CurrentHealth -= damage;
        SetHealth(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            if (!diedFromFall) // Kun afspil animation, hvis døden ikke skyldes fald
            {
                animator.SetBool("PlayerIsDead", true);
                StartCoroutine(HandleDeath()); // Brug coroutine for animation
            }
            else
            {
                Die(); // Ingen wait, spilleren dør straks ved fald
            }
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
            Debug.LogError("Ingen PlayerHeartCounter fundet! Kan ikke håndtere hjerter.");
        }
    }
    private IEnumerator HandleDeath()
    {
        // Afspil død-animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Få animationens længde og tilføj ekstra tid
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 1.0f); // Juster forsinkelsen her

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
            Debug.LogError("Ingen PlayerHeartCounter fundet! Kan ikke håndtere hjerter.");
        }
    }
  void Respawn()
{
    hasFallen = false; // Nulstil fald-status
    // Gendan liv
    CurrentHealth = MaxHealth;
    SetHealth(CurrentHealth);

    // Flyt spilleren til respawn-positionen
    transform.position = respawnPosition;

    // Nulstil spillerens hastighed, hvis der er en Rigidbody
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.velocity = Vector3.zero; // Stop bevægelse
        rb.angularVelocity = Vector3.zero; // Stop rotation
    }

    // Nulstil død-animationen
    if (animator != null)
    {
        animator.SetBool("PlayerIsDead", false);
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
