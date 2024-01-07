using Unity.Entities;

public struct ExplosionPoolSingleton : IComponentData { }

public struct Explosion : IComponentData
{
    public float ImpulseForce;
    public float Radius;
}

public readonly partial struct ExplosionAspect : IAspect
{
    public readonly RefRW<Explosion> Explosion;
    public readonly RefRW<Damage> Damage;
}
