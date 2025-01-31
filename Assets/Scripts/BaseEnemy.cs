using System.Collections;
using UnityEngine;

public class BaseEnemy : HealthAndUI
{
    [SerializeField] GameObject hitVFX;

    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);

        StartCoroutine(HitVFX());
    }

    IEnumerator HitVFX()
    {
        hitVFX.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitVFX.SetActive(false);
    }

    protected override void Death()
    {
        throw new System.NotImplementedException();
    }
}
