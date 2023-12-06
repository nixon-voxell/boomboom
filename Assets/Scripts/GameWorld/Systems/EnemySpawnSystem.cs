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

        var enemyEntity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();
        var enemyAsp = SystemAPI.GetAspect<EnemySpawnerAspect>(enemyEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var i = 0; i < enemyAsp.NumberEnemySpawn; i++)
        {
            var newCube = ecb.Instantiate(enemyAsp.EnemyPrefab);
            var newCubeTransform = enemyAsp.GetRandomEnemyTransform();
            ecb.SetComponent<LocalTransform>(newCube, newCubeTransform);
        }

        ecb.Playback(state.EntityManager);
    }
}
