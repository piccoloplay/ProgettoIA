using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingGui : MonoBehaviour
{

    public Toggle singleInputToggle;
    public static bool single_RandomInput=false;
    public static bool oneHot = false;
    public Toggle oneHotEncoding;
    public static int dataSet;
    public static int measure=1;
    public static int forestDimension;
    public string numberOfTrees;
    public TextMeshProUGUI txt;
    public Slider slider;
    public InputField seedInput;
    public static int seed=23;

    public void SetDataSetIono()
    {
        dataSet = 1;
        StartCoroutine(LoadingScene());
    }
    public void SetDataSetGerman()
    {
        dataSet = 2;
        StartCoroutine(LoadingScene());
    }
    public void SetDataSetCancer()
    {
        dataSet = 3;
        StartCoroutine(LoadingScene());
    }
    public void SetDataSetVehicle()
    {
        dataSet = 4;
        StartCoroutine(LoadingScene());
    }

    public void SetDataSetDiabetes()
    {
        dataSet = 5;
        StartCoroutine(LoadingScene());
    }

    private void Awake()
    {
      
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        forestDimension = (int)slider.value;
        numberOfTrees = "Number of Trees: " + slider.value;
        txt.text = numberOfTrees;
        setSeed();
    }
   

    
   public void setGini(bool b)
    {
        
        measure = 2;
        Debug.Log(measure);
    }
   
    public void setEntropy(bool b)
    {
        measure = 1;
        Debug.Log(measure);
    }

    public void SetEncoding(bool encoding)
    {

        if (oneHotEncoding.isActiveAndEnabled)
        {
            oneHot = false;
        }
        if (oneHotEncoding.isOn)
        {
            oneHot = true;
        }
        
        
        Debug.Log("OneHotEncoding "+oneHot);
    }


    public void SetF(bool encoding)
    {

        
        if (singleInputToggle.isOn)
        {
            single_RandomInput = true;
        }
        else
        {
            single_RandomInput = false;
        }


        Debug.Log("Single Random Input: "+single_RandomInput);
    }



    public void setSeed()
    {
        double t=0;
        if (double.TryParse(seedInput.text.ToString(), out t))
        {
            seed = int.Parse(seedInput.text.ToString());
            Debug.Log("Seed choosed: "+seed);
        }
        
    }
    

    IEnumerator LoadingScene()
    {
        
        yield return new WaitForSeconds(0.03f);
        SceneManager.LoadScene(1);
        
    }



    IEnumerator CloseApp()
    {
        yield return new WaitForSeconds(0.02f);
        Application.Quit();
    }

    public void CloseAppEvent()
    {
        StartCoroutine(CloseApp());
    }
}
