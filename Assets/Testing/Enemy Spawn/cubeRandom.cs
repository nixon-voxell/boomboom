using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct cubeRandom : IComponentData
{
    public Unity.Mathematics.Random Value;
}
