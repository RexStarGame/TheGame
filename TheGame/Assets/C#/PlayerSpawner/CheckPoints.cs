using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActive = false; // Indikerer, om dette er det seneste aktive checkpoint
    public float detectionRadius = 5f; // Juster denne v�rdi efter behov
    private GameObject player;

    void Start()
    {
        // Find spilleren automatisk
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player ikke fundet! S�rg for at spilleren er tagget som 'Player'.");
        }
    }

    void Update()
    {
        if (isActive)
            return; // Hvis checkpointet allerede er aktivt, beh�ver vi ikke tjekke igen

        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= detectionRadius)
            {
                ActivateCheckpoint();
            }
        }
    }

    void ActivateCheckpoint()
    {
        // S�t alle andre checkpoints til inaktive
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        if (checkpointManager != null)
        {
            checkpointManager.ResetCheckpoints();
        }

        // G�r dette checkpoint aktivt
        isActive = true;

        // Opdater spillerens respawn-position
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetRespawnPosition(transform.position);
            Debug.Log("Checkpoint n�et ved position: " + transform.position);
        }
        else
        {
            Debug.LogWarning("PlayerHealth-komponent blev ikke fundet p� spilleren!");
        }
    }

    public void ResetCheckpoint()
    {
        isActive = false;
    }
}
