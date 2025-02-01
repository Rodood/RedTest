using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Criei este script para ser a base dos valores que os personagens jogaveis e
/// os inimigos usam para gerenciar a vida, elementos de UI e tratativas de dano
/// e morte, podendo futuramente ser modificado para adicionar mais atributos comuns entre
/// ambas as classes.
/// </summary>


public abstract class HealthAndUI : MonoBehaviour
{
    [Header("Atribuição dos valores de vida dos personagens")]
    [SerializeField] protected Slider healthSlider; // Referência do slider de vida na UI
    [SerializeField] protected float maxHealth = 5;
    protected float currentHealth;

    // Posição de spawn para ser usada para teste no momento, mas pode ser bom adicionar para respawn do jogador
    [SerializeField] protected Transform spawnPos; 

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

    // Apesar do teste não ter pedido para colocar uma atribuição de morte, achei interessante colocar no momento
    protected abstract void Death();
}
