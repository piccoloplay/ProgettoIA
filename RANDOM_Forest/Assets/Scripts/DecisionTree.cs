using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class DecisionTree {


    public static int unseenValues=0;

    public DecisionTree(string AttributeName)
    {
        Nodes = new Dictionary<string, DecisionTree>();
        this.test = AttributeName;
    }

    public void AddNode(string a, DecisionTree t)
    {
        Nodes[a] = t;
    }

    private string test;

    private Dictionary<string, DecisionTree> nodes;

    public string Test { get => test; set => test = value; }
    public Dictionary<string, DecisionTree> Nodes { get => nodes; set => nodes = value; }

    public string Classify(Example example)
    {
        DecisionTree nextNode = this;
        List<string> record = example.Record;

        if (!SettingGui.oneHot)
        {
            nextNode = NextNode(nextNode, record);
        }
        else
        {
            nextNode = NextNodeWithOneHotEncode(nextNode, record);
        }
       
        return nextNode.Test;
    }


    private DecisionTree NextNodeWithOneHotEncode(DecisionTree nextNode, List<string>record)
    {
        while (!nextNode.IsLeaf())
        {
            int index = int.Parse(nextNode.test);
            string element = record[index];


            if (nextNode.nodes.ContainsKey(element))
            {
                nextNode = nextNode.nodes[element];
            }
            else
            {
                if (nextNode.nodes.Count == 1)
                {
                    unseenValues++;
                    // return "Unable to Classify";
                    string[] keys = nextNode.nodes.Keys.ToArray();
                    element = keys[0];
                    nextNode = nextNode.Nodes[element];
                    continue;
                }
                else
                {
                    string temp = nextNode.test;

                    element = nextNode.ChooseLink(element);
                    nextNode = nextNode.nodes[element];
                }

            }
        }   

            return nextNode; 
    }

    private DecisionTree NextNode(DecisionTree nextNode, List<string>record)
    {
        while (!nextNode.IsLeaf())
        {
            int index = int.Parse(nextNode.test);
            string element = record[index];

            double parse = 0;
            if (double.TryParse(element, out parse))
            {
                element = nextNode.ChooseLink(element);
            }
            if (nextNode.Nodes.ContainsKey(element))
            { nextNode = nextNode.Nodes[element]; }
            else
            {
                unseenValues++;
                //  return "Unable to Classify";
                string[] keys = nextNode.nodes.Keys.ToArray();
                element = keys[0];
                nextNode = nextNode.Nodes[element];
            }
        }
            return nextNode;
    }
    

    public bool IsLeaf()
    {
        return Nodes.Count == 0;

    }

    public string ChooseLink(string s)
    {
        
        var k = nodes.Keys.ToArray();
        //double lengthP=attributeName.Length;
        if (PassTheTest(s, k[0])) return k[0];
        return k[1];
        
    }


    public bool PassTheTest(string feature, string testString)
    {

        //Debug.Log(feature +" "+ testString);
        double num = double.Parse(feature, CultureInfo.InvariantCulture);
        
        double test = double.Parse(testString.Substring(1, testString.Length-1));// leva il maggiore 
        
        return num>test;
    }



    //UTILITIES
    public void printNodes(DecisionTree node)
    {
        string s = "";
        foreach (KeyValuePair<string, DecisionTree> p in node.Nodes)
        {
            s += p.Key + " ";
        }
        Debug.Log(s);
    }


    public void printRecord(List<string>record)
    {
        string res = "Record: ";
        foreach(string s in record)
        {
            res += s + " ";
        }
        Debug.Log(res);
    }





}