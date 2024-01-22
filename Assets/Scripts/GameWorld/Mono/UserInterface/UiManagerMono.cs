public class UiManagerMono : SingletonMono<UiManagerMono>
{
    public UiMono[] Uis;

    public void SetOnlyVisible(System.Type type)
    {
        foreach (UiMono ui in this.Uis)
        {
            if (ui.GetType() == type)
            {
                ui.Root.visible = true;
            }
            else
            {
                ui.Root.visible = false;
            }
        }
    }

    public T GetUi<T>() where T : UiMono
    {
        foreach (UiMono ui in this.Uis)
        {
            if (ui.GetType() == typeof(T))
            {
                return ui as T;
            }
        }

        return null;
    }
}
