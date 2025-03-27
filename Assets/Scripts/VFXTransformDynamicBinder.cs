using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Transform/Dynamic Transform")]
public class VFXTransformDynamicBinder : VFXBinderBase
{
    [VFXPropertyBinding("UnityEngine.Matrix4x4")]
    public ExposedProperty transformProperty = "ObjectTransform";

    [SerializeField]
    public Transform target;

    public override bool IsValid(VisualEffect component)
    {
        return target != null && component != null && component.HasMatrix4x4(transformProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        if (target == null || component == null)
            return;

        try
        {
            Matrix4x4 matrix = target.localToWorldMatrix;
            component.SetMatrix4x4(transformProperty, matrix);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to update VFX transform binding: {e.Message}");
        }
    }
}
