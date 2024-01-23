using Unity.Burst;
using Unity.Entities;

public partial struct PlayerActionAudioSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<GameCurrStateSingleton, GameTargetStateSingleton>()
            .Build()
        );

        state.RequireForUpdate<Tag_PlayerSingleton>();
        state.RequireForUpdate<UserInputSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        GameCurrStateSingleton currState = SystemAPI.GetSingleton<GameCurrStateSingleton>();
        // Player action is only available in game
        if (currState.Value != GameState.InGame)
        {
            return;
        }

        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        if (userInput.Dash)
        {
            AudioManager.Instance.PlaySfx("Dash");
        }

        if (userInput.Bomb)
        {
            AudioManager.Instance.PlaySfx("PlaceBomb");
        }
    }
}

[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionAudioSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ExplosionSingleton explosionSingleton = SystemAPI.GetSingleton<ExplosionSingleton>();

        foreach (
            RefRO<Explode> _ in
            SystemAPI.Query<RefRO<Explode>>()
        )
        {
            AudioManager.Instance.PlaySfx("Bomb");
        }
    }
}
