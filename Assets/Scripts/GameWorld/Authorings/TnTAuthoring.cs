using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

class TnTAuthoring : MonoBehaviour
{
    public float CountDown = 3f;
    public float TnTRadius = 10f;
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
            TNTRadius = authoring.TnTRadius,
        });

    }
}
