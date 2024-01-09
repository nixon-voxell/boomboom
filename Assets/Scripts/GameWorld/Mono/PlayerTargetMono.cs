using UnityEngine;
using Unity.Mathematics;

public class PlayerTargetMono : MonoBehaviour
{
    public static float3 TargetPosition;

    public float Smoothing = 4.0f;

    private void Update()
    {
        // this.transform.position = math.lerp(
        //     this.transform.position, TargetPosition, Time.deltaTime * this.Smoothing
        // );

        this.transform.position = TargetPosition;
    }
}
