using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemySetupSystem : ISystem, ISystemStartStop //for onstartrunning & stoprunning
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyPoolSingleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        UnityEngine.Debug.Log("EnemySetupSystem");
        Entity entity = SystemAPI.GetSingletonEntity<EnemyPoolSingleton>();

        EnemyPoolSingleton poolSingleton = SystemAPI.GetSingleton<EnemyPoolSingleton>();
        Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(entity);

        // Instantiate all the enemy prefab and fill in the enemy pool
        EntityManager manager = state.EntityManager;
        Pool.InstantiatePrefabs(
            ref manager,
            ref poolAspect,
            poolSingleton.Prefab,
            poolSingleton.PoolCount
        );
    }


    public void OnStopRunning(ref SystemState state) { }
}

[UpdateAfter(typeof(EnemySetupSystem)), UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemyTimerSpawnerSystem : ISystem
{
    private float m_SpawnInterval;
    private float m_SpawnTimer;
    private int m_CurrentEnemyCount;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //system will always execute again everytime EnemySpawnerSingleton comp value(s) is modified
        state.RequireForUpdate<EnemySpawnerSingleton>();
        state.RequireForUpdate<EnemyPoolSingleton>();

        m_CurrentEnemyCount = 0;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnityEngine.Debug.Log("Update");
        Entity spawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();

        EnemySpawnerAspect spawnerAspect = SystemAPI.GetAspect<EnemySpawnerAspect>(spawnerEntity);
        EnemyPoolSingleton poolSingleton = SystemAPI.GetSingleton<EnemyPoolSingleton>();

        Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(spawnerEntity);
        DynamicBuffer<Pool.Element> spawnerBuffer = poolAspect.Entities;

        if (m_CurrentEnemyCount < poolSingleton.PoolCount)
        {
            m_SpawnInterval = spawnerAspect.SpawnIntervalRW;
            m_SpawnTimer -= Time.deltaTime;
            //(1) 1st spawn time will be instantaneous, but start at 2nd spawn time and after, always m_SpawnTimer = m_SpawnInterval

            if (m_SpawnTimer < 0)
            {
                EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

                int maxSpawnIncrement = 5; // max spawn wave
                int spawnIncrement = Random.Range(1, maxSpawnIncrement + 1); // Generate a random increment value

                int enemiesToSpawn = Mathf.Min(m_CurrentEnemyCount + spawnIncrement, poolSingleton.PoolCount - m_CurrentEnemyCount);
                /* EXPLAIN
                 * enemiesToSpawn: enemy spawn count on every wave
                 * 
                 * m_CurrentEnemyCount + spawnIncrement (A), enemyAsp.NumberEnemySpawn - m_CurrentEnemyCount (B)
                 * (A): everytime new wave must have more enemy count than before wave, and no fix enemy count increment 
                 * (B): that wave enemy max count spawn left
                */

                for (int i = 0; i < enemiesToSpawn; i++)    //Q2!!!!!! here how take entity from buffer and set them enable each
                {
                    Entity enemyEntity;
                    Pool.GetNextEntity(ref poolAspect, out enemyEntity);

                    LocalTransform enemyTransform = spawnerAspect.GetRandomEnemyTransform();  //get random transform
                    commands.SetComponent<LocalTransform>(enemyEntity, enemyTransform);
                    m_CurrentEnemyCount++;

                    commands.SetEnabled(enemyEntity, true);

                    //commands.Instantiate(enemyEnt);
                }

                commands.Playback(state.EntityManager);
                commands.Dispose();

                m_SpawnTimer = m_SpawnInterval; //(1) HERE, start from 2nd time
            }
        }
    }

}


