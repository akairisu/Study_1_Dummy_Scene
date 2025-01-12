using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class MorphState
{
    public string stateName;
    public GameObject targetObject;
    public Material targetMaterial;
}

public class MorphTransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private MorphState defaultState;
    [SerializeField] private List<MorphState> morphStates = new List<MorphState>();

    [Header("References")]
    [SerializeField] private Morph morphScript;
    [SerializeField] private Material materialToTransition;

    private MorphState currentState;
    private bool isTransitioning = false;
    private static readonly string TRANSITION_PROPERTY = "_TransitionProgress";

    private void Start()
    {
        currentState = defaultState;
        ResetToDefault(true); // Immediate reset at start
    }

    public void TransitionToState(string stateName)
    {
        MorphState targetState = morphStates.Find(s => s.stateName == stateName);
        if (targetState != null && !isTransitioning)
        {
            StartCoroutine(PerformTransition(targetState));
        }
    }

    public void TransitionToDefault()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PerformTransition(defaultState));
        }
    }

    public void ResetToDefault(bool immediate = false)
    {
        StopAllCoroutines();
        if (immediate)
        {
            // Immediate reset
            materialToTransition.SetFloat(TRANSITION_PROPERTY, 0f);
            morphScript.StartMorphToOld();
            currentState = defaultState;
        }
        else
        {
            TransitionToDefault();
        }
    }

    private IEnumerator PerformTransition(MorphState targetState)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        float startValue = materialToTransition.GetFloat(TRANSITION_PROPERTY);
        float targetValue = (targetState == defaultState) ? 0f : 1f;

        // Start morph
        if (targetState == defaultState)
        {
            morphScript.StartMorphToOld();
        }
        else
        {
            morphScript.StartMorphToNew();
        }

        // Transition material
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;
            float currentValue = Mathf.Lerp(startValue, targetValue, progress);
            materialToTransition.SetFloat(TRANSITION_PROPERTY, currentValue);
            yield return null;
        }

        // Ensure final value is set
        materialToTransition.SetFloat(TRANSITION_PROPERTY, targetValue);
        currentState = targetState;
        isTransitioning = false;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    public string GetCurrentStateName()
    {
        return currentState?.stateName ?? "None";
    }
}