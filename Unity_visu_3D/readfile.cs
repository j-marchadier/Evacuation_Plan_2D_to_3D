using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class readfile
{

    //creates an array of arrays
    public float[] myarray;
    float[] myarrayX;
    float[] myarrayZ;
    public float maxX;
    public float minX;
    public float maxZ;
    public float minZ;

    public float meanX;
    public float meanZ;

    string filename;

    public readfile(string filename){
        this.filename = filename;
    }
    public void read()
    {
        ReadString();
        meanX = (minX + maxX) / 2;
        meanZ = (minZ + maxZ) / 2;

    }

    void ReadString()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(filename);
        var fileContents = reader.ReadToEnd();
        reader.Close();
        var lines = fileContents.Split("\n"[0]);
        myarray = new float[lines.Length * 4];
        myarrayX = new float[lines.Length * 2];
        myarrayZ = new float[lines.Length * 2];
        int index = 0;
        foreach(var line in lines)
        {
            var temps = line.Split(';');

            foreach(var temp in temps)
            {
                myarray[index] = float.Parse(temp);
                if(index%2 == 0)
                {
                    myarrayX[(int)(index / 2)] = float.Parse(temp);
                }
                else
                {
                    myarrayZ[(int)(index / 2)] = float.Parse(temp);
                }
                index++;
                Debug.Log(temp);
            }

        }

        maxX = myarrayX.Max();
        minX = myarrayX.Min();

        maxZ = myarrayZ.Max();
        minZ = myarrayZ.Min();

        //Debug.Log(myarray);
    }
}
