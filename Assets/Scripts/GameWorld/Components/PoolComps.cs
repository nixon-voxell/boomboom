using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public static partial class Pool
{
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
        public readonly RefRW<CurrentIndex> CurrentIndex;
        public readonly DynamicBuffer<Element> Entities;

        public int PoolCountRO => this.Entities.Length;
        public ref int CurrentIndexRW => ref this.CurrentIndex.ValueRW.Value;
        public int CurrentIndexRO => this.CurrentIndex.ValueRO.Value;
    }

    [BurstCompile]
    /// <summary>Move an inactive entity to the active entity buffer and set it as active.</summary>
    public static void GetNextEntity(ref Aspect aspect, out Entity entity)
    {
        entity = aspect.Entities[aspect.CurrentIndexRO].Entity;
        aspect.CurrentIndexRW = (aspect.CurrentIndexRO + 1) % aspect.PoolCountRO;
    }
}
