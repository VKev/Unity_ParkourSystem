using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    public Dictionary<EState, BaseState<EState>> States { get; protected set; } = new Dictionary<EState, BaseState<EState>>();

    public BaseState<EState> CurrentState;

}