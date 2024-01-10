using Unity.Entities;

public struct Timer : IComponentData
{
    public float TotalTime;
    public float ElapsedTime;

    public void Set(float totalTime)
    {
        this.TotalTime = totalTime;
        this.ElapsedTime = 0.0f;
    }

    public bool Update(float deltaTime)
    {
        if (this.ElapsedTime > this.TotalTime)
        {
            return true;
        }

        this.ElapsedTime += deltaTime;
        return false;
    }
}
