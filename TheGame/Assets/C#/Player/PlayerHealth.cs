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
    }

    void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHeath(int heath)
    {
        slider.value = heath;
    }
}