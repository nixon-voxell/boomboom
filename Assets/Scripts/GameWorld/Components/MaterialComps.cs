using Unity.Mathematics;
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Position")]
public struct _Position : IComponentData
{
    public float3 Value;
}
