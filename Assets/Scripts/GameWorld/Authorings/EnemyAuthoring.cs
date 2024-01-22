using UnityEngine;
using Unity.Entities;

class EnemyAuthoring : MonoBehaviour
{
    public float Damage;
}

class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<Tag_Enemy>(entity);
        this.AddComponent<Damage>(entity, new Damage
        {
            Value = authoring.Damage,
        });

        this.AddComponent<Tag_FixXZRotation>(entity);
    }
}
