using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class readfile
{
    public float[] myarray;
    // creates a list of x and y coordinates (mostly unused)
    public float[] myarrayX;
    // list of x coordinates
    public float[] myarrayZ;
    // list of associated z coordinates
    public float maxX;
    // maximum x value
    public float minX;
    // minimum x value

    public float maxZ;
    // maximum z value

    public float minZ;
    // minimum z value

    public float meanX;
    // mean x value

    public float meanZ;
    // mean z value

    string filename;
    // name of the read file

    string filetype;
    // type of the file read (walls, items, doors)


    public readfile(string filename, string tag)
    // read a file
    {
        this.filename = filename; // register file name
        this.filetype = tag; // register file type
    }
    public void read()
    // read the file content depending on type
    {
        if (this.filetype == "walls")
        {
            Read_walls();
            meanX = (minX + maxX) / 2;
            meanZ = (minZ + maxZ) / 2;
            // define the mean values of x and z from min and max values

        }

        else if (this.filetype == "items")
        {
            Read_items();
        }

        else if (this.filetype == "doors")
        {
            Read_doors();
        }
    }

    void Read_walls()
    // read a "walls" file
    {
        //Read the text from directly from the file
        StreamReader reader = new StreamReader(filename);
        var fileContents = reader.ReadToEnd();
        reader.Close(); // register the content and close the file

        var lines = fileContents.Split("\n"[0]);
        myarray = new float[lines.Length * 4];
        myarrayX = new float[lines.Length * 2];
        myarrayZ = new float[lines.Length * 2];
        int index = 0;

        // cut the file text in lines
        foreach (var line in lines)
        {
            // cut each line in 4 parts (x1 z1 x2 z2)
            var temps = line.Split(';');

            foreach (var temp in temps)
            {
                // add each number in the value array in order (x then z in sequence)
                myarray[index] = float.Parse(temp);
                if (index % 2 == 0)
                {
                    // add the x values in the x array
                    myarrayX[(int)(index / 2)] = float.Parse(temp);
                }
                else
                {
                    // add the z values in the z array
                    myarrayZ[(int)(index / 2)] = float.Parse(temp);
                }
                index++;
            }

        }
        // search for the maximum and minimum values of x and z
        maxX = myarrayX.Max();
        minX = myarrayX.Min();

        maxZ = myarrayZ.Max();
        minZ = myarrayZ.Min();
    }

    void Read_items()
    // read a "items" file
    {
        ;
    }

    void Read_doors()
    // read a "doors" file
    {
        ;
    }
}
