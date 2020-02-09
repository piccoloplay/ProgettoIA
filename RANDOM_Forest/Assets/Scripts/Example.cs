using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Example
{
    public int indexTarget;
    public static int count = -1;
    private int id;

    public string getTarget(int target)
    {
        return Record[target];
    }

    public Example(string[] row, int target)
    {
        indexTarget = target;
        count++;
        id = count;
        Record = new List<string>();
        int i = 0;
        foreach(string s in row)
        {
            string s1 = s;
            Record.Add(s1);
            i++;
        }
    }

    private List<string> record;

    public List<string> Record { get => record; set => record = value; }
    public int Id { get => id; set => id = value; }

    public bool Contains(string s, int index)
    {
        return record[index].Equals(s);
    }


    public bool ContainsTarget(string s)
    {
       // return Record.Contains(s);
        return record[indexTarget].Equals(s);
    }


    public void Remove(string s)
    {
        Record.Remove(s);
      

    }
    
    public bool PassTheTest(string attribute, double test)
    {
        int attributeL = attribute.Length;
        int index = int.Parse(attribute);
        string s = record[index];
        double num;
        if (!double.TryParse(s, out num)) Debug.Log(s);
        if (double.Parse(s, CultureInfo.InvariantCulture) > test) return true;
        return false;
    }


    public void Replace(string s, int index)
    {
        //Debug.Log(record[index]);
        record[index] = s;
        //Debug.Log(record[index]);
    }


    public void printRecord()
    {
        string result = "";
        foreach(string s in record)
        {
            result += s + " | ";
        }
        Debug.Log(result);
        
    }


}


