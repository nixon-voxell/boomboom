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
    }

    protected override void OnStartRunning()
    {
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

        SystemAPI.SetSingleton<UserInputSingleton>(input);
    }
}
