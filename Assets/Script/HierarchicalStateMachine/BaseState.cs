using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class BaseState<EState> where EState : Enum
{

    public BaseState(EState key, StateManager<EState> context, int level)
    {
        StateKey = key;
        stateManager = context;
        this.level = level;
    }

    public EState StateKey { get; private set; }
    public int level { get; private set; }
    public bool hasFather => CurrentSuperState != null;
    public bool hasChild => CurrentSubState != null;

    [HideInInspector] public StateManager<EState> stateManager;
    [HideInInspector] public BaseState<EState> CurrentSuperState { get; private set; }
    [HideInInspector] public BaseState<EState> CurrentSubState { get; private set; }

    public void TransitionToState(EState newStateKey)
    {
        BaseState<EState> newState = stateManager.States[newStateKey];
        if (newState.level == level)
        {
            if (hasFather)
            {
                if (!newState.hasFather || (newState.hasFather && newState.CurrentSuperState == CurrentSuperState))
                {
                    newState.SetSuperState(CurrentSuperState.StateKey);
                }
                else if (newState.hasFather && newState.CurrentSuperState != CurrentSuperState)
                {
                    ExitAllSuperStates();
                    newState.EnterAllSuperStates();
                }
            }
            ExitState();
            newState.EnterState();

            if (hasChild)
            {
                if (!newState.hasChild || (newState.hasChild && newState.CurrentSubState == CurrentSubState))
                {
                    newState.SetSubState(CurrentSubState.StateKey);
                }
                else if (newState.hasChild && newState.CurrentSubState != CurrentSubState)
                {
                    ExitAllSubStates();
                    newState.EnterAllSubStates();
                }
            }

            stateManager.CurrentState = newState;

        }
        else
        {
            Debug.LogWarning($"Hierarchical State Machine: Transition from {this.StateKey} state to {newStateKey} state, wrong level");
        }
    }
    public void SetSuperState(EState newStateKey)
    {
        BaseState<EState> newState = stateManager.States[newStateKey];
        if (newState.level - level == -1)
        {
            CurrentSuperState = newState;
            if (CurrentSuperState.CurrentSubState != this)
                CurrentSuperState.SetSubState(StateKey);
        }
        else
        {
            Debug.LogWarning($"Hierarchical State Machine: Set super state of {this.StateKey} state, wrong level");
        }
    }
    public void SetSubState(EState newStateKey)
    {
        BaseState<EState> newState = stateManager.States[newStateKey];
        if (newState.level - level == 1)
        {
            CurrentSubState = newState;
            if (CurrentSubState.CurrentSuperState != this)
                CurrentSubState.SetSuperState(StateKey);
        }
        else
        {
            Debug.LogWarning($"Hierarchical State Machine: Set sub state of {this.StateKey} state, wrong level");
        }
    }


    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual void LateUpdateState() { }
    public virtual void OnAnimationIK(int index) { }
    public virtual void CheckTransition() { }
    public virtual void InitializeSubState() { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
    public virtual void OnDrawGizmos() { }

    public void UpdateStates(BaseState<EState> previousState = null)
    {
        UpdateState();
        if (CurrentSuperState != null && CurrentSuperState != previousState)
            CurrentSuperState.UpdateStates(this);
        if (CurrentSubState != null && CurrentSubState != previousState)
            CurrentSubState.UpdateStates(this);
    }
    public void EnterStates(BaseState<EState> previousState = null)
    {
        EnterState();
        if (CurrentSuperState != null && CurrentSuperState != previousState)
            CurrentSuperState.EnterStates(this);
        if (CurrentSubState != null && CurrentSubState != previousState)
            CurrentSubState.EnterStates(this);
    }
    public void EnterAllSuperStates()
    {
        if (CurrentSuperState != null)
        {
            CurrentSuperState.EnterAllSuperStates();
            CurrentSuperState.EnterState();
        }
    }
    public void EnterAllSubStates()
    {
        if (CurrentSubState != null)
        {
            CurrentSubState.EnterState();
            CurrentSubState.EnterAllSubStates();
        }
    }
    public void ExitAllSuperStates()
    {
        if (CurrentSuperState != null)
        {
            CurrentSuperState.ExitAllSuperStates();
            CurrentSuperState.ExitState();
        }
    }
    public void ExitAllSubStates()
    {
        if (CurrentSubState != null)
        {
            CurrentSubState.ExitState();
            CurrentSubState.ExitAllSubStates();
        }
    }
    public void FixedUpdateStates(BaseState<EState> previousState = null)
    {
        FixedUpdateState();
        if (CurrentSuperState != null && CurrentSuperState != previousState)
            CurrentSuperState.FixedUpdateStates(this);
        if (CurrentSubState != null && CurrentSubState != previousState)
            CurrentSubState.FixedUpdateStates(this);
    }

    //For debug
    public string GetAllCurrentStatesToString(BaseState<EState> previousState = null)
    {
        string output = "";
        if (CurrentSuperState != null && CurrentSuperState != previousState)
            output += CurrentSuperState.GetAllCurrentStatesToString(this) + " ";

        output += StateKey.ToString() + " ";

        if (CurrentSubState != null && CurrentSubState != previousState)
            output += CurrentSubState.GetAllCurrentStatesToString(this) + " ";

        return output;

    }

}