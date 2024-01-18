using Unity.Entities;
using Unity.Scenes;

public partial struct ManagerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingleton>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        RefRW<GameManagerSingleton> gameManager = SystemAPI.GetSingletonRW<GameManagerSingleton>();
        gameManager.ValueRW.GameWorldEntity = SceneSystem.LoadSceneAsync(
            state.WorldUnmanaged, gameManager.ValueRO.GameWorld
        );
    }

    public void OnStopRunning(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
    }
}
