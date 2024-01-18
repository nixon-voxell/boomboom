using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public struct ItemSpawnerSingleton : IComponentData
{
    public Entity Item;
}

public struct SpawnerBoundary : IComponentData
{
    public Bounds ItemSpawnBoundary;
}

public readonly partial struct ItemSpawnerAspect : IAspect
{
    public readonly RefRO<LocalTransform> LocalTransform;
    public readonly RefRW<ItemSpawnerSingleton> ItemSpawnerSingleton;
    public readonly RefRW<SpawnerBoundary> ItemSpawnerBoundary;

    public ref ItemSpawnerSingleton SingletonRW => ref this.ItemSpawnerSingleton.ValueRW;
    public ref SpawnerBoundary BoundaryRW => ref this.ItemSpawnerBoundary.ValueRW;

    public Bounds GetSpawnerBoundary()
    {
        Vector3 centralPoint = LocalTransform.ValueRO.Position;
        float spawnerRadius = LocalTransform.ValueRO.Scale / 2;

        float maxX = centralPoint.x + spawnerRadius;
        float maxY = centralPoint.y + spawnerRadius;
        float maxZ = centralPoint.z + spawnerRadius;
        float minX = centralPoint.x - spawnerRadius;
        float minZ = centralPoint.z - spawnerRadius;

        float boundarySizeX = Mathf.Abs(maxX - minX);
        float boundarySizeZ = Mathf.Abs(maxZ - minZ);

        Bounds boundary = new Bounds();
        Vector3 minPoint = new Vector3(minX, maxY, minZ);
        Vector3 maxPoint = new Vector3(maxX, maxY, maxZ);
        boundary.SetMinMax(minPoint, maxPoint);

        return boundary;
    }
}
