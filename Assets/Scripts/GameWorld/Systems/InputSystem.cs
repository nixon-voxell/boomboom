using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public partial class InputSystem : SystemBase
{
    private Controls m_Input = null;

    protected override void OnCreate()
    {
        // Initialize Controls
        this.m_Input = new Controls();
        this.m_Input.Enable();

        this.RequireForUpdate<UserInputSingleton>();

        // Add required UserInputSingleton
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        Entity entity = commands.CreateEntity();
        commands.AddComponent<UserInputSingleton>(entity, UserInputSingleton.Default);

        commands.Playback(this.EntityManager);
    }

    protected override void OnUpdate()
    {
        // Update UserInputSingleton
        UserInputSingleton input = SystemAPI.GetSingleton<UserInputSingleton>();

        input.MoveAxis = this.m_Input.Player.Move.ReadValue<Vector2>();
        input.IsMoving = this.m_Input.Player.Move.IsPressed();
        input.Dash = this.m_Input.Player.Dash.WasPressedThisFrame();
        input.Bomb = this.m_Input.Player.Bomb.WasPressedThisFrame();
        input.Pause = this.m_Input.Player.Pause.WasPerformedThisFrame();

        SystemAPI.SetSingleton<UserInputSingleton>(input);
    }

    protected override void OnDestroy()
    {
        this.m_Input.Dispose();
    }
}
