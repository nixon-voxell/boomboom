using UnityEngine;
using Unity.Entities;

class PlayerAuthoring : MonoBehaviour
{
    public float Speed;
    public float Dash;
    public float LookSpeed;
}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);

        this.AddComponent<Tag_PlayerSingleton>(entity);
        this.AddComponent<SecondaryVelocity>(entity, new SecondaryVelocity { Value = 0.0f, });
        this.AddComponent<Speed>(entity, new Speed { Value = authoring.Speed, });
        this.AddComponent<Dash>(entity, new Dash { Value = authoring.Dash, });
        this.AddComponent<LookSpeed>(entity, new LookSpeed { Value = authoring.LookSpeed });

        this.AddComponent<Tag_FixXZRotation>(entity);
    }
}
