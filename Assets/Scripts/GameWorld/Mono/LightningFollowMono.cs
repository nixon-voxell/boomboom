using UnityEngine;
using UnityEngine.VFX;

public class LightningFollowMono : MonoBehaviour
{
    public Transform FollowTransform;
    public VisualEffect Effect;

    public void LateUpdate()
    {
        this.Effect.SetVector3(ShaderID._Position, FollowTransform.position);
    }
}
