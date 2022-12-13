using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager current;
    public event Action onGateColliderEnter;
    public event Action<int> onGateColorChange;

    private void Awake()
    {
        current = this;
    }

    public void GateColliderEnter()
    {
        if(onGateColliderEnter != null)
        {
            onGateColliderEnter();
        }
    }
    public void GateColorChange(int input)
    {
        if (onGateColorChange != null)
        {
            onGateColorChange(input);
        }
    }
}
