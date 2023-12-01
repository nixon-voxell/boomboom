using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public readonly partial struct cubeAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<cubeComponent> _cubeComponent;
    private readonly RefRO<cubeRandom> _cubeRandom;

    public int numberCubeSpawn => _cubeComponent.ValueRO.NumberCubeSpawn;
    public Entity cubePrefab => _cubeComponent.ValueRO.CubePrefab;
}
