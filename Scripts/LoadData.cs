using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData 
{

    private string document;
    private char separator;

    private int target;

    // si occupa di creare i campi Examples ed Attributes della classe DataSet
    public void BuildDataSet(List<Example>examples, List<Attribute> attributes, int numberOfAttributes)
    {
        setAttributes(attributes, numberOfAttributes);
        TextAsset data_csv = Resources.Load<TextAsset>(document);
        string[] data = data_csv.text.Split(new char[] { '\n' });
        
        for (int i = 0; i < data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { separator }); // ','
            
            for (int j = 0; j < attributes.Count; j++)
            {
                attributes[j].Add(row[j]);
            }
  
            Example example = new Example(row,target);
            examples.Add(example);
        }
        foreach(Attribute attr in attributes)
        {
            attr.SetType();
        }
    }

    // costruttore
    public LoadData(string document, char separator, int target)
    {
        this.document = document;
        this.separator = separator;
        this.target = target;
    }


    public void setAttributes(List<Attribute>attributes, int numberAttributes)
    {
        for(int i = 0; i < numberAttributes; i++)
        {
            attributes.Add(new Attribute(i));
        }
    }

}
