using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TnTAuthoring : MonoBehaviour
{
    public int countDown = 3;

    public class TnTBaker : Baker<TnTAuthoring> 
    {
        public override void Bake(TnTAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TnTCountDown
            {
                countDown = authoring.countDown,
            });
            
        }
    }
}
