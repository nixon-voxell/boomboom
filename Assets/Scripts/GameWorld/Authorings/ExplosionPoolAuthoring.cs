using UnityEngine;
using Unity.Entities;

class ExplosionPoolAuthoring : MonoBehaviour
{
    public int Count;

    [Header("Camera shake")]
    public float ShakeAmplitude = 1.0f;
    public float ShakeFrequency = 1.0f;

    [Header("Explosion Config")]
    public float ExplosionTimer = 3.0f;
    public float ExplosionRadius = 5.0f;
    public float ExplosionForce = 10.0f;
}

class ExplosionPoolBaker : Baker<ExplosionPoolAuthoring>
{
    public override void Bake(ExplosionPoolAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<ExplosionSingleton>(entity, new ExplosionSingleton
        {
            ShakeAmplitude = authoring.ShakeAmplitude,
            ShakeFrequency = authoring.ShakeFrequency,
        });
        this.AddComponent<Pool.CurrentIndex>(entity);
        DynamicBuffer<Pool.Element> elementBuffer = this.AddBuffer<Pool.Element>(entity);

        for (int e = 0; e < authoring.Count; e++)
        {
            // Create and disable explosion entity
            Entity poolEntity = this.CreateAdditionalEntity(TransformUsageFlags.Dynamic, entityName: "ExplosionElement");

            this.AddComponent<Explode>(poolEntity);
            this.AddComponent<ExplosionForce>(poolEntity, new ExplosionForce
            {
                Value = authoring.ExplosionForce,
            });
            this.AddComponent<ExplosionRadius>(poolEntity, new ExplosionRadius
            {
                Value = authoring.ExplosionRadius,
            });
            this.AddComponent<Timer>(poolEntity, new Timer
            {
                TotalTime = authoring.ExplosionTimer,
                ElapsedTime = 0.0f,
            });

            elementBuffer.Add(new Pool.Element
            {
                Entity = poolEntity,
            });
        }
    }
}
