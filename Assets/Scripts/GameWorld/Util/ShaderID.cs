using UnityEngine;

public static class ShaderID
{
    public static readonly int _EyeColor = Shader.PropertyToID("_EyeColor");

    public static readonly int _Eye0Center = Shader.PropertyToID("_Eye0Center");
    public static readonly int _Eye0Size = Shader.PropertyToID("_Eye0Size");

    public static readonly int _Eye1Center = Shader.PropertyToID("_Eye1Center");
    public static readonly int _Eye1Size = Shader.PropertyToID("_Eye1Size");

    public static readonly int _Position = Shader.PropertyToID("_Position");
}
