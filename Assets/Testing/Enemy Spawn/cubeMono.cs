using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//these components can only be seen in non-runtime inspector
public class cubeMono : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberCubeSpawn;
    public GameObject CubePrefab;
    public uint RandomSeed;
}

//to let above components can also be seen in runtime inspector, we use baker
public class cubeBaker : Baker<cubeMono>
{
    [System.Obsolete]
    public override void Bake(cubeMono authoring)   //values above will be assigned into reference components (scripts) added below
    {
        AddComponent(new cubeComponent
        {
            FieldDimensions = authoring.FieldDimensions,
            NumberCubeSpawn = authoring.NumberCubeSpawn,
            CubePrefab = GetEntity(authoring.CubePrefab)
        });

        AddComponent(new cubeRandom
        {
            Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
        });
    }
}
