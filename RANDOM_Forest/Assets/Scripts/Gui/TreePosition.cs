using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePosition : MonoBehaviour
{
    public void Awake()
    {
        List<float> coordinatesX = PossibleX();
        int index = Random.Range(0, coordinatesX.Count);
        transform.position = new Vector3(coordinatesX[index], 0, transform.position.z);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public List<float> PossibleX()
    {
        List<float> coordinatesX = new List<float>();
        for(int i = -260; i <= 210; i = i + 10)
        {
            coordinatesX.Add(i);
        }
        return coordinatesX;
    }

}
