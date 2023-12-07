using UnityEngine;
using Unity.Entities;

class PlayerAuthoring : MonoBehaviour
{
    public float Speed = 1.0f;
}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);

        this.AddComponent<Tag_Player>(entity);
        this.AddComponent<Speed>(entity, new Speed { Value = authoring.Speed, });
    }
}
