using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

class TnTAuthoring : MonoBehaviour
{
    public float CountDown = 3f;
    public float CountDownTimer;
    public float Radius;
}

class TnTBaker : Baker<TnTAuthoring>
{
    public override void Bake(TnTAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new TnT
        {
            CountDown = authoring.CountDown,
            CountDownTimer = authoring.CountDown,
            Radius = authoring.Radius,
        });

    }
}
