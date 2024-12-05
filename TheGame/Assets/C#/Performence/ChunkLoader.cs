using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public Transform player; // Spilleren eller kameraets position
    public float loadRadius = 20f; // Radius for hvilke chunks der skal aktiveres
    public List<string> chunkTags = new List<string>(); // Liste over tags, der identificerer chunks
    public LayerMask chunkLayers; // LayerMask for at identificere chunks via lag

    private List<GameObject> chunks = new List<GameObject>(); // Dynamisk liste over alle chunks i spillet

    void Start()
    {
        // Find alle GameObjects baseret p� tags
        foreach (string tag in chunkTags)
        {
            GameObject[] foundChunks = GameObject.FindGameObjectsWithTag(tag);
            chunks.AddRange(foundChunks); // Tilf�j dem til chunks-listen
        }

        // Find alle GameObjects baseret p� lag
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (((1 << obj.layer) & chunkLayers) != 0 && !chunks.Contains(obj))
            {
                chunks.Add(obj); // Tilf�j, hvis objektet matcher lag og ikke allerede er i listen
            }
        }
    }

    void Update()
    {
        for (int i = chunks.Count - 1; i >= 0; i--) // G� bagl�ns for at undg� problemer, mens vi fjerner objekter
        {
            GameObject chunk = chunks[i];

            // Tjek om chunk er null (�delagt objekt)
            if (chunk == null)
            {
                chunks.RemoveAt(i); // Fjern det �delagte objekt fra listen
                continue;
            }

            // Beregn afstanden fra spilleren til chunkens position
            float distance = Vector3.Distance(player.position, chunk.transform.position);

            // Aktiv�r chunk, hvis den er inden for radius
            if (distance <= loadRadius)
            {
                if (!chunk.activeSelf)
                {
                    chunk.SetActive(true); // Aktiv�r chunk
                }
            }
            else
            {
                if (chunk.activeSelf)
                {
                    chunk.SetActive(false); // Deaktiver chunk
                }
            }
        }
    }
    void CleanUpChunks()
    {
        chunks.RemoveAll(chunk => chunk == null); // Fjern alle null-elementer fra listen
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, loadRadius); // Visualiser radius
        }
    }
}
