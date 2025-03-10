using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlankProgressTracker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject progressDialog;

    [Header("Progress Settings")]
    [SerializeField] private float maxProgress = 100f;
    [SerializeField] private float currentProgress = 0f;
    [SerializeField] private float progressIncreaseRate = 1f;

    private bool isProcessing = false;

    private void Start()
    {
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
            currentProgress += progressIncreaseRate * Time.deltaTime;
            currentProgress = Mathf.Clamp(currentProgress, 0f, maxProgress);
            
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
            progressText.text = $"Progress: {Mathf.Round(currentProgress)}%";
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