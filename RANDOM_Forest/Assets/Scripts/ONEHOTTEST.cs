using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONEHOTTEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*DataSet ds = new DataSet("oneHotEncode",',', 3, 1);
        ds.printAttributes();
        ds.OneHotEncode();
        ds.printINdexesHooott();
        Debug.Log("ciaooo");
        ds.printExamp(ds.Examples);
        ds.printAttributes();*/

        DataSet ds = new DataSet("german", ',', 21, 20);
        //ds.printAttributes();
        //ds.OneHotEncode();
        ds.printExamp(ds.Examples);
        Debug.Log(ds.Target);
        /*
        
         * 
         */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
