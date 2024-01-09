using UnityEngine;
using Unity.Entities;

class ExplosionPoolAuthoring : MonoBehaviour
{
    public int Count;
}

class ExplosionPoolBaker : Baker<ExplosionPoolAuthoring>
{
    public override void Bake(ExplosionPoolAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);

        this.AddComponent<ExplosionPoolSingleton>(entity);
        this.AddComponent<Pool.CurrentIndex>(entity);
        DynamicBuffer<Pool.Element> elementBuffer = this.AddBuffer<Pool.Element>(entity);

        for (int e = 0; e < authoring.Count; e++)
        {
            // Create and disable explosion entity
            Entity poolEntity = this.CreateAdditionalEntity(TransformUsageFlags.Dynamic, entityName: "ExplosionElement");

            elementBuffer.Add(new Pool.Element
            {
                Entity = poolEntity,
            });
        }
    }
}
