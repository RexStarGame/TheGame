using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player; // Spilleren, som kameraet skal f�lge
    public Vector3 offset; // Afstand fra spilleren
    public float smoothSpeed = 0.125f; // Hvor glat kameraet f�lger spilleren (lavere v�rdi = glattere)

    void FixedUpdate()
    {
        // Beregn kameraets �nskede position baseret p� spillerens position og offset
        Vector3 desiredPosition = player.position + offset;

        // Glid langsomt fra kameraets nuv�rende position til den �nskede position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Opdater kameraets position
        transform.position = smoothedPosition;
    }
}
