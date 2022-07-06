using System.Collections;
using UnityEngine;
using System;
using System.IO;

public class SaveAndLoadSystem
{
    protected string Path;

    public SaveAndLoadSystem(string path)
    {
        Path = path;
    }

    public void Load(ref float[][] biases, ref float[][][] weights)
    {
        if(!File.Exists(Path)) return;

        TextReader tr = new StreamReader(Path);
        int NumberOfLines = (int)new FileInfo(Path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for(int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if(new FileInfo(Path).Length > 0)
        {
            for(int i = 0; i < biases.Length; i++)
            {
                for(int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for(int i = 0; i < weights.Length; i++)
            {
                for(int j = 0; j < weights[i].Length; j++)
                {
                    for(int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }


    public void Save(float[][] biases, float[][][] weights)
    {
        File.Create(Path).Close();
        StreamWriter writer = new StreamWriter(Path, true);

        for(int i = 0; i < biases.Length; i++)
        {
            for(int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}