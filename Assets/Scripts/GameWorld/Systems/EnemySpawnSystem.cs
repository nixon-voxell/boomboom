using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct EnemySpawnSystem : ISystem
{
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
        state.Enabled = false;

        Entity enemyEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();
        EnemySpawnerAspect enemyAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(enemyEntity);

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < enemyAsp.NumberEnemySpawn; i++)
        {
            Entity newCube = commands.Instantiate(enemyAsp.EnemyPrefab);
            LocalTransform newCubeTransform = enemyAsp.GetRandomEnemyTransform();
            commands.SetComponent<LocalTransform>(newCube, newCubeTransform);
        }

        commands.Playback(state.EntityManager);
    }
}
