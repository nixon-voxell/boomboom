using UnityEngine;
using Unity.Mathematics;

public class PlayerTargetMono : MonoBehaviour
{
    public static float3 TargetPosition;

    private void Update()
    {
        this.transform.position = TargetPosition;
    }
}
