using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

class TnTAuthoring : MonoBehaviour
{
    public float CountDown = 3f;
    public float CountDownTimer;
    public float3 TnTPos = new float3(0,0,0);
}

class TnTBaker : Baker<TnTAuthoring>
{
    public override void Bake(TnTAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new TnTCountDown
        {
            CountDown = authoring.CountDown,
            CountDownTimer = authoring.CountDown,
            TnTPosition = authoring.TnTPos,
        });

    }
}
