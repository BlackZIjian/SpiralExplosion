using UnityEngine;
using System.Collections;

public enum Transition
{
    NullTransition = 0, // Use this transition to represent a non-existing transition in your system  
    CharacterIdleToWalk,
    CharacterWalkToIdle,
    CharacterLand,
    CharacterJump,
    CharacterFall,
    MeleeKnightSeeSomeOne,
    ToCharacterIdle,
    ToNextAttack
}

public enum StateID
{
    NullStateID = 0, // Use this ID to represent a non-existing State in your system  
    CharacterIdle,
    CharacterWalk,
    CharacterJump,
    CharacterFall,
    MeleeKnightIdle,
    MeleeKnightSeeSomeOne,
    MeleeKnightState,
    CharacterAttack1,
    CharacterAttack2,
    CharacterAttack3
}
