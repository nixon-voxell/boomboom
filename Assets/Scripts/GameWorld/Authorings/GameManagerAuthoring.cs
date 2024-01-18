using Unity.Entities;
using Unity.Entities.Serialization;

#if UNITY_EDITOR
using UnityEngine;

class GameManagerAuthoring : MonoBehaviour
{
    public UnityEditor.SceneAsset GameWorld;
    public UnityEditor.SceneAsset EnvironmentWorld;
}
#endif

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
            GameWorld = new EntitySceneReference(authoring.GameWorld),
            GameWorldEntity = Entity.Null,

            EnvironmentWorld = new EntitySceneReference(authoring.EnvironmentWorld),
            EnvironmentWorldEntity = Entity.Null,
        });
    }
}
