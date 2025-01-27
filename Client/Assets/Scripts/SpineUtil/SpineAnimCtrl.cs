using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;



// This is an example of an animation handle. This is implemented with strings as state names.
// Strings can serve as the identifier when Mecanim is used as the state machine and state source.
// If you don't use Mecanim, using custom ScriptableObjects may be a more efficient way to store information about the state and its connection with specific Spine animations.

// This animation handle implementation also comes with a dummy implementation of transition-handling.
public class SpineAnimCtrl : MonoBehaviour
{

    public enum SpineAnimState
    {
        None=0,
        Idle,
        Run,
        PickIdle,
        PickRun,
        build_work,
        build_fix,
        build_broken,
        build_idle,
        cashwork,
       // sleep,
        sleeping,
        wakeup,
    }

    private static List<ActionData> stateNameArr = new List<ActionData> {
        new ActionData("idle",true),
        new ActionData("idle",true),
        new ActionData("run",true),
        new ActionData("pickidle",true),
        new ActionData("pickrun",true),
        new ActionData("buildwork",true),
        new ActionData("buildfix",true),
        new ActionData("buildbroken",true),
        new ActionData("buildidle",true),
        new ActionData("cashwork",true),
        new ActionData("sleep",true),
        new ActionData("wakeup",false),
    };
    public SkeletonAnimation skeletonAnimation;
    public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
    public List<AnimationTransition> transitions = new List<AnimationTransition>(); // Alternately, an AnimationPair-Animation Dictionary (commented out) can be used for more efficient lookups.
    SpineAnimState previousState, currentState;
    [System.Serializable]
    public class StateNameToAnimationReference
    {
        public string stateName;
        public AnimationReferenceAsset animation;
    }

    public class ActionData
    {
        public string actionName;
        public bool IsLoop;
        public ActionData(string vt, bool loop)
        {
            actionName = vt;
            IsLoop = loop;
        }
    }

    [System.Serializable]
    public class AnimationTransition
    {
        public AnimationReferenceAsset from;
        public AnimationReferenceAsset to;
        public AnimationReferenceAsset transition;
    }

    //readonly Dictionary<Spine.AnimationStateData.AnimationPair, Spine.Animation> transitionDictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>(Spine.AnimationStateData.AnimationPairComparer.Instance);

    public Spine.Animation TargetAnimation { get; private set; }

    private string Idleanimstr;
    void Awake()
    {
        // Initialize AnimationReferenceAssets
        foreach (StateNameToAnimationReference entry in statesAndAnimations)
        {
            entry.animation.Initialize();
        }
        foreach (AnimationTransition entry in transitions)
        {
            entry.from.Initialize();
            entry.to.Initialize();
            entry.transition.Initialize();
        }
        Idleanimstr = stateNameArr[(int)SpineAnimState.Idle].actionName;

    }

    bool stateChanged;
    void Update()
    {
        stateChanged = previousState != currentState;
        previousState = currentState;
        if (stateChanged)
            HandleStateChanged();

    }

    public void SetAnimState(SpineAnimState vt)
    {
        currentState = vt;
    }
   
    public SpineAnimState GetAnimState()
    {
        return currentState;
    }

    void HandleStateChanged()
    {
        if (stateNameArr[(int)currentState].IsLoop)
        {
            PlayAnimationForState(stateNameArr[(int)currentState].actionName, 0);
        }
        else
        {
            PlayOneShotQueue(GetAnimationForState(stateNameArr[(int)currentState].actionName), GetAnimationForState(Idleanimstr));
        }
    }

    public void SetFlip(float horizontal)
    {
        if (horizontal != 0)
        {
            skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? -1f : 1f;
        }
    }

    /// <summary>Plays an animation based on the state name.</summary>
    private void PlayAnimationForState(string stateShortName, int layerIndex)
    {
        PlayAnimationForState(StringToHash(stateShortName), layerIndex);
    }

    /// <summary>Plays an animation based on the hash of the state name.</summary>
    private void PlayAnimationForState(int shortNameHash, int layerIndex)
    {
        Spine.Animation foundAnimation = GetAnimationForState(shortNameHash);
        if (foundAnimation == null)
            return;

        PlayNewAnimation(foundAnimation, layerIndex);
    }

    /// <summary>Gets a Spine Animation based on the state name.</summary>
    private Spine.Animation GetAnimationForState(string stateShortName)
    {
        return GetAnimationForState(StringToHash(stateShortName));
    }

    /// <summary>Gets a Spine Animation based on the hash of the state name.</summary>
    private Spine.Animation GetAnimationForState(int shortNameHash)
    {
        StateNameToAnimationReference foundState = statesAndAnimations.Find(entry => StringToHash(entry.stateName) == shortNameHash);
        return (foundState == null) ? null : foundState.animation;
    }

    /// <summary>Play an animation. If a transition animation is defined, the transition is played before the target animation being passed.</summary>
    private void PlayNewAnimation(Spine.Animation target, int layerIndex)
    {
        Spine.Animation transition = null;
        Spine.Animation current = null;

        current = GetCurrentAnimation(layerIndex);
        if (current != null)
            transition = TryGetTransition(current, target);

        if (transition != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false);
            skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, true, 0f);
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true);
        }

        this.TargetAnimation = target;
    }

    public void PlayOneShotQueue(Spine.Animation oneShot, Spine.Animation idleShot)
    {
        Spine.AnimationState state = skeletonAnimation.AnimationState;
        Spine.Animation current = GetCurrentAnimation(0);
        Spine.Animation transition = TryGetTransition(current, oneShot);
        if (transition != null)
        {
            state.AddAnimation(0, transition, false, 0f);
            state.AddAnimation(0, oneShot, false, 0f);
            state.AddAnimation(0, idleShot, true, 0f);
        }
        else
        {
            state.SetAnimation(0, oneShot, false);
            state.AddAnimation(0, idleShot, true, 0f);
        }
    }

    /// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
    //public void PlayOneShot(Spine.Animation oneShot, int layerIndex)
    //{
    //    Spine.AnimationState state = skeletonAnimation.AnimationState;
    //    state.SetAnimation(0, oneShot, false);

    //    Spine.Animation transition = TryGetTransition(oneShot, TargetAnimation);
    //    if (transition != null)
    //        state.AddAnimation(0, transition, false, 0f);

    //    state.AddAnimation(0, this.TargetAnimation, true, 0f);
    //}

    Spine.Animation TryGetTransition(Spine.Animation from, Spine.Animation to)
    {
        foreach (AnimationTransition transition in transitions)
        {
            if (transition.from.Animation == from && transition.to.Animation == to)
            {
                return transition.transition.Animation;
            }
        }
        return null;

        //Spine.Animation foundTransition = null;
        //transitionDictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out foundTransition);
        //return foundTransition;
    }

    Spine.Animation GetCurrentAnimation(int layerIndex)
    {
        Spine.TrackEntry currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
        return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
    }

    int StringToHash(string s)
    {
        return Animator.StringToHash(s);
    }
}
