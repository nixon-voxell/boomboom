using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct EnemyComponent : IComponentData
{
    public float2 FieldDimensions;
    public int NumberEnemySpawn;
    public Entity EnemyPrefab;
}
