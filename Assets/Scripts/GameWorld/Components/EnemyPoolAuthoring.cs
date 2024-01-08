using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/* SHORT FORM
 * 
 * Comp = component
 * Ent = entity
 * Aut = authoring
 */

public class EnemyPoolAuthoring : MonoBehaviour
{
    public int Count;   //Count: total enemyEnt active count
}

public class EnemyPoolBaker : Baker<EnemyPoolAuthoring>
{
    public override void Bake(EnemyPoolAuthoring authoring)
    {
        int countAut = authoring.Count;

        /* STEP 1: GET ENTITY AND ADD COMPONENTS */

        Entity enemyEnt = GetEntity(TransformUsageFlags.Dynamic);
        Pool.Count countComp = new Pool.Count() { Value = countAut };
        Pool.CurrentIndex indexComp = new Pool.CurrentIndex();

        AddComponent<EnemySpawnerSingleton>(enemyEnt);
        AddComponent<Pool.Count>(enemyEnt, countComp);
        AddComponent<Pool.CurrentIndex>(enemyEnt, indexComp);

        /* STEP 2: ADD ENTITY TO BUFFER */

        DynamicBuffer<Pool.Element> enemyBuffer = AddBuffer<Pool.Element>(enemyEnt);

        for (int e = 0; e < authoring.Count; e++)   //e: current enemyEnt count
        {
            Entity poolEnt = CreateAdditionalEntity(TransformUsageFlags.Dynamic, entityName: "Enemy Element");

            AddComponent<Disabled>(poolEnt);
            AddComponent<Damage>(poolEnt);

            Pool.Element buffer = new Pool.Element { Entity = poolEnt };
            enemyBuffer.Add(buffer);
        }
    }
}
