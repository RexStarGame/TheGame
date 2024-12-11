using UnityEngine;
using UnityEngine.UI;

public class JulemandensHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Refererer til UI-slideren
    private JulemandensHealth julemandensHealth; // Refererer til julemandens health script

    private void Start()
    {
        // Find julemandens health script
        julemandensHealth = FindObjectOfType<JulemandensHealth>();

        if (julemandensHealth != null)
        {
            // Indstil max health for slideren
            healthSlider.maxValue = julemandensHealth.maxHealth;
            healthSlider.value = julemandensHealth.maxHealth;
        }
        else
        {
            Debug.LogError("JulemandensHealth script ikke fundet!");
        }
    }

    private void Update()
    {
        if (julemandensHealth != null)
        {
            // Opdater sliderens værdi baseret på julemandens aktuelle liv
            healthSlider.value = julemandensHealth.currentHealth;
        }
    }
}
