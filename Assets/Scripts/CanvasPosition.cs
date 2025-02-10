using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUp ()
    {
        transform.position += new Vector3(0, 0.02f, 0);
    }

    public void MoveDown ()
    {
        transform.position += new Vector3(0, -0.02f, 0);
    }
}
