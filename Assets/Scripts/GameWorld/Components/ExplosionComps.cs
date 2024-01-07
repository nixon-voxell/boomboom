using Unity.Entities;

public struct Explosion : IComponentData
{
    public float ImpulseForce;
    public float Radius;
}
