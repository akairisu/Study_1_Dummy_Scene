using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiObjectShaderTransition : MonoBehaviour
{
    [Header("Object Groups")]
    public List<GameObject> originalObjects = new List<GameObject>();
    public List<GameObject> transformedObjects = new List<GameObject>();

    [Header("Weight Points and Pivots")]
    public Vector3 originalWeightPoint;
    public Vector3 transformedWeightPoint;
    public Transform originalPivot;
    public Transform transformedPivot;

    [Header("Transition Settings")]
    public float transitionDuration = 1.5f;
    [Range(0f, 1f)] public float transitionSlider = 0f;

    [Header("Shader Offset Settings")]
    public float boundaryOffset = 0.1f;

    [Header("Shader Additional Parameters")]
    public float roundRadius = 0.5f;
    public float transLineThickness = 0.1f;
    public float transLineNoiseScale = 1.0f;
    public float transLineNoiseSpeed = 1.0f;
    [ColorUsage(true, true)] public Color transLineColor = Color.white;

    private float intersectionLowestY;
    private float intersectionHighestY;

    private List<Material> originalMaterials = new List<Material>();
    private List<Material> transformedMaterials = new List<Material>();

    private Vector3 originalWeightWorld;
    private Vector3 transformedWeightWorld;

    void Start()
    {
        InitializeMaterials();
        CalculateWorldSpaceBounds();
        CalculateWeightPointsWorld();
        UpdateShader(transitionSlider);
    }

    void Update()
    {
        UpdateShader(transitionSlider);
    }

    void InitializeMaterials()
    {
        originalMaterials.Clear();
        transformedMaterials.Clear();

        foreach (GameObject obj in originalObjects)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
                originalMaterials.AddRange(rend.materials);
        }

        foreach (GameObject obj in transformedObjects)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
                transformedMaterials.AddRange(rend.materials);
        }
    }

    void CalculateWorldSpaceBounds()
    {
        float overallLowestY = float.MaxValue;
        float overallHighestY = float.MinValue;

        foreach (GameObject obj in originalObjects)
        {
            float objMinY, objMaxY;
            CalculateWorldMinMax(obj, out objMinY, out objMaxY);
            overallLowestY = Mathf.Min(overallLowestY, objMinY);
            overallHighestY = Mathf.Max(overallHighestY, objMaxY);
        }

        foreach (GameObject obj in transformedObjects)
        {
            float objMinY, objMaxY;
            CalculateWorldMinMax(obj, out objMinY, out objMaxY);
            overallLowestY = Mathf.Min(overallLowestY, objMinY);
            overallHighestY = Mathf.Max(overallHighestY, objMaxY);
        }

        intersectionLowestY = overallLowestY - boundaryOffset;
        intersectionHighestY = overallHighestY + boundaryOffset;
    }

    void CalculateWorldMinMax(GameObject obj, out float minY, out float maxY)
    {
        minY = float.MaxValue;
        maxY = float.MinValue;

        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
            return;

        foreach (Vector3 vertex in mf.sharedMesh.vertices)
        {
            Vector3 worldVertex = obj.transform.TransformPoint(vertex);
            if (worldVertex.y < minY) minY = worldVertex.y;
            if (worldVertex.y > maxY) maxY = worldVertex.y;
        }
    }

    void CalculateWeightPointsWorld()
    {
        originalWeightWorld = originalPivot.TransformPoint(originalWeightPoint);
        transformedWeightWorld = transformedPivot.TransformPoint(transformedWeightPoint);
    }

    void UpdateShader(float t) {
        // If transition is at 0 or 1, set currentY to a very low value to hide the line

        float currentY = (t == 0f) ? -100000f : (t == 1f) ? 100000f : Mathf.Lerp(intersectionLowestY, intersectionHighestY, t);
        Vector3 currentWeightPosition = Vector3.Lerp(originalWeightWorld, transformedWeightWorld, t);

        foreach (Material mat in originalMaterials)
        {
            mat.SetFloat("_BaseTransparency", 1f);
            mat.SetFloat("_TransitionLineY", currentY);
            mat.SetInt("_ShowUpperPart", 1);
            mat.SetVector("_WeightPosition", currentWeightPosition);
            mat.SetFloat("_RoundRadius", roundRadius);
            mat.SetFloat("_TransLineThickness", transLineThickness);
            mat.SetFloat("_TransLineNoiseScale", transLineNoiseScale);
            mat.SetFloat("_TransLineNoiseSpeed", transLineNoiseSpeed);
            mat.SetColor("_TransLineColor", transLineColor);
        }

        foreach (Material mat in transformedMaterials)
        {
            mat.SetFloat("_TransitionLineY", currentY);
            mat.SetInt("_ShowUpperPart", 0);
            mat.SetVector("_WeightPosition", currentWeightPosition);
            mat.SetFloat("_RoundRadius", roundRadius);
            mat.SetFloat("_TransLineThickness", transLineThickness);
            mat.SetFloat("_TransLineNoiseScale", transLineNoiseScale);
            mat.SetFloat("_TransLineNoiseSpeed", transLineNoiseSpeed);
            mat.SetColor("_TransLineColor", transLineColor);
        }
    }

    public void StartTransition(bool forward = true)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(forward));
    }

    IEnumerator TransitionRoutine(bool forward)
    {
        float elapsed = 0f;
        float from = forward ? 0f : 1f;
        float to = forward ? 1f : 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            transitionSlider = Mathf.Lerp(from, to, elapsed / transitionDuration);
            UpdateShader(transitionSlider);
            yield return null;
        }

        transitionSlider = to;
        UpdateShader(transitionSlider);
    }
}
