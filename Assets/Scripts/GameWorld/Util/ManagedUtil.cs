using UnityEngine;
using Unity.Mathematics;

public static class ManagedUtil
{
    public static float4 ColorToFloat4(in Color color)
    {
        return new float4(color.r, color.g, color.b, color.a);
    }

    public static float2 Vector4ToFloat2(in Vector4 vector)
    {
        return (Vector2)vector;
    }
}
