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

    /// <summary>Move an inactive entity to the active entity buffer and set it as active.</summary>
    [BurstCompile]
    public static void GetNextEntity(ref Aspect aspect, out Entity entity)
    {
        entity = aspect.Entities[aspect.CurrentIndexRO].Entity;
        aspect.CurrentIndexRW = (aspect.CurrentIndexRO + 1) % aspect.PoolCountRO;
    }

    /// <summary>Instantiate prefabs and add it to the pool.</summary>
    [BurstCompile]
    public static void InstantiatePrefabs(
        ref EntityManager manager,
        ref Aspect aspect,
        in Entity prefab,
        in int count
    )
    {
        // Make sure that the pool is empty first
        aspect.Entities.Clear();
        aspect.CurrentIndexRW = 0;

        for (int e = 0; e < count; e++)
        {
            aspect.Entities.Add(new Element
            {
                Entity = manager.Instantiate(prefab)
            });
        }
    }
}
