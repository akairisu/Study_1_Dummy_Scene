using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Morph : MonoBehaviour
{
    [Tooltip("Game object's mesh should be read/write enabled from the model import settings")]
    [SerializeField] private GameObject _oldObj;
    [Tooltip("Game object's mesh should be read/write enabled from the model import settings")]
    [SerializeField] private GameObject _newObj;

    //[SerializeField] private GameObject _holdPoint;

    [Range(0f, 1f)]
    [SerializeField] private float _slider = 0;

    private bool _standardFlag = false;
    public float NewAlpha = 0.33f;

    public float InterpolateRatio = 0.3f;
    
    private Material _oldMat;
    private Material _newMat;

    private MeshFilter _meshFilter;
    private Renderer _renderer;

    private bool _morphingToNew = false;
    private bool _morphed = false;

    private Mesh _oldMesh;
    private Mesh _newMesh;

    private Vector3[] _oldVertices;
    private Vector3[] _newVertices;

    private int[] _oldTriangles;
    private int[] _newTriangles;

    private Vector3 _oldPosition;
    private Vector3 _newPosition;

    private Quaternion _oldRotation;
    private Quaternion _newRotation;

    private Vector3 _oldScale;
    private Vector3 _newOriginalScale;
    private Vector3 _alignedScale;

    private List<Vector3> _finalVertices;

    private Mesh _interpolatedMesh;
    private List<VertexPair> _pairsOfVertices1;
    private List<VertexPair> _pairsOfVertices2;

    private Material _finalMaterial;

    [SerializeField] private float _speed = 1f;
    private float _maxX = 1f;
    private float _minX = 0f;

    public class VertexPair
    {
        public Vector3 Vertex1 { get; set; }
        public Vector3 Vertex2 { get; set; }

        public VertexPair(Vector3 vertex1, Vector3 vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
    }

    void Start()
    {
        AlignSize(_oldObj, _newObj);
        AlignCenter(_oldObj, _newObj);
        MakeTransparent(_newObj);
        InitMorph();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed!");
            if (!_morphed)
            {
                ResetMaterial(_newObj);
 
                _newObj.SetActive(false);
                _morphed = true;
            }
            _morphingToNew = !_morphingToNew;
        }
        SetSlider();
        MorphObject();
    }

    //Obtain the real scale of game object
    private Vector3 GetScale(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        Vector3 objScale = Vector3.one;
        if (objCollider != null)
        {
            Vector3 localColliderSize = objCollider.bounds.size;
            //Debug.Log("Object Local collider size: " + localColliderSize);
            objScale = Vector3.Scale(localColliderSize, transform.lossyScale);
            //Debug.Log("Object World collider size: " + objScale);
        }
        return objScale;
    }

    //Align the true size of two objects according the scale obtained from GetScale()
    private void AlignSize(GameObject oldObj, GameObject newObj)
    {
        Vector3 oldObjScale = GetScale(oldObj);
        Vector3 newObjScale = GetScale(newObj);

        _alignedScale = new Vector3(
            oldObjScale.x / newObjScale.x,
            oldObjScale.y / newObjScale.y,
            oldObjScale.z / newObjScale.z
        );

        //Debug.Log(oldObjScale);
        //Debug.Log(newObjScale);
        //Debug.Log(_alignedScale);

        _oldScale = oldObj.transform.localScale;
        _newOriginalScale = newObj.transform.localScale;
        newObj.transform.localScale = _alignedScale;
    }

    //Get the mesh's center position in the world space
    Vector3 GetWorldSpaceCenter(GameObject obj)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError("No MeshRenderer found on the object");
            return Vector3.zero;
        }

        // Get the center of the bounds in local space and convert it to world space
        Vector3 localCenter = renderer.bounds.center;
        return localCenter;
    }

    //Align the position of two meshes according to the position obtained ftom GetWorldSpaceCenter()
    private void AlignCenter(GameObject oldObj, GameObject newObj)
    {
        if (oldObj == null || newObj == null)
        {
            Debug.LogError("Objects must not be null!");
            return;
        }

        Vector3 oldCenter = GetWorldSpaceCenter(oldObj);
        //Debug.Log(oldCenter);
        Vector3 newCenter = GetWorldSpaceCenter(newObj);
        //Debug.Log(newCenter);

        newObj.transform.position += (oldCenter - newCenter);
    }

    //Set a material to semi-transparent by adjusting render mode & alpha value
    private void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3); // Sets the material to Transparent
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        Color color = mat.GetColor("_Color");
        color.a = NewAlpha;
        mat.SetColor("_Color", color);
    }

    //Check if the material's shader is standard shader
    private void CheckStandardShader(Material mat)
    {
        if (mat.shader.name == "Standard")
        {
            Debug.Log("Standard!");
            _standardFlag = true;
        }
        else
        {
            Debug.Log("Not standard shader!");
        }
    }

    //Make the new object semi-transparent for preview
    private void MakeTransparent(GameObject obj)
    {
        Material _mat = obj.GetComponent<Renderer>().material;
        CheckStandardShader(_mat);
        if (_standardFlag)
        {
            SetMaterialTransparent(_mat);
        }
    }

    //Set a material to visible by adjusting render mode & alpha value
    private void SetMaterialVisible(Material mat)
    {
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.renderQueue = -1;
        Color color = mat.GetColor("_Color");
        color.a = 1.0f;
        mat.SetColor("_Color", color);
    }

    //Make the new object visible again after the selection is confirmed
    private void ResetMaterial(GameObject obj)
    {
        Material _mat = obj.GetComponent<Renderer>().material;
        CheckStandardShader(_mat);
        if (_standardFlag)
        {
            SetMaterialVisible(_mat);
        }
    }

    //
    private void ResetBound(Mesh mesh, GameObject obj)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 meshScale = _alignedScale;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], meshScale);
        }
        // Set the scaled vertices back to the mesh
        mesh.vertices = vertices;

        mesh.RecalculateBounds();
        obj.transform.localScale = _newOriginalScale * 1f;
    }

    /*private float CalculateDistanceRatio(Vector3 vertex)
    {
        Vector3 worldHoldVertexPos = _holdPoint.transform.TransformPoint(_holdPoint.transform.position);
        Vector3 worldVertexPos = _oldObj.transform.TransformPoint(vertex);
        float distance = Vector3.Distance(worldHoldVertexPos, worldVertexPos);
        return (distance < 0.42f) ? 0f : distance - 0.42f;
    }*/

    // Create pairs of vertices, each pair contains a vertex from the old mesh and a vertex from the new mesh.
    // Vertices are mapped by the nearest one to each other.
    private void CreatePairs1()
    {
        _pairsOfVertices1 = new List<VertexPair>();

        for (int i = 0; i < _oldVertices.Length; i++)
        {
            var oldVertex = _oldVertices[i];

            var nearestToOldVertex = _newVertices.OrderBy(v => Vector3.Distance(v, oldVertex)).FirstOrDefault();

            Vector3 interpolateVertex = Vector3.Lerp(oldVertex, nearestToOldVertex, InterpolateRatio);

            _pairsOfVertices1.Add(new VertexPair(oldVertex, interpolateVertex));
        }
    }

    // Create pairs of vertices, each pair contains a vertex from the new mesh and a vertex from the old mesh.
    // Vertices are mapped by the nearest one to each other.
    private void CreatePairs2()
    {
        _pairsOfVertices2 = new List<VertexPair>();

        for (int i = 0; i < _newVertices.Length; i++)
        {
            var newVertex = _newVertices[i];

            var nearestToNewVertex = _oldVertices.OrderBy(v => Vector3.Distance(v, newVertex)).FirstOrDefault();

            Vector3 interpolateVertex = Vector3.Lerp(nearestToNewVertex, newVertex, InterpolateRatio);

            _pairsOfVertices2.Add(new VertexPair(interpolateVertex, nearestToNewVertex));
        }
    }

    private void InitMorph()
    {
        _oldMesh = _oldObj.GetComponent<MeshFilter>().mesh;
        _newMesh = _newObj.GetComponent<MeshFilter>().mesh;

        _oldMat = _oldObj.GetComponent<Renderer>().material;
        _newMat = _newObj.GetComponent<Renderer>().material;

        _meshFilter = _oldObj.GetComponent<MeshFilter>();
        _renderer = _oldObj.GetComponent<Renderer>();

        _interpolatedMesh = new Mesh();
        _interpolatedMesh.MarkDynamic();

        ResetBound(_newMesh, _newObj);

        if (_meshFilter != null)
        {
            _meshFilter.mesh = _interpolatedMesh;
        }

        _oldVertices = _oldMesh.vertices;
        _newVertices = _newMesh.vertices;

        _oldTriangles = _oldMesh.triangles;
        _newTriangles = _newMesh.triangles;

        _oldPosition = _oldObj.transform.position;
        _newPosition = _newObj.transform.position;

        _oldRotation = _oldObj.transform.localRotation;
        _newRotation = _newObj.transform.localRotation;

        _finalVertices = new List<Vector3>(_oldVertices);

        CreatePairs1();
        CreatePairs2();

        _finalMaterial = new Material(_slider < 0.5 ? _oldMat : _newMat);
    }

    public void SetSlider()
    {
        if (_morphingToNew)
        {
            _slider = Mathf.Lerp(_slider, _maxX, Time.deltaTime * _speed);
        }
        else
        {
            _slider = Mathf.Lerp(_slider, _minX, Time.deltaTime * _speed);
        }
    }

    // Morph the mesh based on the slider value.
    private void MorphObject()
    {
        if (_slider < 0.5f)
        {
            _finalVertices = _pairsOfVertices1.Select(p => Vector3.Lerp(p.Vertex1, p.Vertex2, _slider)).ToList();
        }
        else
        {
            _finalVertices = _pairsOfVertices2.Select(p => Vector3.Lerp(p.Vertex2, p.Vertex1, _slider)).ToList();
        }

        _interpolatedMesh.Clear();

        _interpolatedMesh.SetVertices(_finalVertices);
        _interpolatedMesh.triangles = _slider < 0.5f ? _oldTriangles : _newTriangles;


        _interpolatedMesh.bounds = _slider < 0.5f ? _oldMesh.bounds : _newMesh.bounds;
        _interpolatedMesh.uv = _slider < 0.5f ? _oldMesh.uv : _newMesh.uv;
        _interpolatedMesh.uv2 = _slider < 0.5f ? _oldMesh.uv2 : _newMesh.uv2;
        _interpolatedMesh.uv3 = _slider < 0.5f ? _oldMesh.uv3 : _newMesh.uv3;

        _interpolatedMesh.RecalculateNormals();

        _finalMaterial.Lerp(_oldMat, _newMat, _slider);
        _finalMaterial.SetTexture("_MainTex", _slider < 0.5f ? _oldMat.GetTexture("_MainTex") : _newMat.GetTexture("_MainTex"));
        
        _renderer.material = _finalMaterial;

        _oldObj.transform.position = Vector3.Lerp(_oldPosition, _newPosition, _slider);
        _oldObj.transform.localScale = _slider < 0.5f ? _oldScale : _newOriginalScale;
        _oldObj.transform.localRotation = _slider < 0.5f ? _oldRotation : _newRotation;
    }
}
