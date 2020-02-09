using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionMatrix 
{



    public ConfusionMatrix(List<string> predicted, List<string> expected, List<string> targetList)
    {
        Matrix = BuildConfusionMatrix(predicted, expected, targetList);
    }
    
    private double[,] matrix;

    public double[,] Matrix { get => matrix; set => matrix = value; }

    // matrice nXn che tiene conto di tutte le classificazione 

    public double Accuracy()
    {
        return DiagonalSum()/Total();
    }

    public double ErrorRate()
    {
        return 1 - Accuracy();
    }

    
    public double[,] BuildConfusionMatrix(List<string>predicted,List<string>expected, List<string>targetList)
    {
        //Debug.Log(targetList.Count);
        double[,] confusionMatrix=new double[targetList.Count,targetList.Count];
        
        for(int i = 0; i < targetList.Count; i++)
        {
            for(int j = 0; j < targetList.Count; j++)
            {
                //Debug.Log(expected[i]+" "+ predicted[i]);
                double result = MatchValues(predicted,expected, targetList[i], targetList[j]);
                confusionMatrix[i, j] = result;
            }
        }

        return confusionMatrix;
    }


    public double MatchValues(List<string>predicted,List<string>expected,string one, string two)
    {
        double result = 0.0;
        for(int i = 0; i < predicted.Count; i++)
        {
            if (expected[i].Equals(one) && predicted[i].Equals(two))
            {
                result++;
            }
        }
        return result;
    }

    
    public double DiagonalSum()
    {
        double result = 0.0;
        for(int i = 0; i<Matrix.GetLength(0); i++)
        {
                result += Matrix[i, i];
        }
        return result;
    }


    public double FalseSum()
    {
        double result = 0;
        for(int i = 0; i < Matrix.GetLength(0); i++)
        {
            for(int j = 0; j < Matrix.GetLength(1); j++)
            {
                if (i != j) result += Matrix[i, j];
            }
        }
        return result;
    }


    public double Total()
    {
        double result = 0;
        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                 result += Matrix[i, j];
            }
        }
        return result;
    }

    public void printMatrix(double[,]m)
    {
        string s = "";
        for(int i = 0; i<m.GetLength(0); i++)
        {
            for(int j = 0; j < m.GetLongLength(1); j++)
            {
                s += m[i, j]+" ";
              //  Debug.Log(m[i, j]);
            }
            Debug.Log(s);
            s = "";
        }
    }
}
