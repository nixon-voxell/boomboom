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
        SetSliderVolume(m_BgmSlider, "BgmSource");
        SetSliderVolume(m_SfxSlider, "SfxSource");
    }

    private void SetSliderVolume(Slider slider, string sourceName)
    {
        AudioSource source = FindAnyObjectByType<AudioSource>();

        if (source != null && source.name == sourceName)
        {
            slider.RegisterValueChangedCallback((v) => AudioManager.Instance.ToggleVolume(source, slider.value));
        }
    }

    private void BackMenu()
    {
        UiManagerMono.Instance.SetOnlyVisible(typeof(StartMenuMono));
    }
}
