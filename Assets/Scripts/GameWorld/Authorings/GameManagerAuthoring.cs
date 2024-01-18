using Unity.Entities;
using Unity.Entities.Serialization;

#if UNITY_EDITOR
using UnityEngine;

class GameManagerAuthoring : MonoBehaviour
{
    public UnityEditor.SceneAsset GameWorld;
}
#endif

class GameManagerBaker : Baker<GameManagerAuthoring>
{
    public override void Bake(GameManagerAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<GameManagerSingleton>(entity, new GameManagerSingleton
        {
            GameWorld = new EntitySceneReference(authoring.GameWorld),
        });
    }
}
