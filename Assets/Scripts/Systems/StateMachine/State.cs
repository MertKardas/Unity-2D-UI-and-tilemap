using System;
using UnityEngine;

[Serializable]
public abstract class State<T> where T : class {
    protected T _controller { get; }
    protected StateMachine<T> _machine { get; }

    public string StateName { get; protected set; }

    public State(T controller, StateMachine<T> machine) {
        _controller = controller;
        _machine = machine;
        StateName = GetType().Name;
    }

    // Lifecycle
    public virtual void Enter() { }
    public virtual void Exit() { }

    // CORRECT EXECUTION ORDER
    public virtual void HandleInput() { }
    public virtual void CheckTransitions() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

 
    public virtual StateData GetStateData() { return null; }
    public virtual void LoadStateData(StateData data) { }
}