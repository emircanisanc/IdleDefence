using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public GameObject dieParticle;
    public int damage = 10;
    public int healthMax = 100;
    int health;
    public bool isAlive = true;
    NavMeshAgent ai;
    Collider coll;

    void Awake()
    {
        ai = GetComponent<NavMeshAgent>();
        coll = GetComponent<Collider>();
    }

    void OnEnable()
    {
        health = healthMax;    
        isAlive = true;
        gameObject.SetActive(true);
        ai.isStopped = false;
        coll.enabled = true;
    }

    void Update()
    {
        if (!isAlive)
            return;
        
        ai.SetDestination(Tower.Instance.transform.position);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Tower"))
        {
            if (other.TryGetComponent<Tower>(out var tower))
            {
                tower.ApplyDamage(damage);
                Die();
            }
        }    
    }

    public void ApplyDamage(int damage)
    {
        if (!isAlive)
            return;

        health -= damage;
        if (health <= 0)
            Explode();
    }

    private void Die()
    {
        coll.enabled = false;
        ai.isStopped = true;
        isAlive = false;
        enabled = false;
        gameObject.SetActive(false);
        EnemySpawner.Instance.OnEnemyDied();
    }

    private void Explode()
    {
        AudioManager.Instance.PlayEnemyDieSound(transform.position);
        dieParticle.transform.SetParent(null);
        dieParticle.SetActive(true);
        Die();
    }
}
