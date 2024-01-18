using Unity.Entities;
using Unity.Entities.Serialization;

public struct GameManagerSingleton : IComponentData
{
    public EntitySceneReference GameWorld;
    public Entity GameWorldEntity;
}
