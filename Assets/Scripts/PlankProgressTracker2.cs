using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlankProgressTracker2 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject progressDialog;

    [Header("Targets")]
    [SerializeField] private TriggerZonePlaner target1;
    [SerializeField] private TriggerZonePlaner target2;
    [SerializeField] private TriggerZonePlaner target3;
    [SerializeField] private TriggerZonePlaner target4;

    [Header("Progress Settings")]
    [SerializeField] private float maxProgress = 100f;
    [SerializeField] private float currentProgress = 0f;

    private bool isProcessing = true;

    private void Start()
    {
        // reset maxProgress to target value
        maxProgress = target1.TargetDistance + target2.TargetDistance + target3.TargetDistance + target4.TargetDistance;

        if (progressSlider != null)
        {
            progressSlider.maxValue = maxProgress;
            progressSlider.value = currentProgress;
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
        currentProgress = 0f;
        if (progressSlider != null)
        {
            progressSlider.value = currentProgress;
        }
        UpdateProgressDisplay();
    }

    private void UpdateProgress()
    {
        if (currentProgress < maxProgress)
        {
            currentProgress = target1.CurrentDistance + target2.CurrentDistance + target3.CurrentDistance + target4.CurrentDistance;
            
            if (progressSlider != null)
            {
                progressSlider.value = currentProgress;
            }
            
            UpdateProgressDisplay();
        }
        else
        {
            StopProcessing();
        }
    }

    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            float progressPercentage = (currentProgress / maxProgress) * 100;
            progressText.text = $"Progress: {progressPercentage.ToString("F2")}%";
        }
    }

    public float GetProgress()
    {
        return currentProgress;
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }
} 