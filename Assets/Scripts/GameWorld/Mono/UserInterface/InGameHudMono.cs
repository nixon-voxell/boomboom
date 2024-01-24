using UnityEngine.UIElements;

public class InGameHudMono : UiMono
{
    public Label TimeLbl;
    public Label KillCountLbl;
    public Label BombCountLbl;
    public Label DashCountLbl;
    public ProgressBar HealthBar;

    private void Start()
    {
        this.TimeLbl = this.Root.Query<Label>("time-lbl");
        this.KillCountLbl = this.Root.Query<Label>("kill-count-lbl");
        this.BombCountLbl = this.Root.Query<Label>("bomb-count-lbl");
        this.DashCountLbl = this.Root.Query<Label>("dash-count-lbl");
        this.HealthBar = this.Root.Query<ProgressBar>("health-bar");

        this.HealthBar.lowValue = 0.0f;
        this.HealthBar.highValue = 100.0f;
    }
}
