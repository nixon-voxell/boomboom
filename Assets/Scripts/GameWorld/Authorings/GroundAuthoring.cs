using UnityEngine;
using Unity.Entities;

class GroundAuthoring : MonoBehaviour
{
}

class GroundBaker : Baker<GroundAuthoring>
{
    public override void Bake(GroundAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Renderable);

        this.AddComponent<_Position>(entity);
        this.AddComponent<Tag_GroundSingleton>(entity);
    }
}
