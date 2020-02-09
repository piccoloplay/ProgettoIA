using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataSet 
{
    private int encoded=0;
    public List<int> indexesHotEncode = new List<int>();
    public DataSet(string document, char separator, int numberAttributes, int target)
    {
        builder = new LoadData(document, separator,target);
        builder.BuildDataSet(Examples,Attributes,numberAttributes);
        if(SettingGui.dataSet==3)BruteForceImputation("?");
        this.Target = target;
        if(SettingGui.oneHot) OneHotEncode();
        TrainingSet();
        TestingSet();
        SetTesigGUI();
    }

    private int target;
    private List<Example> trainingSet;
    private List<Example> testingSet;
    private LoadData builder;
    private List<Attribute> attributes = new List<Attribute>();

    

    public void UpdateAttributeIstances(List<Example>examples)
    {
        foreach (Attribute attr in attributes)
        {
            attr.UpdateIstences(examples);
        }
    }
    
    public void SortCategoricalFeatures(List<Example>examples)
    {
        foreach(Attribute attribute in attributes)
        {
            if(attribute.MyType==typeOfAttribute.Categorical) attribute.SortCategoricalFeature(examples);


        }

    }

    private List<Example> examples = new List<Example>();

    
    public List<Attribute> Attributes { get => attributes; set => attributes = value; }
    public List<Example> Examples { get => examples; set => examples = value; }
    public List<Example> TrainingSet1 { get => trainingSet; set => trainingSet = value; }
    public int Target { get => target; set => target = value; }
    public List<Example> TestingSet1 { get => testingSet; set => testingSet = value; }

    public void Imputation(string missSymbol, int attribute)
    {
        Attribute attr = Attributes[attribute];
        string replacemantSample = attr.MyType==typeOfAttribute.Categorical ? replacemantSample=ValueMaxFrequency(attr, missSymbol):replacemantSample=Mean(attr).ToString();
        examples.ForEach(example => { if (example.Contains(missSymbol, attribute))example.Replace(replacemantSample, attribute); });
    }

    public string ValueMaxFrequency(Attribute attr, string flag)
    {
        string result = attr.Istances[0];
        double max = Mathf.NegativeInfinity;
        foreach(string istance in attr.Istances)
        {
            if (istance.Equals(flag)) continue;
            double occurence = examples.FindAll(x => x.Contains(istance,attr.Index)).Count;
            if (occurence > max)
            {
                max = occurence;
                result = istance;
            }
        }
        
        
        return result;
    }

    public double Mean(Attribute attribute)
    {

        List<double> workingList = new List<double>(attribute.NumbersIstances);
        double mean = 0;
        workingList.ForEach(x => mean += x);
        mean = mean / workingList.Count;

        return mean;
    }


    public void BruteForceImputation(string missinSymbol)
    {
        for(int i = 0; i < attributes.Count; i++)
        {

            Imputation(missinSymbol, i);
        }
    }


    public void TrainingSet()
    {
        List<int> indexes = new List<int>();
        System.Random rand = new System.Random(SettingGui.seed);
         trainingSet = new List<Example>();
        int myBoyTrainer = (int)Math.Round((double)9/10*examples.Count);
        for (int i = 0; i < myBoyTrainer; i++)
        {
            int index = rand.Next(examples.Count);
            if (!indexes.Contains(index)) {
                indexes.Add(index);
                Example example = Examples[index];
                trainingSet.Add(Examples[index]);
            } else
            {
                i--;
            }
        }
    }

    public void TestingSet()
    {
        testingSet = new List<Example>();
        List<int> indexesT = indexes(trainingSet);
        for(int i = 0; i < examples.Count; i++)
        {
            if (!indexesT.Contains(i))
            {
                testingSet.Add(examples[i]);
            }
        }
    }

    public List<Example> DummyDataSet()
    {
        List<Example> dummyDataSet = new List<Example>();
        for(int i = 0; i < 600; i++)
        {
            dummyDataSet.Add(examples[i]);
        }
        attributes.ForEach(x => x.UpdateIstences(dummyDataSet));
        //BruteForceImputation
        return dummyDataSet;


    }

    public List<int> indexes(List<Example> examples)
    {
        List<int> result = new List<int>();
        examples.ForEach(x => result.Add(x.Id));
        return result;
    }


    public void printExamp(List<Example> examples)
    {
        Debug.Log("*****************");
        
        foreach (Example example in examples)
        {
            example.printRecord();
            
        }
        Debug.Log("*****************");
    }

    public void printCount()
    {
        Debug.Log("*****************");
        int start = examples[0].Record.Count;
        foreach (Example example in examples)
        {
            if (example.Record.Count != start)
            {
                example.printRecord();
                Debug.Log(example.Id);
            }

        }
        Debug.Log("*****************");

    }
    public double Entropy(List<Example> examples, List<string> target)
    {
        double occurence;
        double total = examples.Count;
        double entropy = 0.0;
        double pi;



        foreach (string s in target)
        {

            occurence = examples.FindAll(x => x.ContainsTarget(s)).Count;

            pi = occurence / total;

            entropy = entropy + (pi * Math.Log(pi, 2));
        }


        return entropy * -1;
    }


    public void OneHotEncode()
    {
        List<string> dummyInstance = new List<string>();
        dummyInstance.Add("0");
        dummyInstance.Add("1");
        List<Attribute> workingList = new List<Attribute>(attributes);
        for(int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i].MyType == typeOfAttribute.Categorical)
            {
                if (i == Target) continue;
                Encode(attributes[i],workingList,dummyInstance);
                indexesHotEncode.Add(i);
                encoded++;
            }
        }
        Target = Target - encoded;
        attributes = workingList;
        ClearOneHotEncode();
        UpdateEncodeAttributes();
    }

    private void Encode(Attribute attribute,List<Attribute>dummies,List<string>dummyInstance)
    {
        foreach(string dummy in attribute.Istances)
        {
            Attribute dummyAttr = new Attribute(dummies.Count,dummyInstance,typeOfAttribute.Categorical);
            dummies.Add(dummyAttr);
            EncodeExample(dummy, attribute.Index);
            
        }
        //dummies.Remove(attribute);
    }


    public void EncodeExample(string s, int attribute)
    {

        examples.ForEach(x => { if (x.Contains(s, attribute))
            {
                x.Record.Add("1");
            }
            else
            {
                x.Record.Add("0");
            }
            //x.Record.RemoveAt(attribute);
        });


    }

    public void ClearOneHotEncode()
    {
        int temp = 0;
        int workIndex=0;
        /*foreach(int sadIndex in indexesHotEncode)
        {
            workIndex=sadIndex - temp;
            Debug.Log(sadIndex + " sadindex " + workIndex + " workindex " + temp + " temp");
            temp++;
            
            attributes.RemoveAt(workIndex);
            examples.ForEach(x=>x.Record.RemoveAt(workIndex));
        }*/


        for(int i = 0; i < indexesHotEncode.Count; i++)
        {
            workIndex = indexesHotEncode[i] - temp;
            //Debug.Log(indexesHotEncode[i] + " sadindex " + workIndex + " workindex " + temp + " temp");
            temp++;

            attributes.RemoveAt(workIndex);
            examples.ForEach(x => x.Record.RemoveAt(workIndex));

        }

    }


    public void UpdateEncodeAttributes()
    {

        for(int i = 0; i < attributes.Count; i++)
        {
            attributes[i].AttributeName = i.ToString();
            attributes[i].Index = i;
        }
    }


    public void printAttributes()
    {
        foreach(Attribute attr in attributes)
        {
            
            attr.printInfoAttribute();
        }
    }


    public void printINdexesHooott()
    {
        string s = "";
        foreach(int sss in indexesHotEncode)
        {
            s += sss+" ";
        }
        Debug.Log(s);
    }



    public void printINdexes(List<int>indexes)
    {
        string result = "";
        foreach(int i in indexes)
        {
            result += i+" ";
        }
        Debug.Log(result);
    }

    public void SetTesigGUI()
    {
        Test.testingSet = new List<string>();
        Test.testingSet.Add("TestingSet");
        foreach(Example example in TestingSet1)
        {
            Test.testingSet.Add(example.Id.ToString());
        }
    }

}
