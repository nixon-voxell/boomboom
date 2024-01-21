using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public static class GameUtil
{
    [BurstCompile]
    public static void KillEnemy(
        ref EntityCommandBuffer commands,
        ref DynamicBuffer<DisabledEnemy> disabledEnemies,
        in Entity enemyEntity
    )
    {
        // Disable entity
        commands.SetEnabled(enemyEntity, false);

        // Add entity to disabled buffer
        disabledEnemies.Add(new DisabledEnemy
        {
            Entity = enemyEntity,
        });
    }
}
