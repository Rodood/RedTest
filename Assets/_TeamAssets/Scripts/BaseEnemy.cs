using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Um script criado como um esboço de um inimigo básico, podendo ser usado como base para
/// futuramente criar diversos tipos de inimigos como chefes e outros.
/// </summary>

public class BaseEnemy : HealthAndUI
{
    [SerializeField] protected Rigidbody rb;

    [SerializeField] protected GameObject hitVFX;

    /* 
     * De forma ideal, o valor da força do knockback estaria no script do jogador que lida com
     * os ataques dele e a tratativa, porém coloquei assim por conta da falta de tempo para 
     * desenvolver melhor os scripts 
     */
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected float knockbackDuration = 0.2f;

    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);

        StartCoroutine(HitVFX());
    }

    public void Knockback(Vector3 _hitDirection)
    {
        StartCoroutine(ApplyKnockback(_hitDirection));
    }

    protected IEnumerator HitVFX()
    {
        hitVFX.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitVFX.SetActive(false);
    }

    protected IEnumerator ApplyKnockback(Vector3 _hitDirection)
    {
        if (rb != null)
        {
            Vector3 force = _hitDirection.normalized * knockbackForce;
            rb.AddForce(force, ForceMode.Impulse);
            yield return new WaitForSeconds(knockbackDuration); // O tempo que o inimigo sofre o knockback
            rb.linearVelocity = Vector3.zero; // Para o movimento
        }
    }

    // Caso morra, no momento irá voltar a posição inicial com a vida cheia
    protected override void Death()
    {
        transform.position = spawnPos.position;

        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }
}
