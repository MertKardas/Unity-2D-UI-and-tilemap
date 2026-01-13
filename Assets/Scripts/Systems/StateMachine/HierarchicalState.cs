using System;

[Serializable]
public abstract class HierarchicalState<T> : State<T> where T : class {
    protected StateMachine<T> _subStateMachine;

    public HierarchicalState(T controller, StateMachine<T> machine)
        : base(controller, machine) {
        _subStateMachine = new StateMachine<T>();
    }

    public void AddSubState(State<T> state) {
        _subStateMachine.AddState(state);
    }

    protected void InitializeSubStates<TState>() where TState : State<T> {
        var state = _subStateMachine.GetState<TState>();
        if (state != null) {
            _subStateMachine.SetState(state);
        }
    }

    // Pass execution to sub-states
    public override void HandleInput() {
        _subStateMachine.HandleInput();
    }

    public override void CheckTransitions() {
        _subStateMachine.CheckTransitions();
    }

    public override void Update() {
        _subStateMachine.Update();
    }

    public override void FixedUpdate() {
        _subStateMachine.FixedUpdate();
    }

    // Nested save/load
    public override StateData GetStateData() {
        var data = base.GetStateData() ?? new StateData();
        data.nestedStateData = _subStateMachine.GetCurrentStateData();
        return data;
    }

    public override void LoadStateData(StateData data) {
        if (data?.nestedStateData != null) {
            _subStateMachine.RestoreState(data.nestedStateData);
        }
    }
}