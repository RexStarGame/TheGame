using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{

    public Slider slider;
    public int MaxHealth = 100;
    public int CurrentHealth;
    

    private void Start()
    {
        CurrentHealth = MaxHealth;
        SetMaxHealth(MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        SetHealth(CurrentHealth);
        if(CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
   
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
