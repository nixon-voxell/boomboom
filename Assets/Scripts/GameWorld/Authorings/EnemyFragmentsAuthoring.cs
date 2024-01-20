using UnityEngine;
using Unity.Entities;

class EnemyFragmentsAuthoring : MonoBehaviour
{
    public int PoolCount;
    public GameObject[] Fragments;
}

class EnemyFragmentsBaker : Baker<EnemyFragmentsAuthoring>
{
    public override void Bake(EnemyFragmentsAuthoring authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<Tag_EnemyFragmentsSingleton>(entity);
        DynamicBuffer<EnemyFragmentPool> fragmentPools = this.AddBuffer<EnemyFragmentPool>(entity);

        foreach (GameObject fragment in authoring.Fragments)
        {
            // Create fragment pool entity
            Entity poolEntity = this.CreateAdditionalEntity(TransformUsageFlags.None, entityName: fragment.name + "Pool");
            // Add pool component
            this.AddComponent<Pool.CurrentIndex>(poolEntity);
            DynamicBuffer<Pool.Element> elementBuffer = this.AddBuffer<Pool.Element>(poolEntity);

            // Add newly created fragment pool entity to the fragment pool buffer
            fragmentPools.Add(new EnemyFragmentPool
            {
                PoolEntity = poolEntity,
                FragmentPrefab = this.GetEntity(fragment, TransformUsageFlags.Dynamic),
                PoolCount = authoring.PoolCount
            });
        }
    }
}
