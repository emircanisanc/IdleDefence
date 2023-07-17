using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage = 50;
    [SerializeField] float moveSpeed = 5f;
    Vector3 direction;
    [SerializeField] float destroyTimerMax = 2.5f;
    float destroyTimer;

    public virtual void FireProjectile(Transform startPoint, Vector3 target)
    {
        transform.position = startPoint.position;
        this.direction = (target - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        gameObject.SetActive(true);
        destroyTimer = destroyTimerMax;
    }

    void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;

        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0)
            DisableProjectile();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().ApplyDamage(damage);
            DisableProjectile();
            return;
        }

        if (other.CompareTag("Ground"))
        {
            DisableProjectile();
            return;
        }
    }

    protected void DisableProjectile()
    {
        gameObject.SetActive(false);
    }
}
