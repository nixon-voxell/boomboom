using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct cubeComponent : IComponentData
{
    public float2 FieldDimensions;
    public int NumberCubeSpawn;
    public Entity CubePrefab;
}
