using Unity.Entities;

public struct Health : IComponentData
{
    public float Value;

    public static Health Default()
    {
        return new Health
        {
            Value = 100.0f,
        };
    }
}
