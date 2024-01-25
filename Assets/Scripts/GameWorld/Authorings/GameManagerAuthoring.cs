using Unity.Entities;

#if UNITY_EDITOR
using UnityEngine;

class GameManagerAuthoring : MonoBehaviour
{
    public UnityEditor.SceneAsset GameWorld;
    public UnityEditor.SceneAsset EnvironmentWorld;
}

class GameManagerBaker : Baker<GameManagerAuthoring>
{
    public override void Bake(GameManagerAuthoring authoring)
    {
        if (authoring.GameWorld == null || authoring.EnvironmentWorld == null)
        {
            return;
        }

        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<GameManagerSingleton>(entity, new GameManagerSingleton
        {
            GameWorld = SceneLoader.From(authoring.GameWorld),

            EnvironmentWorld = SceneLoader.From(authoring.EnvironmentWorld),
        });

        this.AddComponent<GameStatSingleton>(entity, GameStatSingleton.Default());
    }
}
#endif
