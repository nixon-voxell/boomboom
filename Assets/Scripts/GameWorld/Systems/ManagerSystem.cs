using Unity.Entities;

public partial struct ManagerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingleton>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        RefRW<GameManagerSingleton> gameManager = SystemAPI.GetSingletonRW<GameManagerSingleton>();

        gameManager.ValueRW.EnvironmentWorld.LoadScene(ref state);

        AudioManager.Instance.PlayBgm("Theme");
    }

    public void OnStopRunning(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
    }
}
