using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialTransparent : MonoBehaviour
{
    // Start is called before the first frame update
    public Material TargetObjectMaterial; // Material of the target object
    public float Alpha = 128f; // Target alpha value

    private float originalSurface;       // To store the original _Surface value
    private float originalBlend;         // To store the original _Blend value
    private int originalRenderQueue;     // To store the original render queue
    private Color originalBaseColor;     // To store the original Base Map color
    private bool originalTransparentKeyword; // To track the original keyword state

    void Awake()
    {
        if (TargetObjectMaterial != null)
        {
            // Store the original material properties
            originalSurface = TargetObjectMaterial.GetFloat("_Surface");
            originalRenderQueue = TargetObjectMaterial.renderQueue;
            originalBaseColor = TargetObjectMaterial.GetColor("_BaseColor");
            originalTransparentKeyword = TargetObjectMaterial.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT");
        }
        //SetMaterialtoTransparent();
        TargetObjectMaterial.SetFloat("_Surface", 1); // 1 means Transparent, 0 means Opaque

        // Enable transparency keyword
        TargetObjectMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        TargetObjectMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        // Adjust render queue for transparency
        TargetObjectMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        float normalizedAlpha = Alpha / 255f;

        // Get the current Base Map color
        Color baseColor = TargetObjectMaterial.GetColor("_BaseColor");

        // Set the new alpha value
        baseColor.a = normalizedAlpha;

        // Apply the modified color back to the material
        TargetObjectMaterial.SetColor("_BaseColor", baseColor);
    }

    /*private void SetMaterialtoTransparent()
    {
        TargetObjectMaterial.SetFloat("_Surface", 1); // 1 means Transparent, 0 means Opaque

        // Enable transparency keyword
        TargetObjectMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        TargetObjectMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        // Adjust render queue for transparency
        TargetObjectMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        SetBaseMapAlpha(Alpha);
    }

    private void SetBaseMapAlpha(float alpha)
    {
        float normalizedAlpha = alpha / 255f;

        // Get the current Base Map color
        Color baseColor = TargetObjectMaterial.GetColor("_BaseColor");

        // Set the new alpha value
        baseColor.a = normalizedAlpha;

        // Apply the modified color back to the material
        TargetObjectMaterial.SetColor("_BaseColor", baseColor);
    }*/

    private void RevertMaterialToOriginal()
    {
        if (TargetObjectMaterial == null) return;

        // Revert the surface type
        TargetObjectMaterial.SetFloat("_Surface", originalSurface);

        // Revert render queue
        TargetObjectMaterial.renderQueue = originalRenderQueue;

        // Revert Base Map color
        TargetObjectMaterial.SetColor("_BaseColor", originalBaseColor);

        // Revert transparency keywords
        if (originalTransparentKeyword)
        {
            TargetObjectMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            TargetObjectMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");
        }
        else
        {
            TargetObjectMaterial.EnableKeyword("_SURFACE_TYPE_OPAQUE");
            TargetObjectMaterial.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
    }

    void OnDisable()
    {
        // Restore the original material state when exiting play mode
        if (TargetObjectMaterial != null)
        {
            RevertMaterialToOriginal();
        }
    }


}
