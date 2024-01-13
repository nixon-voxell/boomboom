using Unity.Entities;

public struct ExplosionSingleton : IComponentData
{
    public float ShakeAmplitude;
    public float ShakeFrequency;
}

public struct Explode : IComponentData, IEnableableComponent
{
    public Entity LandmineEntity;
}

public struct ExplosionForce : IComponentData
{
    public float Value;
}

public struct ExplosionRadius : IComponentData
{
    public float Value;
}

public readonly partial struct ExplosionAspect : IAspect
{
    public readonly RefRW<ExplosionForce> Force;
    public readonly RefRW<ExplosionRadius> Radius;
    public readonly RefRW<Damage> Damage;

    public ref float ForceRW => ref this.Force.ValueRW.Value;
    public ref float RadiusRW => ref this.Radius.ValueRW.Value;
    public ref float DamageRW => ref this.Damage.ValueRW.Value;
}

public struct ExplosionVfxPoolSingleton : IComponentData
{
    public Entity Prefab;
    public int PoolCount;
}

public struct LandminePoolSingleton : IComponentData
{
    public Entity Prefab;
    public int PoolCount;
}
