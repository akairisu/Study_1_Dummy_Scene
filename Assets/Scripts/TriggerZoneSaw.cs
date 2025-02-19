using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class TriggerZoneSaw : MonoBehaviour
{
    public float TargetDistance = 10f;
    public bool IsTriggered;
    public float CurrentDistance = 0;
    public GameObject Saw;
    private float _lastSawZPosition = 0;
    private float _currentZPosition;
    public GameObject SlicedObject;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (IsTriggered)
        {
            if(_lastSawZPosition == 0)
            {
                _lastSawZPosition = Saw.transform.position.z;
            }
            _currentZPosition = Saw.transform.position.z;
            float moveDistance = Mathf.Abs(_lastSawZPosition - _currentZPosition);
            //Debug.Log("Move Distance:" + moveDistance);
            if(moveDistance > 0.001f)
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
            _lastSawZPosition = _currentZPosition;
            if (CurrentDistance > TargetDistance)
            {
                //Destroy(gameObject);
                gameObject.transform.GetComponent<Renderer>().enabled = false;
                gameObject.transform.GetComponent<MeshCollider>().isTrigger = false;
                IsTriggered = false;
                Rigidbody rb = SlicedObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                MeshCollider collider = SlicedObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }
        else
        {
            _audioSource.Stop();
            _lastSawZPosition = 0;
        }
    }
}
