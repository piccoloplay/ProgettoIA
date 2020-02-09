using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class RANDOMFOREST 
{
    //private static ThreadLocal<System.Random> m_rnd = new ThreadLocal<System.Random>(() => new System.Random());
    [SerializeField]
    
    private List<Example> testingSet;
    private List<Example> trainingSet;
    private List<string> predictedValues = new List<string>();
    private List<string> expectedValues = new List<string>();
    private List<DecisionTree> forest = new List<DecisionTree>();
    private List<string> targetList;
   
    public List<string> PredictedValues { get => predictedValues; set => predictedValues = value; }
    public List<string> ExpectedValues { get => expectedValues; set => expectedValues = value; }
    public List<string> TargetList { get => targetList; set => targetList = value; }
    

    public RANDOMFOREST(DataSet ds, DecisionTreeMaker dt, int numberOfTrees)
    {
        trainingSet = ds.TrainingSet1;
        testingSet = ds.TestingSet1;
        ds.UpdateAttributeIstances(trainingSet);
        ds.SortCategoricalFeatures(trainingSet);
        BuildRandomForest(ds,dt,numberOfTrees);
        PredictDataSet();
    }

    public void perFormance(DataSet ds, DecisionTreeMaker dt, int numberOfTrees)
    {
        BuildRandomForest(ds, dt, numberOfTrees);
        PredictDataSet();
    }


    public string Classify(Example example)
    {
        List<string> predictionSet = new List<string>();
        foreach(DecisionTree tree in forest)
        {
            string predictedValue = tree.Classify(example);
            predictionSet.Add(predictedValue);
        }
        return MajorityVote(predictionSet);
    }
    
    public string MajorityVote(List<string>votes)
    {
        int max = 0;
        string result = TargetList[0];
        foreach(string target in TargetList)
        {
            int occurence = votes.FindAll(x => x.Contains(target)).Count;
            if (occurence > max)
            {
                max = occurence;
                result = target;
            }
        }
        return result;
    }



    public List<Example> BootStrap(List<Example>examples, int seed)
    {
        System.Random rand = new System.Random(seed);
        List<Example> result = new List<Example>();
        for(int i = 0; i < examples.Count; i++)
        {
            int randomIndex = rand.Next(examples.Count);
            Example example = examples[randomIndex];
            result.Add(example);
        }
        //printIndexes(result);
        return result;
    }

    public void PredictDataSet()
    {
        foreach(Example example in testingSet)
        {
            string predictedValue = Classify(example);
            PredictedValues.Add(predictedValue);
        }
    }


    public void BuildRandomForest(DataSet ds, DecisionTreeMaker dt, int numberOfForest)
    {
        TargetList = dt.TargetList(trainingSet);
        expectedValues = dt.ExpectedValues(testingSet);
        List<DecisionTreeMaker> builder = new List<DecisionTreeMaker>();
        for (int i = 0; i < numberOfForest; i++)
        {
            DecisionTreeMaker dtt = new DecisionTreeMaker(ds, dt.IndexTargetClass, dt.MyMeasure);
            builder.Add(dtt);
        }
        builder.AsParallel().ForAll(x=> {
            List<Attribute> attributes = new List<Attribute>(ds.Attributes);
            List<Example> examples = BootStrap(trainingSet,x.Id);
            forest.Add(x.BuildTree(examples, attributes, null)); } );
    }
    
    public List<int> Seeds()
    {
        List<int> seeds = new List<int>();
        for(int i = 0; i < 100; i++)
        {
            seeds.Add(i);
        }
        return seeds;
    }

    public void printIndexes(List<Example>examples)
    {
        string s = "Boostrap";
        foreach(Example example in examples)
        {
            s += example.Id + " ";
        }
        Debug.Log(s);
    }


    public void printVotes(List<string> votes)
    {
        string result = "votes: ";
        foreach(string s in votes)
        {
            result += " "+s;
        }
        Debug.Log(result);
    }

}
