using Unity.Entities;
using Unity.Transforms;

public partial struct ManagerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingleton>();
        state.RequireForUpdate<Tag_MascotSingleton>();
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

    public void OnStopRunning(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        ref GameCurrStateSingleton currState = ref SystemAPI.GetSingletonRW<GameCurrStateSingleton>().ValueRW;
        GameTargetStateSingleton targetState = SystemAPI.GetSingleton<GameTargetStateSingleton>();
        Entity mascotEntity = SystemAPI.GetSingletonEntity<Tag_MascotSingleton>();

        if (currState.Value == targetState.Value)
        {
            return;
        }

        ref GameManagerSingleton gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;
        EntityManager entityManager = state.EntityManager;

        switch (targetState.Value)
        {
            case GameState.Start:
                // Reload environmen world
                gameManager.EnvironmentWorld.UnloadScene(ref state);
                gameManager.EnvironmentWorld.LoadScene(ref state);

                // Enable mascot entity
                entityManager.SetEnabled(mascotEntity, true);
                entityManager.SetComponentData<LocalTransform>(mascotEntity, LocalTransform.Identity);

                // Reset target position
                PlayerTargetMono.Instance.TargetPosition = 0.0f;

                // Disable camera
                VirtualCameraMono.Instance.SetPriority(9);
                // Enable in start menu
                UiManagerMono.Instance.SetOnlyVisible(typeof(StartMenuMono));
                break;

            case GameState.InGame:
                gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;
                gameManager.GameWorld.LoadScene(ref state);

                // Disable mascot entity
                entityManager.SetEnabled(mascotEntity, false);

                // Make as main camera
                VirtualCameraMono.Instance.SetPriority(11);
                // Enable in game hud
                UiManagerMono.Instance.SetOnlyVisible(typeof(InGameHudMono));
                break;
        }

        // Update state after completing all state change related tasks.
        currState.Value = targetState.Value;
    }
}
