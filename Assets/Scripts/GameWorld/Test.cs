using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int InSeconds;

    public int Hours;
    public int Minutes;
    public int Seconds;

    private void OnValidate()
    {
        GameUtil.CalculateTimeFromSeconds(
            in this.InSeconds,
            out this.Hours,
            out this.Minutes,
            out this.Seconds
        );
    }
}
