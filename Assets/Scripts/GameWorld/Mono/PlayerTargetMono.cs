using Unity.Mathematics;

public class PlayerTargetMono : SingletonMono<PlayerTargetMono>
{
    public float3 TargetPosition;

    private void Update()
    {
        this.transform.position = TargetPosition;
    }
}
