using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

/// <summary>
/// A variant of Grabbable that only detects hand interactions without transforming the object.
/// This allows external systems (like OptiTrack) to control the object's position and rotation.
/// </summary>
public class NonTransformableGrabbable : Grabbable
{
    private bool _throwWhenUnselected = false;
    private Rigidbody _rigidbody;
    private VelocityEstimator _velocityEstimator;
    protected Transform _targetTransform;
    protected ITransformer OneGrabTransformer { get; set; }
    protected ITransformer TwoGrabTransformer { get; set; }

    protected override void Awake()
    {
        base.Awake();
        // Disable transformers
        OneGrabTransformer = null;
        TwoGrabTransformer = null;
        
        // Get required components
        _rigidbody = GetComponent<Rigidbody>();
        _velocityEstimator = GetComponent<VelocityEstimator>();
    }

    protected override void Start()
    {
        this.BeginStart(ref _started, () => base.Start());

        if (_targetTransform == null)
        {
            _targetTransform = transform;
        }

        // Initialize velocity estimator if needed
        if (_rigidbody != null && _throwWhenUnselected && _velocityEstimator != null)
        {
            _velocityEstimator.BeginEstimatingVelocity();
        }

        this.EndStart(ref _started);
    }

    public override void ProcessPointerEvent(PointerEvent evt)
    {
        // Keep only the base pointer event processing
        base.ProcessPointerEvent(evt);
    }

    // Override and empty out transformation methods
    private void BeginTransform()
    {
        // Transformation disabled
    }

    private void UpdateTransform()
    {
        // Transformation disabled
    }

    private void EndTransform()
    {
        // Transformation disabled
    }
} 