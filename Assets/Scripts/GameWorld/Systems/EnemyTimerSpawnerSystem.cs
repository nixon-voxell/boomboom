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

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity enemyEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();
        EnemySpawnerAspect enemyAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(enemyEntity);

        if (m_CurrentEnemyCount < enemyAsp.NumberEnemySpawn)
        {
            m_SpawnInterval = enemyAsp.SpawnInterval;
            m_SpawnTimer -= Time.deltaTime;

            if (m_SpawnTimer < 0)
            {
                EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

                int maxEnemiesToSpawn = Mathf.Min(5, enemyAsp.NumberEnemySpawn - m_CurrentEnemyCount);
                int numEnemiesToSpawn = Random.Range(1, maxEnemiesToSpawn + 1);

                for (int i = 0; i < numEnemiesToSpawn; i++)
                {
                    Entity newEnemy = commands.Instantiate(enemyAsp.EnemyPrefab);
                    LocalTransform newEnemyTransform = enemyAsp.GetRandomEnemyTransform();
                    commands.SetComponent<LocalTransform>(newEnemy, newEnemyTransform);
                    m_CurrentEnemyCount++;
                }

                commands.Playback(state.EntityManager);
                commands.Dispose();

                m_SpawnTimer = m_SpawnInterval;
            }
        }


    }
}


