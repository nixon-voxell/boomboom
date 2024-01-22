using UnityEngine.UIElements;
using Unity.Entities;

public class EndMenuMono : UiMono
{
    public Label SurvivalTimeLbl;
    public Label KillCountLbl;

    private Button m_ExitBtn;

    private void Start()
    {
        this.SurvivalTimeLbl = this.Root.Query<Label>("survival-time-lbl");
        this.KillCountLbl = this.Root.Query<Label>("kill-count-lbl");

        this.m_ExitBtn = this.Root.Query<Button>("exit-btn");

        this.m_ExitBtn.clicked += () =>
        {
            GameStateSpawnerMono.Instance.SetTargetState(GameState.Start);
        };
    }
}
