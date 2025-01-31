using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthAndUI : MonoBehaviour
{
    [SerializeField] protected Slider healthSlider;
    [SerializeField] protected float maxHealth = 5;
    protected float currentHealth;

    protected virtual void Start()
    {
        healthSlider.maxValue = maxHealth;
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }
    
    public virtual void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0) 
        { 
            currentHealth = 0;

            Death();
        }
    }

    protected abstract void Death();
}
