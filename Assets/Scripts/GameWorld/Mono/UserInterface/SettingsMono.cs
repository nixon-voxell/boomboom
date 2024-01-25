using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMono : UiMono
{
    public Button MainMenuBtn => this.m_MainMenuBtn;

    private Button m_BackBtn;
    private Button m_MainMenuBtn;
    private Slider m_BgmSlider;
    private Slider m_SfxSlider;

    private void Start()
    {
        this.m_BackBtn = this.Root.Query<Button>("back-btn");
        this.m_MainMenuBtn = this.Root.Query<Button>("exit-to-menu-btn");
        this.m_BgmSlider = this.Root.Query<Slider>("bgm-slider");
        this.m_SfxSlider = this.Root.Query<Slider>("sfx-slider");

        this.m_MainMenuBtn.SetEnabled(false);

        this.m_BackBtn.clicked += () =>
        {
            this.Root.visible = false;
            // Reset time scale
            Time.timeScale = 1.0f;
        };

        this.m_MainMenuBtn.clicked += () =>
        {
            GameStateSpawnerMono.Instance.SetTargetState(GameState.Start);
            this.Root.visible = false;
            // Reset time scale
            Time.timeScale = 1.0f;
        };

        this.m_BgmSlider.lowValue = 0.0f;
        this.m_BgmSlider.highValue = 1.0f;
        this.m_SfxSlider.lowValue = 0.0f;
        this.m_SfxSlider.highValue = 1.0f;

        SetSliderVolume(m_BgmSlider, AudioManager.Instance.BgmSource);
        SetSliderVolume(m_SfxSlider, AudioManager.Instance.SfxSource);
    }

    private void SetSliderVolume(Slider slider, AudioSource source)
    {
        if (source != null)
        {
            slider.RegisterValueChangedCallback((v) => source.volume = slider.value);
        }
    }
}
