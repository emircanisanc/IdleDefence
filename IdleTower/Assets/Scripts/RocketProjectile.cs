using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{
    [SerializeField] GameObject particle;
    [SerializeField] GameObject gfx;
    [SerializeField] float explosionRadius;
    [SerializeField] LayerMask enemyLayer;
    bool isDone;

    public override void FireProjectile(Transform startPoint, Vector3 target)
    {
        particle.SetActive(false);
        gfx.SetActive(true);
        base.FireProjectile(startPoint, target);
        isDone = false;
        enabled = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isDone)
            return;

        if (other.CompareTag("Enemy"))
        {
            isDone = true;
            Explode();
            return;
        }

        if (other.CompareTag("Ground"))
        {
            isDone = true;
            Explode();
            return;
        }
    }

    protected void Explode()
    {
        enabled = false;
        AudioManager.Instance.PlayExplosionSound(transform.position);
        particle.SetActive(true);
        gfx.SetActive(false);

        Collider[] result = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        if (result.Length > 0)
        {
            foreach (var coll in result)
            {
                coll.GetComponent<EnemyController>().ApplyDamage(damage);
            }
        }

        Invoke(nameof(DisableProjectile), 0.7f);
    }
}
