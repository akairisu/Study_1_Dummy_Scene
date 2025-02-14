using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZonePlaner : MonoBehaviour
{
    public float TargetDistance = 500f;
    public bool IsTriggered;
    public float CurrentDistance = 0;
    public Transform Planer;
    public Transform TargetPlane;
    public Material TargetMaterial;
    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _currentPosition;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        TargetMaterial.mainTextureScale = new Vector2(1f, 1f);
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTriggered)
        {
            if (_lastPosition == Vector3.zero)
            {
                _lastPosition = TargetPlane.InverseTransformPoint(Planer.position);
            }
            _currentPosition = TargetPlane.InverseTransformPoint(Planer.position);
            float moveDistance = Mathf.Abs(_currentPosition.x - _lastPosition.x);
            //Debug.Log("Move Distance:" + moveDistance);
            if (moveDistance > 0.001f)
            {
                CurrentDistance += moveDistance;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                _audioSource.Stop();
            }
            _lastPosition = _currentPosition;
            if (CurrentDistance > TargetDistance)
            {
                gameObject.transform.GetComponent<Renderer>().enabled = false;
                gameObject.transform.GetComponent<MeshCollider>().isTrigger = false;
                IsTriggered = false;
                TargetMaterial.mainTextureScale = new Vector2(10f, 1f);
            }
        }
        else
        {
            _audioSource.Stop();
            _lastPosition = Vector3.zero;
        }
    }
}
