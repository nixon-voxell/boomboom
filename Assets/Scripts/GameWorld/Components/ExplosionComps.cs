using Unity.Entities;

public struct ExplosionPoolSingleton : IComponentData { }

public struct Explosion : IComponentData
{
    public float ImpulseForce;
    public float Radius;
}

