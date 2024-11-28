using UnityEngine;

public class HealthBarFollow2D : MonoBehaviour
{
    public Transform enemy; // Fjendens position i verden.
    public Vector3 offset = new Vector3(0, 1, 0); // Offset for at placere health baren over fjendens hoved.

    void Update()
    {
        if (enemy != null)
        {
            // Opdater health barens position direkte i World Space.
            transform.position = enemy.position + offset;
        }
    }
}