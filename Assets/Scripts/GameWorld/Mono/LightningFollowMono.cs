using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class LightningFollowMono : MonoBehaviour
{
    public Transform FollowTransform;
    public VisualEffect Effect;
    public float PlayRate = 3.0f;

    public void Awake()
    {
        this.Effect = this.GetComponent<VisualEffect>();
        this.Effect.playRate = this.PlayRate;
    }

    public void LateUpdate()
    {
        this.Effect.SetVector3(ShaderID._Position, FollowTransform.position);
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        this.Effect = this.GetComponent<VisualEffect>();
        this.Effect.playRate = this.PlayRate;
    }
#endif
}
