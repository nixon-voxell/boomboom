using UnityEngine;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

[AddComponentMenu("Entities/GameWorld/Robot Eye")]
class RobotEyeAuthoring : MonoBehaviour
{
    public Material Material;

    public float Speed = 1.0f;

    [Header("Blink")]
    public float BlinkIntervalMin;
    public float BlinkIntervalMax;

    [Header("Mood")]
    [ColorUsage(false, true)]
    public Color Neutral;
    [ColorUsage(false, true)]
    public Color Angry;
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

        Random Random = Random.CreateFromIndex((uint)authoring.GetInstanceID());

        this.AddComponent<RobotEyeBlink>(entity, new RobotEyeBlink
        {
            IntervalMin = authoring.BlinkIntervalMin,
            IntervalMax = authoring.BlinkIntervalMax,
            TimeElapsed = Random.NextFloat(
                authoring.BlinkIntervalMin, authoring.BlinkIntervalMax
            ),
            Random = Random,
        });
    }
}
