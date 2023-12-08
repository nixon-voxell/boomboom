using Unity.Entities;
using Unity.Mathematics;

public struct UserInputSingleton : IComponentData
{
    public float2 MoveAxis;
    public bool Dash;

    public static readonly UserInputSingleton Default = new UserInputSingleton
    {
        MoveAxis = 0.0f,
        Dash = false,
    };
}
