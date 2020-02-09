using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScrolling : MonoBehaviour
{
    public Vector3 center;
    private Vector3 v;// = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if()
        //transform.LookAt();
        //Debug.Log(center.x + " " + center.y + " " + center.z);
        //v = v -0.001f * (center - transform.position).normalized;
        v = new Vector3(0, 0, 0.03f);
        //v = new Vector3(0, 0, 0.002f).normalized;
        transform.position +=  v;
    }



}
