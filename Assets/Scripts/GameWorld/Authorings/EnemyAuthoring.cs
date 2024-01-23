using UnityEngine;
using Unity.Entities;

class EnemyAuthoring : MonoBehaviour
{
    public float AttackCoolDown = 1.0f;
}

class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<Tag_Enemy>(entity);
        this.AddComponent<Tag_FixXZRotation>(entity);

        this.AddComponent<EnemyCanAttack>(entity);
        this.SetComponentEnabled<EnemyCanAttack>(entity, false);
        this.AddComponent<Timer>(entity, new Timer
        {
            TotalTime = authoring.AttackCoolDown,
            ElapsedTime = authoring.AttackCoolDown,
        });
    }
}
