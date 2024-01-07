using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public static partial class Pool
{
    public struct Count : IComponentData
    {
        public int Value;
    }

    public struct CurrentIndex : IComponentData
    {
        public int Value;
    }

    public struct Element : IBufferElementData
    {
        public Entity Entity;
    }

    public readonly partial struct Aspect : IAspect
    {
        public readonly RefRO<Count> PoolCount;
        public readonly RefRW<CurrentIndex> CurrentIndex;
        public readonly DynamicBuffer<Element> Entities;

        public int PoolCountRO => this.PoolCount.ValueRO.Value;
        public ref int CurrentIndexRW => ref this.CurrentIndex.ValueRW.Value;
        public int CurrentIndexRO => this.CurrentIndex.ValueRO.Value;
    }

    [BurstCompile]
    /// <summary>Move an inactive entity to the active entity buffer and set it as active.</summary>
    public static void GetNextEntity(
        ref EntityCommandBuffer commands,
        ref Aspect aspect,
        out Entity entity
    )
    {
        aspect.CurrentIndexRW = (aspect.CurrentIndexRO + 1) % aspect.PoolCountRO;
        entity = aspect.Entities[aspect.CurrentIndexRO].Entity;
    }
}
