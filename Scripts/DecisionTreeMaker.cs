using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionTreeMaker 
{
    public static int count = -1;
    private int id;
   
    private Measure myMeasure;

    private int indexTargetClass;

    public int Id { get => id; set => id = value; }
    public int IndexTargetClass { get => indexTargetClass; set => indexTargetClass = value; }
    public Measure MyMeasure { get => myMeasure; set => myMeasure = value; }

    public DecisionTreeMaker(DataSet ds, int indexTarget)
    {

        IndexTargetClass = indexTarget;
    }

    public DecisionTreeMaker(DataSet ds, int indexTarget,Measure measure)
    {
        count++;
        id = count;
        MyMeasure = measure;
        IndexTargetClass = indexTarget;
    }

    /*
     * PSEUDOCODE pag.702
     * 
     function DECISION-TREE-LEARNING(examples,attributes,parent examples) 
     returns a tree 
     if examples is empty then return PLURALITY-VALUE(parent examples)
     else if all examples have the same classiﬁcation then return the classiﬁcation 
     else if attributes is empty then return PLURALITY-VALUE(examples) 
     else 
        A←argmaxa ∈ attributes IMPORTANCE(a,examples) 
        tree ←a new decision tree with root test A 
        foreach value vk of A do 
            exs ←{e : e∈examples and e.A = vk} 
            subtree ←DECISION-TREE-LEARNING(exs,attributes −A,examples) 
            add a branch to tree with label (A = vk) and subtree subtree 
        return tree

         
    */
    public DecisionTree BuildTree(List<Example>examples,List<Attribute>attributes,List<Example>parentExamples)
    {
        if (examples.Count == 0)
        {
            return Plurality_Value(parentExamples);
        }
        else if (SameClassification(examples)) 
        {
            Example one = examples[0];
            string result = one.getTarget(IndexTargetClass);
            return new DecisionTree(result);
        }
        else if (attributes.Count == 0)
        {
            return Plurality_Value(examples);
        }
        else
        {
            Attribute A = IMPORTANCE(attributes, examples);
            DecisionTree tree = new DecisionTree(A.AttributeName); //radice
            attributes.Remove(A);
            if (A.MyType == typeOfAttribute.Categorical)
            {
                foreach (string vk in A.Istances)
                {
                    List<Example> subExs = SplitDataSet(examples, vk,A.Index);
                    // update istanze
                    List<Attribute> subAttributes = updateAttributes(subExs,attributes);
                    DecisionTree subTree = BuildTree(subExs, subAttributes, examples);
                    tree.AddNode(vk, subTree);
                }
            }
            else
            {
                List<Example> subExsTestPassed = SplitDataSetForContinuesValues(examples, A);
                List<Example> subExsTestNotPassed = SplitDataSetForContinuesValues_TestNotPassed(examples,A);
                List<Attribute> subAttributesP = updateAttributes(subExsTestPassed, attributes);
                List<Attribute> subAttributesNP = updateAttributes(subExsTestNotPassed, attributes);
                DecisionTree subTree1 = BuildTree(subExsTestPassed, subAttributesP, examples);
                DecisionTree subTree2 = BuildTree(subExsTestNotPassed, subAttributesNP, examples);
                string trasholdMajor = ">"+A.Trashold.ToString();
                string trasholdMinor = "<=" + A.Trashold.ToString();
                tree.AddNode(trasholdMajor, subTree1);
                tree.AddNode(trasholdMinor, subTree2);
            }
            return tree;
        }     
    }

    public List<Attribute> updateAttributes(List<Example>examples,List<Attribute>attributes)
    {
        List<Attribute> updateAtt = new List<Attribute>();
        foreach(Attribute attr in attributes)
        {
            Attribute attribute = (Attribute)attr.Clone();
            attribute.UpdateIstences(examples);
            updateAtt.Add(attribute);
        }
        return updateAtt; 
    }


    private List<Example> SplitDataSetForContinuesValues_TestNotPassed(List<Example> examples, Attribute attr)
    {
        return examples.FindAll(x => !x.PassTheTest(attr.AttributeName, attr.Trashold));
    }


    private List<Example> SplitDataSetForContinuesValues(List<Example> examples, Attribute attr)
    {

        return examples.FindAll(x => x.PassTheTest(attr.AttributeName,attr.Trashold));
    }

    private List<Example> SplitDataSet(List<Example> examples, string vk, int attribute)
    {
        return examples.FindAll(x => x.Contains(vk,attribute));
    }

    private Attribute ImportanceInformationGain(List<Attribute> attributes, List<Example> examples)
    {
        List<string> targetList = TargetList(examples);
        double greatestGain = 0.0;
        Attribute attributeWithBestGain = attributes[0];
        foreach (Attribute attr in attributes)
        {
            double gain = attr.CalculateInformationGain(examples,targetList);
            if (gain > greatestGain)
            {
                    greatestGain = gain;
                    attributeWithBestGain = attr;
            }
        }
        return  attributeWithBestGain;
    }
    
    private Attribute ImportanceImpurity(List<Attribute> attributes, List<Example> examples)
    {
        // RANDOM FOREST HOOK
        List<string> targetList = TargetList(examples);
        double minImpurity = Mathf.Infinity;
        Attribute attributeWithBestGain = attributes[0];
        foreach (Attribute attr in attributes)
        {

            double impurity = attr.CalculateImpurity(examples, targetList);
          //  Debug.Log(impurity + "gini");
            if (impurity < minImpurity)
            {

                minImpurity = impurity;
                attributeWithBestGain = attr;

            }
        }

        
        return attributeWithBestGain;

    }



    private Attribute IMPORTANCE(List<Attribute> attributes1, List<Example> examples)
    {
        if (!SettingGui.single_RandomInput)
        {
            List<Attribute> attributes = Selection(attributes1);    
            Attribute bestSPlitAttribute = MyMeasure == Measure.GINI ? bestSPlitAttribute = ImportanceImpurity(attributes, examples) : bestSPlitAttribute = ImportanceInformationGain(attributes, examples);
            return bestSPlitAttribute;
        }
        else
        {
            System.Random rand = new System.Random(id);
            int randAttribute = rand.Next(attributes1.Count);
            Attribute luckyAttribute = attributes1[randAttribute];
            List<string> targetList = TargetList(examples);
            double t=luckyAttribute.CalculateInformationGain(examples,targetList);
            //return attributes1[randAttribute];
            return luckyAttribute;
        }
    }


    public Attribute SingleInput()
    {
        
        return null;
    }



    public List<Attribute> Selection(List<Attribute>attributes)
    {
        System.Random rand = new System.Random(id);
        List<Attribute> a = new List<Attribute>();
        int F = (int)Math.Log(attributes.Count, 2)+1;
        for(int i = 0; i < F; i++)
        {
            int index = rand.Next(attributes.Count);
            if (!a.Contains(attributes[index])) { a.Add(attributes[index]); } else
            {
                i--;
            }
        }
        return a;
    }

    private bool SameClassification(List<Example>examples)
    {
        string s=examples[0].getTarget(IndexTargetClass);

        return examples.Count==examples.FindAll(x=>x.ContainsTarget(s)).Count;
    }

    private DecisionTree Plurality_Value(List<Example> parentExamples)
    {

        double max = Mathf.NegativeInfinity; //flag
        string result="Unable to classify";
        foreach(string s in TargetList(parentExamples))
        {
            if (parentExamples.FindAll(x=>x.ContainsTarget(s)).Count>max)
            {
                max = parentExamples.FindAll(x => x.ContainsTarget(s)).Count;
                result = s;
            }
        }
      
        return new DecisionTree(result);
        
    }


    public List<string> ExpectedValues(List<Example> examples)
    {
        List<string> targetList = new List<string>();
        foreach (Example example in examples)
        {
             targetList.Add(example.Record[IndexTargetClass]);
        }
        return targetList;
    }
   
    public List<string> TargetList(List<Example>examples)
    {
        List<string> targetList = new List<string>();
        foreach (Example example in examples)
        {
            if (!targetList.Contains(example.Record[IndexTargetClass])) targetList.Add(example.Record[IndexTargetClass]);
        }
        return targetList;
    }

   

   public void printAttributes(List<Attribute>attributes)
    {
        string s = "";
        foreach(Attribute attr in attributes)
        {
            s += attr.AttributeName+" | ";
        }
        Debug.Log(s+"numero attributi: "+attributes.Count);
    }


    public void printIstances(Attribute attr)
    {
        string result = attr.AttributeName+": ";
        foreach(string s in attr.Istances)
        {
            result += s + " ";
        }
        Debug.Log(result);
    }

    public void printTargetList(List<string> targetList)
    {
        string result = indexTargetClass+": ";
     foreach(string s in targetList)
        {
            result += s + " ";
        }
        Debug.Log(result);
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


}







public enum Measure { GINI, ENTROPY };


