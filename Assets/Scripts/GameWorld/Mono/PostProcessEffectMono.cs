using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Unity.Mathematics;

public class PostProcessEffect : SingletonMono<PostProcessEffect>
{
    [SerializeField] private Volume m_Volume;
    [Header("Default Values")]
    [SerializeField] private float m_ChromaticDefault;
    [SerializeField] private float m_DistortDefault;

    [Header("Return Speeds")]
    [SerializeField] private float m_ChromaticReturnSpeed;
    [SerializeField] private float m_DistortReturnSpeed;

    private ChromaticAberration m_Chromatic;
    private LensDistortion m_Distort;
    [HideInInspector] public float ChromaticIntensity;
    [HideInInspector] public float DistortIntensity;

    private void Start()
    {
        this.m_Volume.profile.TryGet<ChromaticAberration>(out this.m_Chromatic);
        this.m_Volume.profile.TryGet<LensDistortion>(out this.m_Distort);
        this.ChromaticIntensity = this.m_Chromatic.intensity.value;
    }

    private void Update()
    {
        this.m_Chromatic.intensity.value = this.ChromaticIntensity;
        this.m_Distort.intensity.value = this.DistortIntensity;

        this.ChromaticIntensity = math.lerp(
            this.ChromaticIntensity, this.m_ChromaticDefault,
            Time.deltaTime * this.m_ChromaticReturnSpeed
        );

        this.DistortIntensity = math.lerp(
            this.DistortIntensity, this.m_DistortDefault,
            Time.deltaTime * this.m_DistortReturnSpeed
        );
    }
}
