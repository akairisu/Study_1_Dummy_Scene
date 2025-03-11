using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlankProgressTracker3 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject progressDialog;

    [Header("Targets")]
    [SerializeField] private List<GameObject> targets;

    private int totalTargets;
    private int destroyedTargets;
    private bool isProcessing = true;

    private void Start()
    {
        // Initialize target counts
        totalTargets = targets.Count;
        destroyedTargets = 0;

        if (progressSlider != null)
        {
            progressSlider.maxValue = totalTargets;
            progressSlider.value = destroyedTargets;
        }
        UpdateProgressDisplay();
    }

    private void Update()
    {
        if (isProcessing)
        {
            UpdateProgress();
        }
    }

    public void StartProcessing()
    {
        isProcessing = true;
        if (progressDialog != null)
        {
            progressDialog.SetActive(true);
        }
    }

    public void StopProcessing()
    {
        isProcessing = false;
    }

    public void ResetProgress()
    {
        destroyedTargets = 0;
        if (progressSlider != null)
        {
            progressSlider.value = destroyedTargets;
        }
        UpdateProgressDisplay();
    }

    private void UpdateProgress()
    {
        // Count destroyed/inactive targets
        destroyedTargets = 0;
        foreach (GameObject target in targets)
        {
            if (target == null || !target.activeInHierarchy)
            {
                destroyedTargets++;
            }
        }
            
        if (progressSlider != null)
        {
            progressSlider.value = destroyedTargets;
        }
            
        UpdateProgressDisplay();

        // Check if all targets are destroyed
        if (destroyedTargets >= totalTargets)
        {
            StopProcessing();
        }
    }

    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            progressText.text = $"Progress: {destroyedTargets}/{totalTargets}";
        }
    }

    public int GetDestroyedTargets()
    {
        return destroyedTargets;
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }
} 