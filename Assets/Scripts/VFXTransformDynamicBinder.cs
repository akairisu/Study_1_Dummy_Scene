using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Transform/Dynamic Transform")]
public class VFXTransformDynamicBinder : VFXBinderBase
{
    [VFXPropertyBinding("UnityEngine.Matrix4x4")]
    public ExposedProperty transformProperty = "ObjectTransform";

    public Transform target;

    public override bool IsValid(VisualEffect component)
    {
        return target != null && component.HasMatrix4x4(transformProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetMatrix4x4(transformProperty, target.localToWorldMatrix);
    }
}
