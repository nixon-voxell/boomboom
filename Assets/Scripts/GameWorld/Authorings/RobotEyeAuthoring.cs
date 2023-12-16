using UnityEngine;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

[AddComponentMenu("Entities/GameWorld/Robot Eye")]
class RobotEyeAuthoring : MonoBehaviour
{
    public Material Material;

    public float Speed = 1.0f;

    [Header("Eye Default Configs")]
    public RobotEye LeftEye;
    public RobotEye RightEye;

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

        // Robot material
        this.AddComponentObject<RobotBase>(entity, new RobotBase
        {
            Material = authoring.Material,
        });

        // Robot eyes
        RobotEyes eyes = new RobotEyes
        {
            Color = ManagedUtil.ColorToFloat4(authoring.Neutral),
            LeftEye = authoring.LeftEye,
            RightEye = authoring.RightEye,
        }.CopyCurrentDataToOrigin();

        this.AddComponent<RobotEyes>(entity, eyes);
        this.AddComponent<RobotEyesTarget>(entity, new RobotEyesTarget
        {
            Color = eyes.Color,
            LeftEye = eyes.LeftEye,
            RightEye = eyes.RightEye,
        });

        // Eye speed
        this.AddComponent<RobotEyeSpeed>(entity, new RobotEyeSpeed
        {
            Value = authoring.Speed
        });

        // Eye blink
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
