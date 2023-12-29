using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct EnemyTimerSpawnerSystem : ISystem
{
    private float m_SpawnInterval;
    private float m_SpawnTimer;
    private int m_CurrentEnemyCount;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnerSingleton>();    
        //system will always execute again everytime this ecs component value(s) is modified

        m_CurrentEnemyCount = 0;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //return;

        Entity enemyEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>(); //get entity which contains this ecs component
        EnemySpawnerAspect enemyAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(enemyEntity);

        if (m_CurrentEnemyCount < enemyAsp.NumberEnemySpawn)    //NumberEnemySpawn: max count of enemy spawned in the game
        {
            m_SpawnInterval = enemyAsp.SpawnInterval;
            m_SpawnTimer -= Time.deltaTime;
            //(1) 1st spawn time will be instantaneous, but start at 2nd spawn time and after, always m_SpawnTimer = m_SpawnInterval

            if (m_SpawnTimer < 0)
            {
                EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

                int maxSpawnIncrement = 5; // max spawn wave
                int spawnIncrement = Random.Range(1, maxSpawnIncrement + 1); // Generate a random increment value

                int enemiesToSpawn = Mathf.Min(m_CurrentEnemyCount + spawnIncrement, enemyAsp.NumberEnemySpawn - m_CurrentEnemyCount);
                /* EXPLANATION
                 * enemiesToSpawn: enemy spawn count on every wave
                 * m_CurrentEnemyCount + spawnIncrement (A), enemyAsp.NumberEnemySpawn - m_CurrentEnemyCount (B)
                 * (A): everytime new wave must have more enemy count than before wave, and no fix enemy count increment 
                 * (B): that wave enemy max count spawn left
                */

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    Entity newEnemy = commands.Instantiate(enemyAsp.EnemyPrefab);
                    LocalTransform newEnemyTransform = enemyAsp.GetRandomEnemyTransform();  //get random transform
                    commands.SetComponent<LocalTransform>(newEnemy, newEnemyTransform);
                    m_CurrentEnemyCount++;

                    //commands.SetEnabled(newEnemy, false); //(HERE WORKED)
                }

                commands.Playback(state.EntityManager);
                commands.Dispose();

                m_SpawnTimer = m_SpawnInterval; //(1) HERE, start from 2nd time

                Debug.Log("Enemies spawned: " + m_CurrentEnemyCount.ToString());
            }
        }
    }
}


