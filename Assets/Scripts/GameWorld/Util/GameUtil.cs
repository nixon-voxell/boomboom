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

    [BurstCompile]
    public static void CalculateTimeFromSeconds(
        in int inSeconds,
        out int hours,
        out int minutes,
        out int seconds
    )
    {
        const int HR_SECONDS = 60 * 60;
        const int MIN_SECONDS = 60;

        int secondsLeft = inSeconds;

        hours = secondsLeft / HR_SECONDS;
        secondsLeft -= hours * HR_SECONDS;

        minutes = secondsLeft / MIN_SECONDS;
        secondsLeft -= minutes * MIN_SECONDS;

        seconds = secondsLeft;
    }
}
