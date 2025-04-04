using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneSaw : MonoBehaviour
{
    public float TargetDistance = 2f;
    public bool IsTriggered;
    public float CurrentDistance = 0;
    public string SawTag = "Saw";
    public GameObject SlicedObject;
    public Material IdleMaterial;  // Material for untriggered state
    public Material TriggeredMaterial;  // Material for triggered state
    private float _lastSawZPosition = 0;
    private float _currentZPosition;
    private AudioSource _audioSource;
    private Transform _currentSaw;
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
        if (other.CompareTag(SawTag))
        {
            IsTriggered = true;
            _currentSaw = other.transform;
            if (_renderer != null && TriggeredMaterial != null)
            {
                _renderer.material = TriggeredMaterial;
            }
            if (_audioSource != null && !_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(SawTag))
        {
            IsTriggered = false;
            _currentSaw = null;
            _lastSawZPosition = 0;
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
        if (IsTriggered && _currentSaw != null)
        {
            if(_lastSawZPosition == 0)
            {
                _lastSawZPosition = _currentSaw.position.z;
            }
            _currentZPosition = _currentSaw.position.z;
            float moveDistance = Mathf.Abs(_lastSawZPosition - _currentZPosition);
            
            if(moveDistance > 0.001f)
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
            _lastSawZPosition = _currentZPosition;
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
                
                // Add physics to sliced object
                Rigidbody rb = SlicedObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                MeshCollider collider = SlicedObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }
    }
}
