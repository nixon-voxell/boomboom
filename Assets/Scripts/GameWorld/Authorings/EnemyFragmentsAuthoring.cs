using UnityEngine;
using Unity.Entities;

class EnemyFragmentsAuthoring : MonoBehaviour
{
    public GameObject[] Fragments;
}

class EnemyFragmentsBaker : Baker<EnemyFragmentsAuthoring>
{
    public override void Bake(EnemyFragmentsAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<Tag_EnemyFragmentsSingleton>(entity);
        DynamicBuffer<EnemyFragment> fragments = this.AddBuffer<EnemyFragment>(entity);

        foreach (GameObject fragment in authoring.Fragments)
        {
            fragments.Add(new EnemyFragment
            {
                Value = this.GetEntity(fragment, TransformUsageFlags.Dynamic)
            });
        }
    }
}
