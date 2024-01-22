using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(DefaultFallDeathSystem))]
public partial class SpecialFallDeathSystem : ComponentSystemGroup { }

/// <summary>Disable entities when they fall too much.</summary>
public partial struct DefaultFallDeathSystem : ISystem
{
    public const float DEATH_HEIGHT = -40.0f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (worldTransform, entity) in
            SystemAPI.Query<LocalToWorld>()
            .WithAbsent<Tag_Enemy, Tag_PlayerSingleton>()
            .WithEntityAccess()
        )
        {
            if (worldTransform.Position.y < DEATH_HEIGHT)
            {
                // If it's just an ordinary physics body, disable it!
                // We avoid destroying entity as it may come from a pool.
                commands.SetEnabled(entity, false);
            }
        }

        commands.Playback(state.EntityManager);
    }
}

/// <summary>Kill enemies when they fall too much.</summary>
[UpdateInGroup(typeof(SpecialFallDeathSystem))]
public partial struct EnemyFallDeathSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnerSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Disabled enemy buffer
        DynamicBuffer<DisabledEnemy> disabledEnemies = SystemAPI.GetBuffer<DisabledEnemy>(
            SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>()
        );

        foreach (
            var (worldTransform, entity) in
            SystemAPI.Query<LocalToWorld>()
            .WithAll<Tag_Enemy>()
            .WithEntityAccess()
        )
        {
            if (worldTransform.Position.y < DefaultFallDeathSystem.DEATH_HEIGHT)
            {
                // DEATH!!!
                GameUtil.KillEnemy(ref commands, ref disabledEnemies, in entity);
            }
        }

        commands.Playback(state.EntityManager);
    }
}

/// <summary>Game over when player falls too much.</summary>
[UpdateInGroup(typeof(SpecialFallDeathSystem))]
public partial struct PlayerFallDeathSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<GameCurrStateSingleton, GameTargetStateSingleton>()
            .Build()
        );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        ref GameTargetStateSingleton targetState = ref SystemAPI.GetSingletonRW<GameTargetStateSingleton>().ValueRW;

        foreach (
            var (worldTransform, entity) in
            SystemAPI.Query<LocalToWorld>()
            .WithAll<Tag_PlayerSingleton>()
            .WithEntityAccess()
        )
        {
            if (worldTransform.Position.y < DefaultFallDeathSystem.DEATH_HEIGHT)
            {
                // Game over
                targetState.Value = GameState.End;
                commands.SetEnabled(entity, false);
            }
        }

        commands.Playback(state.EntityManager);
    }
}
