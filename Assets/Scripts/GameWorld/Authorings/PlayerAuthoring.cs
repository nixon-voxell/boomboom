using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

class PlayerAuthoring : MonoBehaviour
{
    public float Speed = 1.0f;
    public float Dash = 2.0f;
}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);

        this.AddComponent<Tag_Player>(entity);
        this.AddComponent<SecondaryVelocity>(entity, new SecondaryVelocity { Value = 0.0f, });
        this.AddComponent<Speed>(entity, new Speed { Value = authoring.Speed, });
        this.AddComponent<Dash>(entity, new Dash { Direction = new float2(1.0f, 0.0f), Value = authoring.Dash, });
    }
}
