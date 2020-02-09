using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class Attribute : ICloneable
{



    public void SortCategoricalFeature(List<Example> trainingSet)
    {
        
        List<double> occurences = new List<double>();
        istances.ForEach(x =>
        {
            double num = trainingSet.FindAll(example => example.Contains(x, index)).Count;
            occurences.Add(num);
        });

        
        double tempNum;
        string temp;
        for (int j = 0; j < occurences.Count; j++)
        {
            for (int i = 0; i < occurences.Count - 1; i++)
            {
                if (occurences[i] > occurences[i + 1])
                {
                    tempNum = occurences[i + 1];
                    temp = istances[i + 1];
                    occurences[i + 1] = occurences[i];
                    istances[i + 1] = istances[i];
                    occurences[i] = tempNum;
                    istances[i] = temp;
                }
            }


        }
    }


    
    public object Clone()
    {
        int i = Index;
        string name = attributeName;
        List<string> ist = new List<string>(istances);
        List<double> dob = new List<double>(numbersIstances);
        typeOfAttribute type = myType;
        return new Attribute(i,name,ist,dob,type);
    }

    public Attribute(int index, string attributeName, List<string>istances, List<double>nums, typeOfAttribute myType)
    {
        this.Index = index;
        this.attributeName = attributeName;
        this.istances = istances;
        numbersIstances = nums;
        this.myType = myType;
    }

    public Attribute(int index, List<string> istances, typeOfAttribute myType)
    {
        this.Index = index;
        this.attributeName = index.ToString();
        this.istances = istances;
        
        this.myType = myType;
    }


    private int index;
    private typeOfAttribute myType;
    private List<string> istances = new List<string>();
    private List<double> numbersIstances = new List<double>();
    string attributeName;
    private double trashold;
   
    public string AttributeName { get => attributeName; set => attributeName = value; }
    public List<string> Istances { get => istances; set => istances = value; }
    public List<double> NumbersIstances { get => numbersIstances; set => numbersIstances = value; }
    public typeOfAttribute MyType { get => myType; set => myType = value; }
    public double Trashold { get => trashold; set => trashold = value; }
    public int Index { get => index; set => index = value; }

    public void UpdateIstences(List<Example>examples)
    {
        
        List<string> istanzeUpdate = new List<string>();
        List<double> numUpdate = new List<double>();
        for(int i = 0; i < istances.Count; i++)
        {
            
           
            if (examples.FindAll(x => x.Contains(istances[i],Index)).Count > 0)
            {
                istanzeUpdate.Add(istances[i]);
             
                if (myType == typeOfAttribute.Continuos) numUpdate.Add(numbersIstances[i]);
            }

        } 
        istances = istanzeUpdate;
        numbersIstances = numUpdate;
    }


    public void printIstances()
    {
        string sr = "Categorical: ";
        foreach(string s in istances)
        {
            sr += s+" "; 
        }
        Debug.Log(sr+" "+istances.Count );
    }

    public void printNumIst()
    {
        string sr = "Continuous: ";
        foreach(double num in numbersIstances)
        {
            sr += num + " ";
        }
        Debug.Log(sr+" "+numbersIstances.Count);
    }

    public void SetType()
    {
        MyType = numbersIstances.Count > 0 ? MyType = typeOfAttribute.Continuos : MyType = typeOfAttribute.Categorical;

    }


    public Attribute(int s)
    {
        AttributeName = s+"";
        Index = s;
    }


    public void Add(string s)
    {
        
        if (s.Equals("?")||s.Equals(" ")||s.Equals("")) return;
        
        double num;
        if (double.TryParse(s, out num))
        {
            num = double.Parse(s, CultureInfo.InvariantCulture);
            if (!numbersIstances.Contains(num))
            {
                NumbersIstances.Add(num);  
                istances.Add(s);

            }
            
        }
        else 
        {

            if (!Istances.Contains(s)) istances.Add(s);

        }
        

    }
    
    internal double CalculateInformationGain(List<Example> examples, List<string> targetList)
    {
        if (MyType == typeOfAttribute.Categorical)
        {
            return CalculateGainForCategorial(examples, targetList);
        }
        else
        {
            return CalculateGainForContinuos(examples, targetList);
        }


    }
    // gini
    internal double CalculateImpurity(List<Example> examples, List<string> targetList)
    {
        if (MyType == typeOfAttribute.Categorical)
        {
            return CalculateGainForCategorialGINI(examples, targetList);
        }
        else
        {
            return CalculateGainForContinuosGINI(examples, targetList);
        }


    }

    private double CalculateGainForCategorial(List<Example> examples, List<string> targetList)
    {
        double occurence;
        double occurenceVk;
        double total = examples.Count;
        double entropyVk = 0.0;
        double entropy = 0.0;
        double pi;
        double rapporto;

        foreach (string vk in istances)
        {
            occurence = examples.FindAll(x => x.Contains(vk,Index)).Count;
            //occurence = examples.AsParallel().Count(x => x.Contains(vk,index));
            if (occurence == 0) continue;//**
            rapporto = occurence / total;
            entropyVk = 0.0;
            foreach (string target in targetList)
            {
                occurenceVk = examples.FindAll(x => x.Contains(vk,Index) && x.ContainsTarget(target)).Count;
               // occurenceVk = examples.AsParallel().Count(x => x.Contains(vk,index) && x.ContainsTarget(target));
                if (occurenceVk == 0) continue; //**
                pi = occurenceVk / occurence;
                entropyVk = entropyVk + pi * Math.Log(pi, 2);
            }
            entropyVk *= -1;
            entropy = entropy + entropyVk * rapporto;
        }


        //Debug.Log(EntropyParent(examples, targetList) - entropy+"categorical");

        return EntropyParent(examples, targetList) - entropy;
    }

    public double EntropyParent(List<Example> examples, List<string> target)
    {
        double occurence;
        double total = examples.Count;
        double entropy = 0.0;
        double pi;



        foreach (string s in target)
        {

            occurence = examples.FindAll(x => x.ContainsTarget(s)).Count;
            //occurence = examples.AsParallel().Count(x => x.ContainsTarget(s));
            pi = occurence / total;

            entropy = entropy + (pi * Math.Log(pi, 2));
        }


        return entropy * -1;
    }



    private double CalculateGainForCategorialGINI(List<Example> examples, List<string> targetList)
    {
        double occurence;
        double occurenceVk;
        double total = examples.Count;
        double giniVK = 0.0;
        double impurity = 0.0;
        double pi;
        double rapporto;

        foreach (string vk in istances)
        {
            occurence = examples.FindAll(x => x.Contains(vk,Index)).Count;
            //occurence = examples.AsParallel().Count(x => x.Contains(vk,Index));
            if (occurence == 0) continue;//**
            rapporto = occurence / total;
            giniVK = 0.0;
            foreach (string target in targetList)
            {
                occurenceVk = examples.FindAll(x => x.Contains(vk,Index) && x.ContainsTarget(target)).Count;
                //occurenceVk = examples.AsParallel().Count(x => x.Contains(vk,index) && x.ContainsTarget(target));
                if (occurenceVk == 0) continue; //**
                pi = occurenceVk / occurence;
                giniVK = giniVK + (pi * pi);
            }
            giniVK = 1-giniVK;
            impurity = impurity + giniVK * rapporto;
        }


        //Debug.Log(EntropyParent(examples, targetList) - entropy+"categorical");

        return  impurity;
    }

    private double CalculateGainForContinuos(List<Example> examples, List<string> targetList)
    {

        TrasholdFinder tr = new TrasholdFinder(examples, targetList, this);
        //trashold = tr.ChooseTrasholder();
        trashold = tr.ChooseTrasholderEntropy();
        if (trashold == double.PositiveInfinity)
        {
            trashold = numbersIstances[0];
            //trashold = numbersIstances[0];
            //return 0;
            return CalculateGainForCategorial(examples, targetList);
        }
        // Debug.Log(tr.CalculateEntropy(trashold)+"continuous");
        return tr.InformationGain;
        return tr.CalculateEntropy(trashold);
    }


    private double CalculateGainForContinuosGINI(List<Example> examples, List<string> targetList)
    {

        TrasholdFinder tr = new TrasholdFinder(examples, targetList, this);
        trashold = tr.ChooseTrasholder();
        // Debug.Log(tr.CalculateEntropy(trashold)+"continuous");
        if (trashold == double.PositiveInfinity)
        {
            //trashold = numbersIstances[0];
            //return 1;

            return CalculateGainForCategorialGINI(examples,targetList);


        }
        return tr.CalculateEntropy(trashold);
    }








    public void printInfoAttribute()
    {
        Debug.Log(myType+" "+attributeName);
        string s0 = "";
        if (myType == typeOfAttribute.Continuos)
        {

            foreach (double d in numbersIstances)
            {
                s0 += d + " ";
            }
        }
        else
        {
            foreach (string s in istances)
            {
                string s1 = s;
                s0 += s1 + " ";
            }
        }
        Debug.Log(s0);
    }

    

}

// /// /////////////////////////////////////////////////////////////////////
// ************************************* //

public class TrasholdFinder
{

    private double trashold;
    private double informationGain;



    //int count = -1;
    private List<Example> examples;
    private List<string> targetList;
    private List<string> instances;
    private string index;
    private Attribute myatt;

    public double Trashold { get => trashold; set => trashold = value; }
    public double InformationGain { get => informationGain; set => informationGain = value; }

    public TrasholdFinder(List<Example>examples,List<string>targetList, Attribute attribute)
    {
        myatt = attribute;
        instances = new List<string>(attribute.Istances);
        this.examples = examples;
        this.targetList = targetList;
        index = attribute.AttributeName;

    }
    
    public double ChooseTrasholder()
    {

        List<double> nums = Call();
        HeapSort(nums);
        List<double> middlePoints = MiddlePoints(nums);
        
        if (middlePoints.Count == 0) return double.PositiveInfinity;
        ///////////////////////////////////////////////
        double gini=double.PositiveInfinity;

        double result = middlePoints[0];
       
        foreach(double middlePoint in middlePoints)
        {
            double giniPoint= CalculateGini(middlePoint);
            if (giniPoint < gini)
            {
                result = middlePoint;
                gini = giniPoint;
            }

        }
        return result;
    }


    

    public void HeapSort(List<double> numIstance)
    {
        MySorter sorter = new MySorter();
        numIstance.Sort(sorter);

    }


    public List<double> MiddlePoints(List<double> input)
    {
        List<double> result = new List<double>();
        double start = input[0];
        input.ForEach(x => { result.Add((x + start) / 2); start = x; });
        result.Remove(result[0]);
        return result;


    }


    public List<double> Call()
    {

        List<double> result = new List<double>();
        instances.ForEach(item => { if (examples.FindAll(x => x.Contains(item,myatt.Index)).Count > 0) result.Add(GetValue(item)); });


        return result;
    }

    
    public double GetValue(string vk)
    {
        //count++;
        //Debug.Log(vk+" "+count);
        //if (vk.Equals("1")) vk = "0.171";
        //return double.Parse(vk.Substring(0,vk.Length-myatt.AttributeName.Length), CultureInfo.InvariantCulture); 
        return double.Parse(vk, CultureInfo.InvariantCulture);
    }



    public double CalculateGini(double middlePoint)
    {

        double occurencePass;
        double occurenceNotPass;
        double total = examples.Count;
                
        double pi1=0.0;
        double pi2=0.0;
        double giniImpurity;
        double gini1=0.0;
        double gini2 = 0.0;
        double total_pass = examples.FindAll(x => x.PassTheTest(index, middlePoint)).Count;
        //double total_pass = examples.AsParallel().Count(x => x.PassTheTest(index, middlePoint));
        double total_notPass= examples.FindAll(x => !x.PassTheTest(index, middlePoint)).Count;
        //double total_notPass = examples.AsParallel().Count(x => !x.PassTheTest(index, middlePoint));
        foreach (string target in targetList)
        {
           
            occurencePass = examples.FindAll(x => x.PassTheTest(index, middlePoint) && x.ContainsTarget(target)).Count;
            //occurencePass = examples.AsParallel().Count(x => x.PassTheTest(index, middlePoint) && x.ContainsTarget(target));

            occurenceNotPass = examples.FindAll(x => (!x.PassTheTest(index, middlePoint)) && x.ContainsTarget(target)).Count;
            //occurenceNotPass = examples.AsParallel().Count(x => (!x.PassTheTest(index, middlePoint)) && x.ContainsTarget(target));

            // occurenceNotPass = examples.FindAll(x => (!x.PassTheTest(index, middlePoint)) && x.Contains(target)).Count;

            pi1 = occurencePass / total_pass;

                pi1 = pi1 * pi1;
                gini1 = gini1 + pi1;
            
            
                pi2 = occurenceNotPass / total_notPass;
                pi2 = pi2 * pi2;

                gini2 = gini2 + pi2;
            
        }
        gini1 = 1 - gini1;
        gini2 = 1 - gini2;
        giniImpurity = (total_pass / total)*gini1+(total_notPass/total)*gini2;
        return giniImpurity;
    }
    public double CalculateEntropy(double middlePoint)
    {

        double occurencePass=0.0;
        double occurenceNotPass=0.0;
        double total = examples.Count;

        double pi1;
        double pi2;
        double informationGain;
        double entropy1 = 0.0;
        double entropy2 = 0.0;
        double total_pass = examples.FindAll(x => x.PassTheTest(index, middlePoint)).Count;
        //double total_pass = examples.AsParallel().Count(x => x.PassTheTest(index, middlePoint));
        double total_notPass = examples.FindAll(x => !x.PassTheTest(index, middlePoint)).Count;
        //double total_notPass = examples.AsParallel().Count(x => !x.PassTheTest(index, middlePoint));
        foreach (string target in targetList)
        {
            
            occurencePass = examples.FindAll(x => x.PassTheTest(index, middlePoint) && x.ContainsTarget(target)).Count;
            //occurencePass = examples.AsParallel().Count(x => x.PassTheTest(index, middlePoint) && x.ContainsTarget(target));
            if (occurencePass != 0)
            {
                pi1 = occurencePass / total_pass;
                pi1 = pi1 * Math.Log(pi1, 2);
                entropy1 = entropy1 + pi1;
            }

            occurenceNotPass = examples.FindAll(x => (!x.PassTheTest(index, middlePoint)) && x.ContainsTarget(target)).Count;
           // occurenceNotPass = examples.AsParallel().Count(x => (!x.PassTheTest(index, middlePoint)) && x.ContainsTarget(target));
            if (occurenceNotPass != 0)
            {
                pi2 = occurenceNotPass / total_notPass;
                pi2 = pi2 * Math.Log(pi2, 2);
                entropy2 = entropy2 + pi2;
            }
        }
        double entropyParent = myatt.EntropyParent(examples, targetList);
        //Debug.Log("entropyParent: "+entropyParent);
        entropy1 =  - entropy1;
        entropy2 =  - entropy2;
        //Debug.Log(entropy1 + " " + entropy2);
        informationGain = entropyParent-((total_pass / total) * entropy1 + (total_notPass / total) * entropy2);
        //Debug.Log("informationGain: "+informationGain);
        return informationGain;
    }
    public double ChooseTrasholderEntropy()
    {
        List<double> nums = Call();
        HeapSort(nums);
        List<double> middlePoints = MiddlePoints(nums);
        
        if (middlePoints.Count == 0) return double.PositiveInfinity;
       
        double informationGain = double.NegativeInfinity;
        double result = middlePoints[0];
        
        foreach (double middlePoint in middlePoints)
        {
            double entropyPoint = CalculateEntropy(middlePoint);
            //Debug.Log(entropyPoint+" "+middlePoint);
            if (entropyPoint > informationGain)
            {
                result = middlePoint;
                informationGain = entropyPoint;
            }

        }
        Trashold = result;
        this.informationGain = informationGain;
        //myatt.Trashold = result;
        return result;
        return informationGain;
    }

}

public enum typeOfAttribute { Categorical, Continuos }; 

class MySorter : IComparer<double>
{
    public int Compare(double x, double y)
    {


        return x.CompareTo(y);

    }
}