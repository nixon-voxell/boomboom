using Unity.Entities;
using Unity.Mathematics;

public struct UserInputSingleton : IComponentData
{
    public float2 MoveAxis;
    public bool IsMoving;
    public bool Dash;

    public static readonly UserInputSingleton Default = new UserInputSingleton
    {
        MoveAxis = 0.0f,
        IsMoving = false,
        Dash = false,
    };
}
