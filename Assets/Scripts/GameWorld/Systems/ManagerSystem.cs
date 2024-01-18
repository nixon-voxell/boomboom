using Unity.Entities;

public partial struct ManagerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingleton>();
        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<GameCurrStateSingleton, GameTargetStateSingleton>()
            .Build()
        );
    }

    public void OnStartRunning(ref SystemState state)
    {
        ref GameManagerSingleton gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;

        gameManager.EnvironmentWorld.LoadScene(ref state);

        AudioManager.Instance.PlayBgm("MainMenu");
    }

    public void OnStopRunning(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        ref GameCurrStateSingleton currState = ref SystemAPI.GetSingletonRW<GameCurrStateSingleton>().ValueRW;
        GameTargetStateSingleton targetState = SystemAPI.GetSingleton<GameTargetStateSingleton>();

        if (currState.Value == targetState.Value)
        {
            return;
        }

        switch (targetState.Value)
        {
            case GameState.InGame:
                ref GameManagerSingleton gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;
                gameManager.GameWorld.LoadScene(ref state);

                VirtualCameraMono.Instance.SetPriority(11);
                break;
        }

        // Update state after completing all state change related tasks.
        currState.Value = targetState.Value;
    }
}
