using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player; // Spilleren, som kameraet skal følge
    public Vector3 offset; // Afstand fra spilleren
    public float smoothSpeed = 0.125f; // Hvor glat kameraet følger spilleren (lavere værdi = glattere)

    void FixedUpdate()
    {
        // Beregn kameraets ønskede position baseret på spillerens position og offset
        Vector3 desiredPosition = player.position + offset;

        // Glid langsomt fra kameraets nuværende position til den ønskede position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Opdater kameraets position
        transform.position = smoothedPosition;
    }
}
