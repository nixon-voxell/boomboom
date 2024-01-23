using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct ManagerSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameManagerSingleton>();
        state.RequireForUpdate<Tag_MascotSingleton>();

        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<GameCurrStateSingleton, GameTargetStateSingleton>()
            .Build()
        );

        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<Tag_MascotSingleton, Child>()
            .Build()
        );

        state.RequireForUpdate<GameStatSingleton>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        ref GameManagerSingleton gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;

        gameManager.EnvironmentWorld.LoadScene(ref state);

        AudioManager.Instance.PlayBgm("MainMenu");
    }

    public void OnStopRunning(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        ref GameCurrStateSingleton currState = ref SystemAPI.GetSingletonRW<GameCurrStateSingleton>().ValueRW;
        GameTargetStateSingleton targetState = SystemAPI.GetSingleton<GameTargetStateSingleton>();

        if (currState.Value == targetState.Value)
        {
            return;
        }

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Game manager
        ref GameManagerSingleton gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;

        // Game stat
        ref GameStatSingleton gameStat = ref SystemAPI.GetSingletonRW<GameStatSingleton>().ValueRW;

        // Mascot
        Entity mascotEntity = SystemAPI.GetSingletonEntity<Tag_MascotSingleton>();
        DynamicBuffer<Child> mascotChildren = SystemAPI.GetBuffer<Child>(mascotEntity);
        ref readonly LocalTransform mascotTransform = ref SystemAPI.GetComponentRO<LocalTransform>(mascotEntity).ValueRO;

        switch (targetState.Value)
        {
            case GameState.Start:
                // Revert to normal time scale
                UnityEngine.Time.timeScale = 1.0f;
                // Enable mascot entity
                this.SetMascotEnable(
                    ref commands,
                    in mascotChildren,
                    mascotEntity,
                    true
                );

                // Reload environment world
                gameManager.GameWorld.UnloadScene(ref state);
                gameManager.EnvironmentWorld.UnloadScene(ref state);
                gameManager.EnvironmentWorld.LoadScene(ref state);

                // Reset target position
                PlayerTargetMono.Instance.TargetPosition = mascotTransform.Position;

                // Disable camera
                VirtualCameraMono.Instance.SetPriority(9);
                // Enable in start menu
                UiManagerMono.Instance.SetOnlyVisible(typeof(StartMenuMono));
                // Reset game stat
                gameStat = GameStatSingleton.Default();
                break;

            case GameState.InGame:
                // Disable mascot entity
                this.SetMascotEnable(
                    ref commands,
                    in mascotChildren,
                    mascotEntity,
                    false
                );

                gameManager = ref SystemAPI.GetSingletonRW<GameManagerSingleton>().ValueRW;
                gameManager.GameWorld.LoadScene(ref state);

                // Make as main camera
                VirtualCameraMono.Instance.SetPriority(11);
                // Enable in game hud
                UiManagerMono.Instance.SetOnlyVisible(typeof(InGameHudMono));
                break;

            case GameState.End:
                // Immediate slowdown of game
                UnityEngine.Time.timeScale = 0.1f;
                // Enable end menu to display game stat
                UiManagerMono.Instance.SetOnlyVisible(typeof(EndMenuMono));
                EndMenuMono endMenu = UiManagerMono.Instance.GetUi<EndMenuMono>();

                int hours, minutes, seconds;
                GameUtil.CalculateTimeFromSeconds((int)gameStat.SurvivalTime, out hours, out minutes, out seconds);

                endMenu.SurvivalTimeLbl.text = $"{hours:00}:{minutes:00}:{seconds:00}";
                endMenu.KillCountLbl.text = gameStat.KillCount.ToString();

                break;
        }

        commands.Playback(state.EntityManager);

        // Update state after completing all state change related tasks.
        currState.Value = targetState.Value;
    }

    private void SetMascotEnable(
        ref EntityCommandBuffer commands,
        in DynamicBuffer<Child> mascotChildren,
        Entity entity,
        bool enable
    )
    {
        foreach (Child child in mascotChildren)
        {
            commands.SetEnabled(child.Value, enable);
        }
    }
}

public partial struct GameTimeUpdate : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameStatSingleton>();
        state.RequireForUpdate<GameCurrStateSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        GameCurrStateSingleton currState = SystemAPI.GetSingleton<GameCurrStateSingleton>();

        if (currState.Value != GameState.InGame)
        {
            return;
        }

        ref GameStatSingleton gameStat = ref SystemAPI.GetSingletonRW<GameStatSingleton>().ValueRW;

        gameStat.SurvivalTime += SystemAPI.Time.DeltaTime;
    }
}

public partial struct PlayerDeathSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tag_PlayerSingleton>();
        state.RequireForUpdate(
            SystemAPI.QueryBuilder()
            .WithAll<GameCurrStateSingleton, GameTargetStateSingleton>()
            .Build()
        );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GameCurrStateSingleton currState = SystemAPI.GetSingleton<GameCurrStateSingleton>();

        // Game already ended, no point in checking
        if (currState.Value == GameState.End)
        {
            return;
        }

        // Player's health
        ref Health health = ref SystemAPI.GetComponentRW<Health>(
            SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>()
        ).ValueRW;

        ref GameTargetStateSingleton targetState = ref SystemAPI.GetSingletonRW<GameTargetStateSingleton>().ValueRW;

        if (health.Value <= 0.0f)
        {
            targetState.Value = GameState.End;
        }
    }
}
