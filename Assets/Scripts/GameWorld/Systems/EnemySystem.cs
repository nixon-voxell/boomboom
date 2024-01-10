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
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // system will always execute again everytime EnemySpawnerSingleton comp value(s) is modified
        state.RequireForUpdate<EnemySpawnerSingleton>();
        state.RequireForUpdate<EnemyPoolSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity spawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();

        // Get spawner components
        EnemySpawnerAspect spawnerAspect = SystemAPI.GetAspect<EnemySpawnerAspect>(spawnerEntity);

        // Get timer
        RefRW<Timer> timer = SystemAPI.GetComponentRW<Timer>(spawnerEntity);

        // Get pooling components
        EnemyPoolSingleton poolSingleton = SystemAPI.GetSingleton<EnemyPoolSingleton>();

        // Get pool aspect
        Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(spawnerEntity);
        DynamicBuffer<Pool.Element> spawnerBuffer = poolAspect.Entities;

        if (spawnerAspect.ActiveEnemyCountRO < poolSingleton.PoolCount)
        {
            //(1) 1st spawn time will be instantaneous, but start at 2nd spawn time and after, always m_SpawnTimer = m_SpawnInterval

            if (timer.ValueRW.Update(SystemAPI.Time.DeltaTime))
            {
                timer.ValueRW.ElapsedTime = 0.0f;
                EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

                int maxSpawnIncrement = 5; // max spawn wave
                int spawnIncrement = Random.Range(1, maxSpawnIncrement + 1); // Generate a random increment value

                // Every new wave must have more enemy count than before wave, and no fix enemy count increment.
                int targetSpawnCount = spawnerAspect.ActiveEnemyCountRO + spawnIncrement;
                // Number of enemies left.
                int enemyLeft = poolSingleton.PoolCount - spawnerAspect.ActiveEnemyCountRO;
                // The final amount of enemies to spawn.
                int spawnCount = Mathf.Min(targetSpawnCount, enemyLeft);

                for (int i = 0; i < spawnCount; i++)
                {
                    Entity enemyEntity;
                    Pool.GetNextEntity(ref poolAspect, out enemyEntity);

                    LocalTransform enemyTransform = spawnerAspect.GetRandomEnemyTransform();  //get random transform
                    commands.SetComponent<LocalTransform>(enemyEntity, enemyTransform);
                    spawnerAspect.ActiveEnemyCountRW++;

                    commands.SetEnabled(enemyEntity, true);

                    //commands.Instantiate(enemyEnt);
                }

                commands.Playback(state.EntityManager);
            }
        }
    }

}
