using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Image progressionBar;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(1);

        /*while (gameLevel.progress < 1)
        {


            //progressionBar.fillAmount = gameLevel.progress;
            yield return new WaitForEndOfFrame();
        }*/
        yield return new WaitForEndOfFrame();
    }
}
