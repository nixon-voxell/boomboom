using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMono : UiMono
{
    private Button m_BackBtn;
    private Slider m_BgmSlider;
    private Slider m_SfxSlider;

    private void Start()
    {
        this.m_BackBtn = this.Root.Query<Button>("back-btn");
        this.m_BgmSlider = this.Root.Query<Slider>("bgm-slider");
        this.m_SfxSlider = this.Root.Query<Slider>("sfx-slider");

        this.m_BackBtn.clicked += this.BackMenu;
    }

    private void Update()
    {
        SetSliderVolume(m_BgmSlider, AudioManager.Instance.BgmSource);
        SetSliderVolume(m_SfxSlider, AudioManager.Instance.SfxSource);
    }

    private void SetSliderVolume(Slider slider, AudioSource source)
    {
        slider.lowValue = 0;
        slider.highValue = 1;

        if (source != null)
        {
            slider.RegisterValueChangedCallback((v) => source.volume = slider.value);
        }
    }

    private void BackMenu()
    {
        UiManagerMono.Instance.SetOnlyVisible(typeof(StartMenuMono));
    }
}
