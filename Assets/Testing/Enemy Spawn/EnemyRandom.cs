using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct EnemyRandom : IComponentData
{
    public Unity.Mathematics.Random Value;
}
