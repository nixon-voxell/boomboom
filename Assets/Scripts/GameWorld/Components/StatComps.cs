using Unity.Entities;

public struct Health : IComponentData
{
    public float Value;
}

public struct Damage : IComponentData
{
    public float Value;
}

public struct Speed : IComponentData
{
    public float Value;
}
