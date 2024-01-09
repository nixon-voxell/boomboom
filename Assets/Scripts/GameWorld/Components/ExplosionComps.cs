using Unity.Entities;

public struct ExplosionPoolSingleton : IComponentData { }

public struct Explode : IComponentData, IEnableableComponent
{
    public Entity LandmineEntity;
}

public struct ExplosionForce : IComponentData
{
    public float Value;
}

public struct ExplosionRadius : IComponentData
{
    public float Value;
}

public struct ExplosionTimer : IComponentData
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

public readonly partial struct ExplosionAspect : IAspect
{
    public readonly RefRW<ExplosionForce> Force;
    public readonly RefRW<ExplosionRadius> Radius;
    public readonly RefRW<Damage> Damage;

    public ref float ForceRW => ref this.Force.ValueRW.Value;
    public ref float RadiusRW => ref this.Radius.ValueRW.Value;
    public ref float DamageRW => ref this.Damage.ValueRW.Value;
}

public struct LandminePoolSingleton : IComponentData
{
    public Entity Prefab;
    public int PoolCount;
}
