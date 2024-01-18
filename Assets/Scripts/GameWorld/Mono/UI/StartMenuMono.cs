using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuMono : UISingletonMono<StartMenuMono>
{
    private Button m_StartBtn;
    private Button m_SettingsBtn;
    private Button m_QuitBtn;

    private void Start()
    {
        this.m_StartBtn = this.m_Root.Query<Button>("start-btn");
        this.m_SettingsBtn = this.m_Root.Query<Button>("settings-btn");
        this.m_QuitBtn = this.m_Root.Query<Button>("quit-btn");

        this.m_StartBtn.clicked += this.StartGame;
        this.m_QuitBtn.clicked += this.QuitGame;
    }

    private void StartGame()
    {
        GameStateSpawnerMono.Instance.SetTargetState(GameState.InGame);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
