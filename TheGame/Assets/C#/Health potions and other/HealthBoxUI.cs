using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For UI Image

public class HealthBoxUI : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player; // Reference til spilleren
    public float displayDistance = 3f; // Afstand inden for hvilken UI vises

    [Header("UI Elements")]
    public Canvas uiCanvas; // Canvas der indeholder UI-elementerne
    public TextMeshProUGUI instructionText; // Tekst der viser "Tryk p� E"
    public Image keyboardImage; // Billede af tastaturet eller E-tasten

    private bool isPicked = false; // Lokal variabel til at tracke, om dette objekt er samlet op
    private bool isPlayerClose = false;

    void Start()
    {
        isPicked = false;

        // Skjul UI fra starten
        uiCanvas.gameObject.SetActive(false);

        // Hvis spilleren ikke er sat, pr�v at finde den automatisk
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player ikke fundet! S�rg for at s�tte player-referencen eller tagge spilleren som 'Player'.");
            }
        }
    }

    void Update()
    {
        // Hvis objektet er samlet op, skjul UI permanent
        if (isPicked)
        {
            HideUI();
            return;
        }

        if (player != null)
        {
            // Beregn afstanden mellem spilleren og HealthBox
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= displayDistance)
            {
                if (!isPlayerClose)
                {
                    // Spilleren er nu t�t nok, vis UI
                    ShowUI();
                }
                isPlayerClose = true;
            }
            else
            {
                if (isPlayerClose)
                {
                    // Spilleren er g�et v�k, skjul UI
                    HideUI();
                }
                isPlayerClose = false;
            }
        }
    }

    public void MarkAsPicked()
    {
        // Kald denne metode, n�r objektet bliver samlet op
        isPicked = true; // Marker objektet som samlet op
        HideUI(); // S�rg for, at UI skjules
    }

    void ShowUI()
    {
        // Vis kun UI, hvis objektet ikke er samlet op
        if (!isPicked)
        {
            uiCanvas.gameObject.SetActive(true);
        }
    }

    void HideUI()
    {
        uiCanvas.gameObject.SetActive(false);
    }
}
