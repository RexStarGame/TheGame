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
            Debug.LogWarning("LootPrefabs-listen er tom. Tilføjer en test prefab automatisk.");
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

        // Generer et tilfældigt tal mellem 0 og 100
        float randomValue = Random.Range(0f, 100f);
        Debug.Log($"Tilfældig værdi: {randomValue}, Drop chance: {dropChance}");

        // Tjek om den tilfældige værdi er mindre end eller lig med dropChance
        if (randomValue <= dropChance)
        {
            if (lootPrefabs.Count > 0)
            {
                // Vælg et tilfældigt loot fra listen
                int randomIndex = Random.Range(0, lootPrefabs.Count);
                GameObject selectedLoot = lootPrefabs[randomIndex];

                // Beregn en tilfældig position inden for lootSpawnRadius
                Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * lootSpawnRadius);
                spawnPosition.y = transform.position.y; // Hold samme y-værdi som fjenden

                Debug.Log($"Spawning {selectedLoot.name} på position: {spawnPosition}");
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
