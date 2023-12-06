using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//these components can only be seen in non-runtime inspector
public class EnemyMono : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberEnemySpawn;
    public GameObject EnemyPrefab;
    public uint RandomSeed;
}

//to let above components can also be seen in runtime inspector, we use baker
public class EnemyBaker : Baker<EnemyMono>
{
    [System.Obsolete]
    public override void Bake(EnemyMono authoring)   //values above will be assigned into reference components (scripts) added below
    {
        AddComponent(new EnemyComponent
        {
            FieldDimensions = authoring.FieldDimensions,
            NumberEnemySpawn = authoring.NumberEnemySpawn,
            EnemyPrefab = GetEntity(authoring.EnemyPrefab)
        });

        AddComponent(new EnemyRandom
        {
            Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
        });
    }
}
