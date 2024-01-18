using Unity.Entities;
using Unity.Entities.Serialization;

public struct GameManagerSingleton : IComponentData
{
    public EntitySceneReference GameWorld;
    public Entity GameWorldEntity;

    public EntitySceneReference EnvironmentWorld;
    public Entity EnvironmentWorldEntity;
}

public struct GameStateSingleton : IComponentData
{
    public GameState State;
}

public enum GameState
{
    Start,
    InGame,
    End,
    Pause,
}
