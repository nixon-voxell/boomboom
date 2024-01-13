using UnityEngine;
using Unity.Mathematics;
using Cinemachine;

public class VirtualCameraMono : SingletonMono<VirtualCameraMono>
{
    [Tooltip("The speed that the camera will reduce the noice amplitude and frequency.")]
    [SerializeField] private float m_CalmSpeed = 1.5f;

    [SerializeField] private float m_MaxAmplitude = 5.0f;
    [SerializeField] private float m_MaxFrequency = 8.0f;

    private CinemachineVirtualCamera m_VCam;
    private CinemachineBasicMultiChannelPerlin m_Noise;

    public void AddNoiseAmplitude(float amplitude)
    {
        this.SetNoiseAmplitude(m_Noise.m_AmplitudeGain + amplitude);
    }

    public void AddNoiseFrequency(float frequency)
    {
        this.SetNoiseFrequency(m_Noise.m_FrequencyGain + frequency);
    }

    public void SetNoiseAmplitude(float amplitude)
    {
        amplitude = math.abs(amplitude);
        m_Noise.m_AmplitudeGain = math.min(amplitude, this.m_MaxAmplitude);
    }

    public void SetNoiseFrequency(float frequency)
    {
        frequency = math.abs(frequency);
        m_Noise.m_FrequencyGain = math.min(frequency, this.m_MaxFrequency);
    }

    protected override void Awake()
    {
        base.Awake();

        m_VCam = this.GetComponent<CinemachineVirtualCamera>();
        m_Noise = m_VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        m_Noise.m_FrequencyGain = math.lerp(m_Noise.m_FrequencyGain, 0.0f, Time.deltaTime * this.m_CalmSpeed);
        m_Noise.m_AmplitudeGain = math.lerp(m_Noise.m_AmplitudeGain, 0.0f, Time.deltaTime * this.m_CalmSpeed);
    }
}
