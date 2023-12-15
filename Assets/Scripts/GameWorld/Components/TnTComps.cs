using Unity.Entities;
using Unity.Mathematics;

public struct TnTCountDown : IComponentData
{
    public float CountDown;
    public float CountDownTimer;
    public float3 TnTPosition;
}
