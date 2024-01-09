using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]  //system runs at first and only once
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
       
        m_CurrentEnemyCount = 0;
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity spawnerEnt = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();
        EnemySpawnerAspect spawnerAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(spawnerEnt);

        Pool.Aspect poolAsp = SystemAPI.GetAspect<Pool.Aspect>(spawnerEnt);
        DynamicBuffer<Pool.Element> spawnerBuffer = poolAsp.Entities;

        //DynamicBuffer<Pool.Element> spawnerBuffer = SystemAPI.GetBuffer<Pool.Element>(spawnerEnt);

        for (int e = 0; e < spawnerAsp.MaxEnemySpawnCountRW; e++)   //Q1!!!! prefab no instantiate and also added into the spawnerBuffer
        {
            EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

            Entity enemyPrefabEnt = spawnerAsp.EnemyPrefabRO;
            commands.Instantiate(enemyPrefabEnt);
            commands.SetEnabled(enemyPrefabEnt, false);

            Pool.Element element = new Pool.Element { Entity = enemyPrefabEnt };
            spawnerBuffer.Add(element);
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity spawnerEnt = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();
        EnemySpawnerAspect spawnerAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(spawnerEnt);

        Pool.Aspect poolAsp = SystemAPI.GetAspect<Pool.Aspect>(spawnerEnt);
        DynamicBuffer<Pool.Element> spawnerBuffer = poolAsp.Entities;

        if (m_CurrentEnemyCount < spawnerAsp.MaxEnemySpawnCountRW)
        {
            m_SpawnInterval = spawnerAsp.SpawnIntervalRW;
            m_SpawnTimer -= Time.deltaTime;
            //(1) 1st spawn time will be instantaneous, but start at 2nd spawn time and after, always m_SpawnTimer = m_SpawnInterval

            if (m_SpawnTimer < 0)
            {
                EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

                int maxSpawnIncrement = 5; // max spawn wave
                int spawnIncrement = Random.Range(1, maxSpawnIncrement + 1); // Generate a random increment value

                int enemiesToSpawn = Mathf.Min(m_CurrentEnemyCount + spawnIncrement, spawnerAsp.MaxEnemySpawnCountRW - m_CurrentEnemyCount);
                /* EXPLAIN
                 * enemiesToSpawn: enemy spawn count on every wave
                 * 
                 * m_CurrentEnemyCount + spawnIncrement (A), enemyAsp.NumberEnemySpawn - m_CurrentEnemyCount (B)
                 * (A): everytime new wave must have more enemy count than before wave, and no fix enemy count increment 
                 * (B): that wave enemy max count spawn left
                */

                for (int i = 0; i < enemiesToSpawn; i++)    //Q2!!!!!! here how take entity from buffer and set them enable each
                {
                    Entity enemyEnt;
                    Pool.GetNextEntity(ref poolAsp, out enemyEnt);

                    commands.SetEnabled(enemyEnt, true);

                    LocalTransform enemyTransform = spawnerAsp.GetRandomEnemyTransform();  //get random transform
                    commands.SetComponent<LocalTransform>(enemyEnt, enemyTransform);
                    m_CurrentEnemyCount++;

                    //commands.Instantiate(enemyEnt);
                }

                commands.Playback(state.EntityManager);
                commands.Dispose();

                m_SpawnTimer = m_SpawnInterval; //(1) HERE, start from 2nd time
            }
        }
    }

}


