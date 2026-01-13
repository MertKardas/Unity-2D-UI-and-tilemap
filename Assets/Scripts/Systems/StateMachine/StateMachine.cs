using System;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine<T> where T : class {
    private State<T> _currentState;
    private Dictionary<Type, State<T>> _states = new Dictionary<Type, State<T>>();

    public State<T> CurrentState => _currentState;

    public void AddState(State<T> state) {
        Type stateType = state.GetType();
        if (!_states.ContainsKey(stateType)) {
            _states.Add(stateType, state);
        }
    }

    public void SetState(State<T> newState) {
        if (newState == null) {
            Debug.LogWarning("Attempting to set null state!");
            return;
        }

        if (!_states.ContainsValue(newState)) {
            Debug.LogWarning($"State {newState.GetType().Name} not added to machine!");
            return;
        }

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    // PROPER EXECUTION ORDER: Input → Transitions → Update
    public void Update() {
        if (_currentState == null) return;

        HandleInput();
        CheckTransitions();
        _currentState.Update();
    }

    public void HandleInput() {
        _currentState?.HandleInput();
    }

    public void CheckTransitions() {
        _currentState?.CheckTransitions();
    }

    public void FixedUpdate() {
        _currentState?.FixedUpdate();
    }

    public TState GetState<TState>() where TState : State<T> {
        if (_states.TryGetValue(typeof(TState), out var state)) {
            return state as TState;
        }
        return null;
    }

    // Save/Load support
    public StateData GetCurrentStateData() {
        if (_currentState == null) return null;

        var data = new StateData {
            stateTypeName = _currentState.GetType().FullName
        };

        // Get state-specific data
        var stateData = _currentState.GetStateData();
        if (stateData != null) {
            data.customData = stateData.customData;
            data.nestedStateData = stateData.nestedStateData;
        }

        return data;
    }

    public void RestoreState(StateData data) {
        if (data == null) return;

        Type stateType = Type.GetType(data.stateTypeName);
        if (stateType == null) {
            Debug.LogError($"State type {data.stateTypeName} not found!");
            return;
        }

        var getStateMethod = typeof(StateMachine<T>)
            .GetMethod(nameof(GetState))
            .MakeGenericMethod(stateType);

        var state = getStateMethod.Invoke(this, null) as State<T>;

        if (state != null) {
            SetState(state);
            state.LoadStateData(data);
        }
    }

    public string GetCurrentStatePath() {
        if (_currentState == null) return "None";

        string path = _currentState.StateName;
        var data = _currentState.GetStateData();

        if (data?.nestedStateData != null) {
            string nestedType = data.nestedStateData.stateTypeName;
            if (!string.IsNullOrEmpty(nestedType)) {
                string nestedName = nestedType.Split('.')[^1];
                path += $" → {nestedName}";
            }
        }

        return path;
    }
}
[Serializable]
public class StateData {
    public string stateTypeName;
    public Dictionary<string, object> customData;
    public StateData nestedStateData;

    public StateData() {
        customData = new Dictionary<string, object>();
    }
}