using MeshTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundTest : MonoBehaviour
{

    public BoundEffect boundEffect;
    public Renderer target;

    public void Set()
    {
        boundEffect.SetEffect(target.bounds, Vector3.zero);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(1);
            Set();
        }
    }

}
