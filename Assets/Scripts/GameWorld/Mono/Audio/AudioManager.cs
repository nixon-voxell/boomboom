using UnityEngine;
using System;

/* if want to call any music here in other script, 
 * call those functions at region Public Call Functions
 * 
 * example: AudioManager.Instance.PlaySfx("Place Bomb");
 * example: AudioManager.Instance.BgmSource.Stop();
*/
public class AudioManager : SingletonMono<AudioManager>
{
    public SoundElement[] BgmSoundList, SfxSoundList;
    public AudioSource BgmSource, SfxSource;

    #region Public Call Functions
    public void PlayBgm(string musicName)
    {
        SoundElement ele = GetMusic(BgmSoundList, musicName);
        PlayBgm(ele, BgmSource);
    }

    public void PlaySfx(string musicName)
    {
        SoundElement ele = GetMusic(SfxSoundList, musicName);
        PlaySfx(ele, SfxSource);
    }

    #region Only UI
    public void ToggleMute(AudioSource source)  //AudioManager.Instance.BgmSource
    {
        source.mute = !source.mute;
    }

    /* in ui script, create a void function to manage, like example below:
    * 
    * public void ToggleMusic()
    * {
    *      AudioManager.Instance.ToggleMute(AudioManager.Instance.BgmSource)
    *  }
    *  
    *  why create void function? it is for later click button event use
    */

    public void ToggleVolume(AudioSource source, float newVolume)
    {
        source.volume = newVolume;
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
