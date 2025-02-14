using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Editor;
using static Oculus.Interaction.TransformerUtils;

public class SawCollisionHandler : MonoBehaviour
{
    /*public GrabFreeTransformer GrabFreeTransformer;
    public OneGrabTranslateTransformer OneGrabTranslateTransformer;
    public Grabbable Grabbable;
    public Transform ParentObject;
    private bool _isSnapped = false;*/
    public Material TriggerMaterial;
    public Material UntriggerMaterial;

    /*void Start()
    {
        if (GrabFreeTransformer == null)
        {
            Debug.LogWarning("No GrabFreeTransformer found on the saw.");
        }
    }*/

    /*void OnCollisionEnter(Collision collision)
    {
        if (!_isSnapped)
        {
            _isSnapped = true;
            Vector3 collidedObjectPosition = collision.gameObject.transform.position;
            Debug.Log("Collided Object Position: " + collidedObjectPosition);
            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;

            Vector3 offset = ParentObject.position - transform.TransformPoint(contactPoint);
            Vector3 targetPos = collidedObjectPosition + offset;
            ParentObject.position = targetPos;

            TransformerUtils.PositionConstraints _positionConstraints =
            new TransformerUtils.PositionConstraints()
            {
                XAxis = new TransformerUtils.ConstrainedAxis(),
                YAxis = new TransformerUtils.ConstrainedAxis(),
                ZAxis = new TransformerUtils.ConstrainedAxis()
            };
            _positionConstraints.XAxis.ConstrainAxis = true;
            _positionConstraints.YAxis.ConstrainAxis = true;
            _positionConstraints.XAxis.AxisRange = new FloatRange { Min = targetPos.x, Max = targetPos.x };
            _positionConstraints.YAxis.AxisRange = new FloatRange { Min = targetPos.y, Max = targetPos.y };
            GrabFreeTransformer.InjectOptionalPositionConstraints(_positionConstraints);
            GrabFreeTransformer.UpdateTransform();

            OneGrabTranslateTransformer.Constraints.MinX.Constrain = true;
            OneGrabTranslateTransformer.Constraints.MinY.Constrain = true;
            OneGrabTranslateTransformer.Constraints.MaxX.Constrain = true;
            OneGrabTranslateTransformer.Constraints.MaxY.Constrain = true;
            OneGrabTranslateTransformer.Constraints.MinX.Value = targetPos.x;
            OneGrabTranslateTransformer.Constraints.MinY.Value = targetPos.y;
            OneGrabTranslateTransformer.Constraints.MaxX.Value = targetPos.x;
            OneGrabTranslateTransformer.Constraints.MaxY.Value = targetPos.y;
            Grabbable.InjectOptionalOneGrabTransformer(OneGrabTranslateTransformer);
            OneGrabTranslateTransformer.UpdateTransform();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        _isSnapped = false;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = TriggerMaterial;
            }
            other.GetComponent<TriggerZoneSaw>().IsTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other != null)
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = UntriggerMaterial;
            }
            other.GetComponent<TriggerZoneSaw>().IsTriggered = false;
        }
    }
}

