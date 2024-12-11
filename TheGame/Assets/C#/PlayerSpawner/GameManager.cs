using System;
using UnityEngine;
using TMPro; // Husk at inkludere TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public int nisserTilbage = 20;
    public GameObject bossObject;
    public Transform bossSpawnPoint;
    public TextMeshProUGUI nisseCounterText; // Reference til TextMeshPro UI
    public GameObject winMenu; // UI-element til vindermenuen

    private bool bossSpawned = false;
    private bool gameWon = false;

    public static event Action OnEnemyKilled;

    private static GameManager instance;

    private void Awake()
    {
        // Singleton-pattern for at sikre �n central manager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Opdater tekst ved start
        UpdateNisseCounter();

        // S�rg for, at vindermenuen er skjult ved start
        if (winMenu != null)
        {
            winMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("WinMenu er ikke tildelt i Inspector!");
        }
    }

    public static void Dr�bNisse()
    {
        if (instance != null)
        {
            instance.nisserTilbage--;
            Debug.Log("Nisser tilbage: " + instance.nisserTilbage);

            // Opdater nisset�lleren
            instance.UpdateNisseCounter();

            if (instance.nisserTilbage <= 0 && !instance.bossSpawned)
            {
                instance.AktiverBoss();
            }

            // Trigger event
            OnEnemyKilled?.Invoke();
        }
    }

    private void AktiverBoss()
    {
        if (bossSpawned)
        {
            Debug.LogWarning("Bossen er allerede aktiveret!");
            return;
        }

        if (bossObject == null)
        {
            Debug.LogError("Boss-objektet er ikke tildelt i GameManager!");
            return;
        }

        bossSpawned = true;
        bossObject.SetActive(true);
        bossObject.transform.position = bossSpawnPoint.position;
        Debug.Log("Bossen er aktiveret og placeret ved spawn-point!");

        // Fjern teksten, n�r bossen spawner
        if (nisseCounterText != null)
        {
            nisseCounterText.text = "Julemanden er her!";
        }
    }

    private void UpdateNisseCounter()
    {
        if (nisseCounterText != null)
        {
            nisseCounterText.text = $"Dr�b {nisserTilbage} nisser for at m�de Julemanden!";
        }
        else
        {
            Debug.LogWarning("NisseCounterText er ikke tildelt i Inspector!");
        }
    }

    public void BossDied()
    {
        if (gameWon)
            return;

        gameWon = true;
        Debug.Log("Bossen er d�d! Vindermenu vises.");

        if (winMenu != null)
        {
            winMenu.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Debug.Log("Genstarter spillet...");
        // Implement�r genstart, for eksempel:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Afslutter spillet...");
        Application.Quit();
    }
}
