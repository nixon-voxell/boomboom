using UnityEngine.UIElements;

public class InGameHudMono : UiMono
{
    public Label TimeLbl;
    public ProgressBar HealthBar;

    private void Start()
    {
        this.TimeLbl = this.Root.Query<Label>("time-lbl");
        this.HealthBar = this.Root.Query<ProgressBar>("health-bar");

        this.HealthBar.lowValue = 0.0f;
        this.HealthBar.highValue = 100.0f;
    }
}
