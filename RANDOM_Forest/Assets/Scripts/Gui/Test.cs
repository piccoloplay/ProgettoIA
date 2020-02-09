using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;


public class Test : MonoBehaviour
{
    public static List<string> testingSet;
    public Dropdown Positive;
    public Dropdown Negative;
    public Dropdown Class3;
    public Dropdown Class4;
    public Dropdown TestingSet;
    public TextMeshProUGUI trainigTestSize, testingSetSize;
    public TextMeshProUGUI trainingSet, testingset;
    public Button backToHome;
    private ConfusionMatrix confusion;
    public TextMeshProUGUI measureDisplayed;//got it
    public TextMeshProUGUI time;
    public TextMeshProUGUI numberOfTrees;//got it

    public TextMeshProUGUI memoryConsumed;// ??

    public TextMeshProUGUI accuracy;
    public TextMeshProUGUI errorRate;
    PerformanceClassification p;
    public TextMeshProUGUI dataSetName;
    [SerializeField]
    private Image accuracyBar;
    [SerializeField]
    private Image errorBar;
    // printare la confusion

     void Awake()
    {
        DateTime _start=DateTime.Now;
        int forest = setNumberOfTrees();
        int dataSet = SettingGui.dataSet;
        Measure measure = setMeasure();
        switch (dataSet)
        {
            case 1:
                performanceIONO(forest,measure);
                break;
            case 2:
                performanceGerman(forest,measure);
                break;
            case 3:
                performanceBreastCancer(forest,measure);
                break;
            case 4:
                performanceVehicle(forest,measure);
                break;
            case 5:
                performanceDiabetes(forest,measure);
                break;
 
        }
        TimeSpan elapsed = DateTime.Now - _start;
        time.text = "Time consumed: " + elapsed.Minutes +"min : "+elapsed.Seconds+ "''";
        confusion = p.C;
        setAccuracyErrorRate();
        ConfusionMatrixDropDown();
        SetTestingExamples();
        string memorya = GetAllocatedMemory();
        memoryConsumed.text ="Memory consumed: "+ memorya;
    }
    // Start is called before the first frame update
    void Start()
    {
        /*setAccuracyErrorRate();
        double finishTime = Time.realtimeSinceStartup;
        time.text = "Time consumed: " + finishTime.ToString() + " sec";
        Debug.Log(finishTime);*/

    }
    
    public void performanceBreastCancer(int forest, Measure measure)
    {
        p = new PerformanceClassification("cancer", ',', 11, 10, measure);
        p.PerformanceRandomForest(forest);
        dataSetName.text = "DataSet: BreastCancer";
    }
    private string GetAllocatedMemory(bool forceFullCollection = false)
    {
        double bytes = GC.GetTotalMemory(forceFullCollection);

        return $"{((bytes / 1024d) / 1024d).ToString("N2")} MB";
    }
    public void setAccuracyErrorRate()
    {
        accuracy.text = "Accuracy: "+confusion.Accuracy().ToString();
        errorRate.text = "ErrorRate: "+confusion.ErrorRate().ToString();
    }

    public void performanceVehicle(int forest, Measure measure)
    {
        p = new PerformanceClassification("veichle", ',', 19, 18, measure);
        p.PerformanceRandomForest(forest);
        dataSetName.text = "DataSet: Vehicle";
    }
    
    public void performanceDiabetes(int forest, Measure measure)
    {
        p = new PerformanceClassification("diabetes", ',', 9, 8, measure);
        p.PerformanceRandomForest(forest);
        dataSetName.text = "DataSet: Diabetes";
    }
    
    public void performanceGerman(int forest, Measure measure)
    {
        p = new PerformanceClassification("german", ',', 21, 20, measure);
        p.PerformanceRandomForest(forest);
        dataSetName.text = "DataSet: German CreditCard";
    }


  

    public void performanceIONO(int forest, Measure measure)
    {
        p = new PerformanceClassification("iono2", ',', 35, 34, measure);
        p.PerformanceRandomForest(forest);
        dataSetName.text = "DataSet: IonoSphere";
    }


    public Measure setMeasure()
    {
        Measure measure = SettingGui.measure == 1 ? measure = Measure.ENTROPY : Measure.GINI;
        measureDisplayed.text = "Measure: "+measure.ToString();
        return measure;       
    }

    public int setNumberOfTrees()
    {
        int forest = SettingGui.forestDimension;
        numberOfTrees.text = "Number of Trees used: "+forest.ToString();
        return forest;

    }
    

    public void LoadHome()
    {
        SettingGui.single_RandomInput = false;
        SettingGui.oneHot = false;
        SettingGui.measure = 1;
        SettingGui.seed = 23;
        DecisionTree.unseenValues = 0;
        Example.count = -1;
        DecisionTreeMaker.count = -1;
        SceneManager.LoadScene(0);
    }


    public void SetConfusionMatrix()
    {
        Destroy(Class3.gameObject);
        Destroy(Class4.gameObject);
        string TP = "TP";
        string FP = "FP";
        string TN = "TN";
        string FN = "FN";

        List<string> positive = new List<string>();
        positive.Add("CLASS 1");
        List<string> negative = new List<string>();
        negative.Add("CLASS 2");
        for(int i = 0; i < confusion.Matrix.GetLength(0); i++)
        {
            if (i == 0)
            {
                positive.Add(TP+": "+confusion.Matrix[0, i].ToString());
                negative.Add(FN+": "+confusion.Matrix[1, i].ToString());
            }
            else
            {
                positive.Add(FP+": "+confusion.Matrix[0, i].ToString());
                negative.Add(TN+": "+confusion.Matrix[1, i].ToString());
            }
        }
        
        Positive.AddOptions(positive);
        Negative.AddOptions(negative);
        Debug.Log("TestingSet size: " + p.Ds.TrainingSet1.Count.ToString());
        Debug.Log("TestingSet size: " + p.Ds.TestingSet1.Count.ToString());
        testingSetSize.text = "TestingSet size: "+p.Ds.TestingSet1.Count.ToString();
        trainigTestSize.text = "TrainingSet size: "+p.Ds.TrainingSet1.Count.ToString();
    }

    public void SetConfusionMatrixVehicle()
    {
        Debug.Log("enabled");
        Class3.enabled = true;
        Class4.enabled = true;
        List<string> class1 = new List<string>();
        class1.Add("CLASS 1");
        List<string> class2 = new List<string>();
        class2.Add("CLASS 2");
        List<string> class3 = new List<string>();
        class3.Add("CLASS 3");
        List<string> class4 = new List<string>();
        class4.Add("CLASS 4");
        for (int i = 0; i < confusion.Matrix.GetLength(0); i++)
        {
           
                class1.Add( confusion.Matrix[0, i].ToString());
                class2.Add( confusion.Matrix[1, i].ToString());
            class3.Add(confusion.Matrix[2, i].ToString());
            class4.Add(confusion.Matrix[3, i].ToString());

        }

        Positive.AddOptions(class1);
        Negative.AddOptions(class2);
        Class3.AddOptions(class3);
        Class4.AddOptions(class4);
        Debug.Log("TestingSet size: " + p.Ds.TrainingSet1.Count.ToString());
        Debug.Log("TestingSet size: " + p.Ds.TestingSet1.Count.ToString());
        testingSetSize.text = "TestingSet size: " + p.Ds.TestingSet1.Count.ToString();
        trainigTestSize.text = "TrainingSet size: " + p.Ds.TrainingSet1.Count.ToString();
    }
    public void SetTestingExamples()
    {
        TestingSet.AddOptions(testingSet);
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

    public void ConfusionMatrixDropDown()
    {
        if (SettingGui.dataSet == 4)
        {
            SetConfusionMatrixVehicle();
        }
        else
        {
            SetConfusionMatrix();
        }
    }
}




