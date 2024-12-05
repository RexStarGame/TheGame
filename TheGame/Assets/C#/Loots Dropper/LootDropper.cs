using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Loot Settings")]
    public float dropChance = 100f; // Chance for at droppe loot (i procent)
    public List<GameObject> lootPrefabs; // Liste over loot-prefabs
    public float lootSpawnRadius = 1f; // Radius omkring fjenden, hvor loot kan spawnes

    [ContextMenu("Test DropLoot")]

    void Start()
    {
        if (lootPrefabs == null || lootPrefabs.Count == 0)
        {
            Debug.LogWarning("LootPrefabs-listen er tom. Tilf�jer en test prefab automatisk.");
            GameObject testLoot = new GameObject("TestLoot");
            lootPrefabs.Add(testLoot);
        }
    }

    public void TestDropLoot()
    {
        DropLoot();
    }

    public void DropLoot()
    {
        Debug.Log($"DropLoot() blev kaldt for {gameObject.name}");

        // Generer et tilf�ldigt tal mellem 0 og 100
        float randomValue = Random.Range(0f, 100f);
        Debug.Log($"Tilf�ldig v�rdi: {randomValue}, Drop chance: {dropChance}");

        // Tjek om den tilf�ldige v�rdi er mindre end eller lig med dropChance
        if (randomValue <= dropChance)
        {
            if (lootPrefabs.Count > 0)
            {
                // V�lg et tilf�ldigt loot fra listen
                int randomIndex = Random.Range(0, lootPrefabs.Count);
                GameObject selectedLoot = lootPrefabs[randomIndex];

                // Beregn en tilf�ldig position inden for lootSpawnRadius
                Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * lootSpawnRadius);
                spawnPosition.y = transform.position.y; // Hold samme y-v�rdi som fjenden

                Debug.Log($"Spawning {selectedLoot.name} p� position: {spawnPosition}");
                Instantiate(selectedLoot, spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("LootPrefabs-listen er tom!");
            }
        }
        else
        {
            Debug.Log("Ingen loot droppet denne gang.");
        }
    }
}
