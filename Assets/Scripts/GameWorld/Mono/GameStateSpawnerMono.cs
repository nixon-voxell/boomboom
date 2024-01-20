using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class GameStateSpawnerMono : SingletonMono<GameStateSpawnerMono>
{
    [SerializeField] private GameState m_DefaultState = GameState.Start;

    public Entity StateEntity;
    private EntityManager m_EntityManager;

    protected override void Awake()
    {
        base.Awake();
        this.m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        this.StateEntity = this.m_EntityManager.CreateEntity(typeof(GameCurrStateSingleton), typeof(GameTargetStateSingleton));

        this.m_EntityManager.SetComponentData<GameCurrStateSingleton>(
            this.StateEntity,
            new GameCurrStateSingleton
            {
                Value = this.m_DefaultState,
            }
        );

        this.m_EntityManager.SetComponentData<GameTargetStateSingleton>(
            this.StateEntity,
            new GameTargetStateSingleton
            {
                Value = this.m_DefaultState,
            }
        );
    }

    public void SetTargetState(GameState state)
    {
        this.m_EntityManager.SetComponentData<GameTargetStateSingleton>(
            this.StateEntity,
            new GameTargetStateSingleton
            {
                Value = state
            }
        );
    }
}
