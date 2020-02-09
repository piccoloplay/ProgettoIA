using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceClassification 
{
    private DataSet ds;
    private DecisionTreeMaker dm;
    private ConfusionMatrix c;

    public ConfusionMatrix C { get => c; set => c = value; }
    public DataSet Ds { get => ds; set => ds = value; }

    public PerformanceClassification(string document, char separator, int attributes, int targetIndex, Measure measure)
    {
        Ds = new DataSet(document, separator, attributes, targetIndex);
        Ds.Attributes.Remove(Ds.Attributes[targetIndex]);
        dm = new DecisionTreeMaker(Ds, Ds.Target, measure);
    }


    

    public void PerformanceTree()
    {
        //ds.printCount();
        //ds.printExamp(ds.DummyDataSet());
       // ds.UpdateAttributeIstances(ds.DummyDataSet());
        DecisionTree tree = dm.BuildTree(Ds.DummyDataSet(), Ds.Attributes, Ds.DummyDataSet());
        //string f=tree.Classify(ds.Examples[0]);
        List<string> predicted = new List<string>();
        foreach (Example example in Ds.DummyDataSet())
        {
            predicted.Add(tree.Classify(example));
        }
        List<string> classes = dm.TargetList(Ds.DummyDataSet());
        List<string> expected = dm.ExpectedValues(Ds.DummyDataSet());
        ConfusionMatrix c = new ConfusionMatrix(predicted, expected, classes);
        c.printMatrix(c.Matrix);
        Debug.Log(c.Accuracy() + " Accuracy");
        Debug.Log(c.ErrorRate() + " ErrorRate");
      
    }
   
    public void PerformanceRandomForest(int forest)
    {
        RANDOMFOREST rm = new RANDOMFOREST(Ds, dm, forest);
        c = new ConfusionMatrix(rm.PredictedValues, rm.ExpectedValues, rm.TargetList);
    }


    public void printClasses(List<string> classes)
    {

    }


    

}
