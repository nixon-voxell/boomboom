using UnityEngine;
using Unity.Entities;

class MascotAuthoring : MonoBehaviour
{
}

class MascotBaker : Baker<MascotAuthoring>
{
    public override void Bake(MascotAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Renderable);

        this.AddComponent<Tag_MascotSingleton>(entity);
    }
}
