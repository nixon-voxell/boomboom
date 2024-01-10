using UnityEngine;
using Unity.Entities;

class ExplosionVfxPoolAuthoring : MonoBehaviour
{
    public int PoolCount;
    public GameObject Prefab;
}

class ExplosionVfxPoolBaker : Baker<ExplosionVfxPoolAuthoring>
{
    public override void Bake(ExplosionVfxPoolAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<ExplosionVfxPoolSingleton>(entity, new ExplosionVfxPoolSingleton
        {
            PoolCount = authoring.PoolCount,
            Prefab = this.GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
        });
        this.AddComponent<Pool.CurrentIndex>(entity);
        DynamicBuffer<Pool.Element> elementBuffer = this.AddBuffer<Pool.Element>(entity);
    }
}
