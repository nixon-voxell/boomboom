using System;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Mathematics;

/* if want to call any music here in other script, 
 * call those functions at region Public Call Functions
 * 
 * example: AudioManager.Instance.PlaySfx("Place Bomb");
 * example: AudioManager.Instance.BgmSource.Stop();
*/
public class AudioManager : SingletonMono<AudioManager>
{
    public const string BgmCutoffFreq = "BgmCutoffFreq";

    [Header("Audio Mixer")]
    public AudioMixer Mixer;
    [SerializeField] private float m_BgmCutoffFreqIncrement = 5.0f;
    [SerializeField] private float m_BgmCutoffFreqDecrement = 5.0f;
    [SerializeField] private float m_BgmMuffledFreq = 200.0f;
    [SerializeField] private float m_BgmNormalFreq = 22000.0f;

    private bool m_BgmMuffledState;
    private float m_BgmCutoffFreqCurr;

    [Header("Sound")]
    [SerializeField] private SoundElement[] m_BgmSoundList, m_SfxSoundList;
    public AudioSource BgmSource, SfxSource;

    private void Start()
    {
        this.m_BgmMuffledState = true;
        this.m_BgmCutoffFreqCurr = this.m_BgmMuffledFreq;
    }

    public void Update()
    {
        if (this.m_BgmMuffledState)
        {
            this.m_BgmCutoffFreqCurr = math.max(
                this.m_BgmCutoffFreqCurr - this.m_BgmCutoffFreqDecrement,
                this.m_BgmMuffledFreq
            );
        }
        else
        {
            this.m_BgmCutoffFreqCurr = math.min(
                this.m_BgmCutoffFreqCurr + this.m_BgmCutoffFreqIncrement,
                this.m_BgmNormalFreq
            );
        }
        this.Mixer.SetFloat(BgmCutoffFreq, this.m_BgmCutoffFreqCurr);
    }

    public void SetBgmMuffled(bool muffled)
    {
        this.m_BgmMuffledState = muffled;
    }

    #region Public Call Functions
    public void PlayBgm(string musicName)
    {
        SoundElement ele = GetMusic(m_BgmSoundList, musicName);
        PlayBgm(ele, BgmSource);
    }

    public void PlaySfx(string musicName)
    {
        SoundElement ele = GetMusic(m_SfxSoundList, musicName);
        PlaySfx(ele, SfxSource);
    }

    #region Only UI
    public void ToggleMute(AudioSource source)  //AudioManager.Instance.BgmSource
    {
        source.mute = !source.mute;
    }

    #endregion
    #endregion

    #region Private Quick Functions
    private SoundElement GetMusic(SoundElement[] list, string musicName)
    {
        SoundElement ele = Array.Find(list, x => x.SoundName == musicName);

        return ele;
    }

    private void PlayBgm(SoundElement ele, AudioSource source)
    {
        if (!ele.Equals(default(SoundElement)))
        {
            source.clip = ele.Clip;
            source.Play();
        }
        else { return; }
    }

    private void PlaySfx(SoundElement ele, AudioSource source)
    {
        if (!ele.Equals(default(SoundElement)))
        {
            source.clip = ele.Clip;
            source.PlayOneShot(ele.Clip);
        }
        else { return; }
    }
    #endregion
}
