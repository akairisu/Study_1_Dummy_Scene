using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZonePlaner : MonoBehaviour
{
    public float TargetDistance = 20f;
    public bool IsTriggered;
    public float CurrentDistance = 0;
    public string PlanerTag = "Planer";
    public Transform TargetPlane;
    public Material IdleMaterial;  // Material for untriggered state
    public Material TriggeredMaterial;  // Material for triggered state
    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _currentPosition;
    private AudioSource _audioSource;
    private Transform _currentPlaner;
    private Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<Renderer>();
        if (_renderer != null && IdleMaterial != null)
        {
            _renderer.material = IdleMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlanerTag))
        {
            IsTriggered = true;
            _currentPlaner = other.transform;
            if (_renderer != null && TriggeredMaterial != null)
            {
                _renderer.material = TriggeredMaterial;
            }
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlanerTag))
        {
            IsTriggered = false;
            _currentPlaner = null;
            _lastPosition = Vector3.zero;
            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            if (_renderer != null && IdleMaterial != null)
            {
                _renderer.material = IdleMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTriggered && _currentPlaner != null)
        {
            if (_lastPosition == Vector3.zero)
            {
                _lastPosition = TargetPlane.InverseTransformPoint(_currentPlaner.position);
            }
            _currentPosition = TargetPlane.InverseTransformPoint(_currentPlaner.position);
            float moveDistance = Mathf.Abs(_currentPosition.x - _lastPosition.x);
            
            if (moveDistance > 0.001f)
            {
                CurrentDistance += moveDistance;
                if (_audioSource != null && !_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource != null)
                {
                    _audioSource.Stop();
                }
            }
            _lastPosition = _currentPosition;
            if (CurrentDistance > TargetDistance)
            {
                gameObject.transform.GetComponent<Renderer>().enabled = false;
                gameObject.transform.GetComponent<MeshCollider>().isTrigger = false;
                IsTriggered = false;
                if (_audioSource != null)
                {
                    _audioSource.Stop();
                }
                if (_renderer != null && TriggeredMaterial != null)
                {
                    _renderer.material = TriggeredMaterial;
                }
            }
        }
    }
}

