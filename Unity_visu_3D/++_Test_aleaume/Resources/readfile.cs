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

    string filetype;

    public readfile(string filename, string tag){
        this.filename = filename;
        this.filetype = tag;
    }
    public void read()
    {
        if(this.filetype == "walls")
        {
            ReadString();
            meanX = (minX + maxX) / 2;
            meanZ = (minZ + maxZ) / 2;
        }
    }

    void ReadString()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(filename);
        var fileContents = reader.ReadToEnd();
        reader.Close();

        // prend le texte du fichier
        var lines = fileContents.Split("\n"[0]);
        myarray = new float[lines.Length * 4];
        myarrayX = new float[lines.Length * 2];
        myarrayZ = new float[lines.Length * 2];
        int index = 0;

        // découpe le texte en lignes
        foreach(var line in lines)
        {
            // découpe les lignes en 4 nombres chacune
            var temps = line.Split(';');

            foreach(var temp in temps)
            {
                // ajoute chaque nombre dans l'array des valeurs
                myarray[index] = float.Parse(temp);
                if(index%2 == 0)
                {
                    // array des valeurs x
                    myarrayX[(int)(index / 2)] = float.Parse(temp);
                }
                else
                {
                    // array des valeurs z
                    myarrayZ[(int)(index / 2)] = float.Parse(temp);
                }
                index++;
            }

        }
        // trouve les valeurs min et max de x et z dans les valeurs données
        maxX = myarrayX.Max();
        minX = myarrayX.Min();

        maxZ = myarrayZ.Max();
        minZ = myarrayZ.Min();
    }

    void ReadPrefab()
    {

    }
}
