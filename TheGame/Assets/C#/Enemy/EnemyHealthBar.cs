using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public float maxHealth = 100f; // Fjendens maksimale liv.
    public float currentHealth; // Fjendens aktuelle liv.
    public float defence = 5f; // Forsvarsv�rdien.
    public Image healthBar; // Health bar UI-element.
    public float evasionChance = 0.2f; // 20% chance for at undvige angreb.
    private bool isDead = false; // Ny variabel til at tracke drab

    public LootDropper lootDropper; // Reference til LootDropper-scriptet
    // Reference til MissTextController
    public MissTextController missTextController;
    void Start()
    {
        // Automatisk finde LootDropper-komponenten
        lootDropper = GetComponent<LootDropper>();
        if (lootDropper == null)
        {
            Debug.LogWarning("LootDropper-komponenten blev ikke fundet p� fjenden!");
        }
        // S�t fjendens aktuelle liv til maks i starten.
        currentHealth = maxHealth;
        UpdateHealthBar();

    }

    public void TakeDamage(float playerDamage)
    {
        //Debug.Log($"TakeDamage kaldt med skade: {playerDamage}");

        // Tjek om fjenden undviger angrebet
        float randomValue = Random.Range(0f, 1f);
        if (randomValue < evasionChance)
        {
            // Fjenden undviger angrebet
            //Debug.Log("Fjenden undveg angrebet!");
            // Vis "Miss" tekst
            if (missTextController != null)
            {
                missTextController.ShowMissText();
            }
            else
            {
                //Debug.LogWarning("MissTextController er ikke sat i Inspector!");
            }
            return;
        }

        // Beregn faktisk skade baseret p� forsvar.
        float actualDamage = Mathf.Max(playerDamage - defence, 0); // Skade kan ikke v�re negativ.
        //Debug.Log($"Faktisk skade efter forsvar: {actualDamage}");

        // Reduc�r fjendens liv.
        currentHealth -= actualDamage;

        //Debug.Log($"Fjenden tog {actualDamage} skade. Liv tilbage: {currentHealth}");

        // Opdater health bar.
        UpdateHealthBar();

        // Tjek, om fjenden d�r.
        if (currentHealth <= 0)
        {   
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // Tjek om fjenden allerede er dr�bt
        if (isDead) return;

        isDead = true; // Marker fjenden som dr�bt
        GameManager.Dr�bNisse(); // T�l kun �t drab

        if (lootDropper != null)
        {
            lootDropper.DropLoot();
        }

        // Destruer fjenden
        Destroy(gameObject);
    }
}