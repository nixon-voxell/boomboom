using UnityEngine;
using Unity.Entities;

class LandminePoolAuthoring : MonoBehaviour
{
    public int PoolCount;
    public GameObject Prefab;
}

class LandminePoolBaker : Baker<LandminePoolAuthoring>
{
    public override void Bake(LandminePoolAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<LandminePoolSingleton>(entity, new LandminePoolSingleton
        {
            PoolCount = authoring.PoolCount,
            Prefab = this.GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
        });
        this.AddComponent<Pool.CurrentIndex>(entity);
        DynamicBuffer<Pool.Element> elementBuffer = this.AddBuffer<Pool.Element>(entity);
    }
}
