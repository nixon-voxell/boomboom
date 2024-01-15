using UnityEngine;
using Unity.Entities;

class ExplosionVfxAuthoring : MonoBehaviour
{
    public float VfxDuration;
}

class ExplosionVfxBaker : Baker<ExplosionVfxAuthoring>
{
    public override void Bake(ExplosionVfxAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);

        this.AddComponent<Timer>(entity, new Timer
        {
            TotalTime = authoring.VfxDuration,
            ElapsedTime = 0.0f,
        });

        this.AddComponent<Tag_ExplosionVfx>(entity);
    }
}
