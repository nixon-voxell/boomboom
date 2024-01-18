using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ItemSpawnerAuthoring : MonoBehaviour
{
    public GameObject Item;
}

public class ItemSpawnerBaker : Baker<ItemSpawnerAuthoring>
{
    public override void Bake(ItemSpawnerAuthoring authoring)
    {
        Entity spawnerEntity = this.GetEntity(TransformUsageFlags.None);

        this.AddComponent<ItemSpawnerSingleton>(spawnerEntity);
    }
}
