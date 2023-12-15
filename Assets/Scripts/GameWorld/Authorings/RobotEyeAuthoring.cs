using UnityEngine;
using Unity.Entities;

[AddComponentMenu("Entities/GameWorld/Robot Eye")]
class RobotEyeAuthoring : MonoBehaviour
{
    public Material Material;

    public float Speed = 1.0f;

    public float BlinkingDuration;
    public float BlinkingIntervalMin;
    public float BlinkingIntervalMax;
}

class RobotEyeBaker : Baker<RobotEyeAuthoring>
{
    public override void Bake(RobotEyeAuthoring authoring)
    {
        if (authoring.Material == null)
        {
            return;
        }

        Entity entity = this.GetEntity(TransformUsageFlags.Renderable);

        Material material = authoring.Material;

        this.AddComponentObject<RobotBase>(entity, new RobotBase
        {
            Material = authoring.Material,
        });

        this.AddComponent<RobotEyeSpeed>(entity, new RobotEyeSpeed
        {
            Value = authoring.Speed
        });
    }
}
