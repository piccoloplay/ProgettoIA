using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Load : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(LoadForest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    IEnumerator LoadForest()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(2);

        yield return null;
    }
}
