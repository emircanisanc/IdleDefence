using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    EnemyController[] enemies;
    int enemyCount;
    int deadEnemyCounter;

    void Start()
    {
        enemies = FindObjectsOfType<EnemyController>();

        foreach (var enemy in enemies)
            enemy.enabled = false;
    }

    public void StartSpawn()
    {
        enemyCount = enemies.Length;
        foreach (var enemy in enemies)
            enemy.enabled = true;
    }

    public void OnEnemyDied()
    {
        deadEnemyCounter++;
        if (deadEnemyCounter >= enemyCount)
        {
            GameManager.Instance.EndGame();
        }
    }   
}
