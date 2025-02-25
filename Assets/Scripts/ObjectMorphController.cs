using UnityEngine;
using UnityEngine.VFX;

public class ObjectMorphController : MonoBehaviour
{
    [Header("Objects and VFX")]
    public GameObject originalObject;
    public GameObject morphedObject;
    public VisualEffect vfxGraph;

    [Header("Settings")]
    public float transitionDuration = 2.0f; // Total transition time
    private float counter = 0f;
    private bool isTransitioning = false;

    private Material originalMaterial;
    private Material morphedMaterial;

    private Mesh originalMesh;
    private Mesh morphedMesh;

    private static readonly int TransbarID = Shader.PropertyToID("_transbar");
    private static readonly int TurbulenceID = Shader.PropertyToID("Turbulance");
    private static readonly int OriginMeshID = Shader.PropertyToID("OriginMesh");

    void Start()
    {
        // Get the materials
        originalMaterial = originalObject.GetComponent<Renderer>().material;
        morphedMaterial = morphedObject.GetComponent<Renderer>().material;

        // Get the meshes
        originalMesh = originalObject.GetComponent<MeshFilter>().mesh;
        morphedMesh = morphedObject.GetComponent<MeshFilter>().mesh;

        // Initialize shader values
        originalMaterial.SetFloat(TransbarID, 0);
        morphedMaterial.SetFloat(TransbarID, 1);

        // Disable VFX at start
        vfxGraph.Stop();
    }

    void Update()
    {
        // Test trigger using Space Key
        if (Input.GetKeyDown(KeyCode.Space) && !isTransitioning)
        {
            StartTransition();
        }

        if (isTransitioning)
        {
            UpdateTransition();
        }
    }

    public void StartTransition()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        counter = 0f;

        // Set initial VFX properties
        vfxGraph.SetMesh(OriginMeshID, originalMesh);
        vfxGraph.SetFloat(TurbulenceID, 0);
        vfxGraph.Play();
    }

    private void UpdateTransition()
    {
        counter += Time.deltaTime;
        float progress = counter / transitionDuration;

        if (progress < 0.5f)
        {
            // First half of transition
            float lerpValue = progress * 2; // Normalize to [0, 1] range
            originalMaterial.SetFloat(TransbarID, Mathf.Lerp(0, 1, lerpValue));
            morphedMaterial.SetFloat(TransbarID, 1); // Keep morphed object at 1
            vfxGraph.SetFloat(TurbulenceID, Mathf.Lerp(0, 1, lerpValue));
        }
        else
        {
            // Second half of transition
            float lerpValue = (progress - 0.5f) * 2; // Normalize second half to [0, 1] range
            originalMaterial.SetFloat(TransbarID, 1); // Keep original object at 1
            morphedMaterial.SetFloat(TransbarID, Mathf.Lerp(1, 0, lerpValue));
            vfxGraph.SetFloat(TurbulenceID, Mathf.Lerp(1, 0, lerpValue));

            // Swap VFX mesh at midpoint
            if (progress >= 0.5f && vfxGraph.GetMesh(OriginMeshID) != morphedMesh)
            {
                vfxGraph.SetMesh(OriginMeshID, morphedMesh);
            }
        }

        // End transition
        if (counter >= transitionDuration)
        {
            isTransitioning = false;
            vfxGraph.Stop();

            // Ensure final values
            originalMaterial.SetFloat(TransbarID, 1);
            morphedMaterial.SetFloat(TransbarID, 0);
        }
    }
}
