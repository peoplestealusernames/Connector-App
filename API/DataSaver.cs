using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class DataSaver
{
    private string Loc;
    public DataSaver(string FileLocation)
    {
        this.Loc = FileLocation;
    }

    public void Save(string data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(this.Loc);
        bf.Serialize(file, data);
        file.Close();
    }
    public string Load()
    {
        if (File.Exists(this.Loc))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(this.Loc, FileMode.Open);
            string data = (string)bf.Deserialize(file);
            file.Close();
            return data;
        }
        else
            return "There is no save data!";
    }
}