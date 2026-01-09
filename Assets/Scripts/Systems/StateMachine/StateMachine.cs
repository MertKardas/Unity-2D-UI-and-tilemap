using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class State<T> where T: class
{
    protected T _controller { get; }
    protected StateMachine<T> _machine { get; }
    public State(T controller, StateMachine<T> machine) {
        _controller = controller;
        _machine = machine;
    }

    public virtual void Enter() {
        
    }
    public virtual void Exit() {}
    public virtual void Update() {
        InputHandle();
        CheckTransitions(); 
    }
    public virtual void FixedUpdate() { }
    public virtual void InputHandle() { }
    public virtual void CheckTransitions() { }  
}

public class StateMachine<T>where T : class
{
    private State<T> _currentState;
    private Dictionary<Type, State<T>> _states = new Dictionary<Type, State<T>>(4);
    //Debug in inspector
    public string CurrentStateName => _currentState?.GetType().Name;

    public void SetState(State<T> newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update()
    {
        _currentState?.Update();
    }
    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }
    public void AddState(State<T> state) {
        if (!_states.ContainsKey(state.GetType())) {
            _states.Add(state.GetType(), state);
        }
    }
    public State<T> GetState<U>() where U : State<T> {
        if (_states.TryGetValue(typeof(U), out var state)) {
            return state;
        }
        return null;
    }
}
