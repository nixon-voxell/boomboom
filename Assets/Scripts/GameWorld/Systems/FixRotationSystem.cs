using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct FixXZRotation : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            RefRW<LocalTransform> transform in
            SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAll<Tag_FixXZRotation>()
        )
        {
            transform.ValueRW.Rotation.value.x = 0.0f;
            transform.ValueRW.Rotation.value.z = 0.0f;
        }
    }
}
