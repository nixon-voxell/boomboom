using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;

public struct GameManagerSingleton : IComponentData
{
    public SceneLoader GameWorld;
    public SceneLoader EnvironmentWorld;
}

public struct SceneLoader
{
    public EntitySceneReference Scene;
    public Entity Entity;

#if UNITY_EDITOR
    public static SceneLoader From(UnityEditor.SceneAsset scene)
    {
        return new SceneLoader
        {
            Scene = new EntitySceneReference(scene),
            Entity = Entity.Null,
        };
    }
#endif

    public void LoadScene(ref SystemState state)
    {
        this.Entity = SceneSystem.LoadSceneAsync(
            state.WorldUnmanaged, this.Scene
        );
    }

    public void UnloadScene(ref SystemState state)
    {
        SceneSystem.UnloadScene(
            state.WorldUnmanaged, this.Entity
        );

        this.Entity = Entity.Null;
    }

    public bool HasSceneLoaded(ref SystemState state)
    {
        return SceneSystem.IsSceneLoaded(
            state.WorldUnmanaged, this.Entity
        );
    }
}

public struct GameCurrStateSingleton : IComponentData
{
    public GameState Value;
}

public struct GameTargetStateSingleton : IComponentData
{
    public GameState Value;
}

public enum GameState
{
    Start,
    InGame,
    End,
}

public struct GameStatSingleton : IComponentData
{
    public int KillCount;
    public float SurvivalTime;

    public static GameStatSingleton Default()
    {
        return new GameStatSingleton
        {
            KillCount = 0,
            SurvivalTime = 0.0f,
        };
    }
}
