using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestMorph : MonoBehaviour
{
    [Tooltip("Game object's mesh should be read/write enabled from the model import settings")]
    [SerializeField] private GameObject _oldObj;
    [Tooltip("Game object's mesh should be read/write enabled from the model import settings")]
    [SerializeField] private GameObject _newObj;

    [Range(0f, 1f)]
    [SerializeField] private float _slider = 0;

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
    private Vector3 _newScale;

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

    // Create pairs of vertices, each pair contains a vertex from the old mesh and a vertex from the new mesh.
    // Vertices are mapped by the nearest one to each other.
    private void CreatePairs1()
    {
        _pairsOfVertices1 = new List<VertexPair>();

        for (int i = 0; i < _oldVertices.Length; i++)
        {
            var oldVertex = _oldVertices[i];

            var nearestToOldVertex = _newVertices.OrderBy(v => Vector3.Distance(v, oldVertex)).FirstOrDefault();

            _pairsOfVertices1.Add(new VertexPair(oldVertex, nearestToOldVertex));
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

            _pairsOfVertices2.Add(new VertexPair(newVertex, nearestToNewVertex));
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

        _oldScale = _oldObj.transform.localScale;
        _newScale = _newObj.transform.localScale;

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

        //_finalMaterial.Lerp(_oldMat, _newMat, _slider);
        //_finalMaterial.SetTexture("_MainTex", _slider < 0.5f ? _oldMat.GetTexture("_MainTex") : _newMat.GetTexture("_MainTex"));
        _finalMaterial = new Material(_slider < 0.5 ? _oldMat : _newMat);
        _renderer.material = _finalMaterial;

        _oldObj.transform.position = Vector3.Lerp(_oldPosition, _newPosition, _slider);
        _oldObj.transform.localRotation = _slider < 0.5f ? _oldRotation : _newRotation;
        _oldObj.transform.localScale = _slider < 0.5f ? _oldScale : _newScale;
    }
    // Update is called once per frame
    void Update()
    {
        _newObj.SetActive(false);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed!");
            if (!_morphed)
            {
                InitMorph();
                _morphed = true;
            }
            _morphingToNew = !_morphingToNew;
        }
        if (_morphed)
        {
            SetSlider();
            MorphObject();
        }
    }
}
