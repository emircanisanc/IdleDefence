using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomber : GunBase
{
    [SerializeField] Transform bombSpawnPoint;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject bomb;

    public int damage = 100;
    public float explosionRadius = 2f;

    Transform target;

    protected float nextFireTime;
    [SerializeField] protected float fireDuration = 2f;

    Collider[] colliders;

    void Awake()
    {
        bomb = Instantiate(bomb);
        bomb.SetActive(false);
    }

    protected override void HandleGunUpdate()
    {
        if (Time.time < nextFireTime)
            return;

        CheckArea();

        if (target == null)
            return;

        nextFireTime = Time.time + fireDuration;
        SpawnBomb();
    }

    public override void SetAttackMode()
    {
        base.SetAttackMode();
        //transform.localEulerAngles = new Vector3(279f,252f,104f);
    }

    protected void CheckArea()
    {
        colliders = Physics.OverlapSphere(transform.position, explosionRadius * 2, targetLayer);
        if (colliders.Length > 0)
        {
            target = colliders[0].transform;
            return;
        }
        target = null;
    }

    protected void SpawnBomb()
    {
        Vector3 targetPos = target.position;
        bomb.transform.position = bombSpawnPoint.position;
        bomb.SetActive(true);
        float jumpTime = Mathf.Min(1f, fireDuration / 2);
        bomb.transform.DOJump(targetPos, 0.5f, 1, jumpTime).OnComplete(() => Explode());
    }

    protected void Explode()
    {
        target = null;
        bomb.SetActive(false);

        colliders = Physics.OverlapSphere(bomb.transform.position, explosionRadius, targetLayer);
        if (colliders.Length > 0)
        {
            foreach(var coll in colliders)
            {
                if (coll.TryGetComponent<EnemyController>(out var enemy))
                {
                    enemy.ApplyDamage(damage);
                }
            }
        }
    }
}
