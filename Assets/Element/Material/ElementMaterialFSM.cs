using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

//fsm
#region
public class ElementMaterialFSM : MonoBehaviour
{
    private List<ElementMaterialState> states;
    // The only way one can change the state of the FSM is by performing a transition  
    // Don't change the CurrentState directly  
    private ElementMaterialState currentState;

    public ElementMaterialState CurrentState { get { return currentState; } }
    public string currentStateName { get { return currentState.Name; } }
    public ElementMaterialState defaultState { set { defaultState = value; } get { return defaultState; } }

    public ElementMaterialState findState(string statename)
    {
        foreach(ElementMaterialState s in states)
        {
            if (s.Name == statename)
                return s;
        }
        return null;
    }

    public void resetToDefaultState()
    {
        currentState = states[0];
    }

    public ElementMaterialFSM()
    {
        states = new List<ElementMaterialState>();
    }

    /// <summary>  
    /// This method places new states inside the FSM,  
    /// or prints an ERROR message if the state was already inside the List.  
    /// First state added is also the initial state.  
    /// </summary>  
    public void AddState(ElementMaterialState s)
    {
        // Check for Null reference before deleting  
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        // First State inserted is also the Initial state,  
        //   the state the machine is in when the simulation begins  
        if (states.Count == 0)
        {
            states.Add(s);
            currentState = s;
            return;
        }

        // Add the state to the List if it's not inside it  
        foreach (ElementMaterialState state in states)
        {
            if (state.Name == s.Name)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.Name +
                               " because state has already been added");
                return;
            }
        }
        states.Add(s);
    }

    /// <summary>  
    /// This method delete a state from the FSM List if it exists,   
    ///   or prints an ERROR message if the state was not on the List.  
    /// </summary>  
    public void DeleteState(string s)
    {
        // Search the List and delete the state if it's inside it  
        foreach (ElementMaterialState state in states)
        {
            if (state.Name == s)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + s +
                       ". It was not on the list of states");
    }

    /// <summary>  
    /// This method tries to change the state the FSM is in based on  
    /// the current state and the transition passed. If current state  
    ///  doesn't have a target state for the transition passed,   
    /// an ERROR message is printed.  
    /// </summary>  
    public void PerformTransition(StateTransition trans)
    {
        // Check for NullTransition before changing the current state  
        if (trans == null)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }

        ElementMaterialState target = findState(trans.targetState);
        if (target == null)
        {
            Debug.LogError("FSM ERROR: Target State is not existing");
            return;
        }

        // Update the currentStateID and currentState     

        foreach (ElementMaterialState state in states)
        {
            if (state.Name == currentStateName)
            {
                // Do the post processing of the state before setting the new one  
                currentState.DoBeforeLeaving();

                currentState = target;

                // Reset the state to its desired condition before it can reason or act  
                currentState.DoBeforeEntering();
                break;
            }
        }

    } // PerformTransition()  
}
#endregion

//transtion for states
#region
public class StateTransition
{
    public string name;
    public string currentState;//当前态
    public string targetState;//目标态

    public StateTransition(string cur,string tar)
    {
        this.currentState = cur;
        this.targetState = tar;
        this.name = cur + "To" + tar;
    }
}
#endregion

//states
#region
public abstract class ElementMaterialState
{
    protected List<StateTransition> set = new List<StateTransition>();
    protected string stateName;
    public string Name { get { return stateName; } }
    public ElementMaterialFSM fsm;

    public bool ExistTransition(string cur, string tar)
    {

        foreach(StateTransition trans in set)
        {
            if (trans.currentState == cur && trans.targetState == tar)
                return true;
        }
        return false;
    }

    public StateTransition findTransition(string target)
    {
        foreach (StateTransition trans in set)
        {
            if (trans.targetState == target)
                return trans;
        }
        return null;
    }

    public void AddTransition(StateTransition trans)
    {

        // Since this is a Deterministic FSM,  
        //   check if the current transition was already inside the map  
        if (ExistTransition(trans.currentState,trans.targetState))
        {
            Debug.LogError("FSMState ERROR: State " + trans.currentState + " already has transition " + trans.targetState +
                           "Impossible to assign to another state");
            return;
        }

        set.Add(trans);
    }

    /// <summary>  
    /// This method deletes a pair transition-state from this state's map.  
    /// If the transition was not inside the state's map, an ERROR message is printed.  
    /// </summary>  
    public void DeleteTransition(StateTransition trans)
    {
        // Check if the pair is inside the map before deleting  
        if (ExistTransition(trans.currentState, trans.targetState))
        {
            set.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.name+
                       " was not on the state's transition list");
    }

    /// <summary>  
    /// This method is used to set up the State condition before entering it.  
    /// It is called automatically by the FSMSystem class before assigning it  
    /// to the current state.  
    /// </summary>  
    public virtual void DoBeforeEntering() { }

    /// <summary>  
    /// This method is used to make anything necessary, as reseting variables  
    /// before the FSMSystem changes to another one. It is called automatically  
    /// by the FSMSystem before changing to a new state.  
    /// </summary>  
    public virtual void DoBeforeLeaving() { }

    /// <summary>  
    /// This method decides if the state should transition to another on its list  
    /// NPC is a reference to the object that is controlled by this class  
    /// </summary>  
    public abstract void Reason();

    /// <summary>  
    /// This method controls the behavior of the NPC in the game World.  
    /// Every action, movement or communication the NPC does should be placed here  
    /// NPC is a reference to the object that is controlled by this class  
    /// </summary>  
    public abstract void Act();
}
#endregion

