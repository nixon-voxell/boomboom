using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class cubeMono : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberCubeSpaawn;
    public GameObject CubePrefab;
}

public class cubeBaker : Baker<cubeMono>
{
    [System.Obsolete]
    public override void Bake(cubeMono authoring)
    {
        AddComponent(new cubeComponent
        {
            FieldDimensions = authoring.FieldDimensions,
            NumberCubeSpaawn = authoring.NumberCubeSpaawn,
            CubePrefab = GetEntity(authoring.CubePrefab)
        });
    }
}
