using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveLoadGame
{
    public int HighestScore { get; set; }
    public float BestTime { get; set; }

    public SaveLoadGame() { }
    public void SaveGame()
    {
        try
        {
            //creating a BinaryFormatter object to serialize the file
            BinaryFormatter bf = new BinaryFormatter();
            //open file in write mode, all previous data will be deleted
            using (FileStream fStream = new FileStream(Application.persistentDataPath + "/save.dat", FileMode.Create, FileAccess.Write))
            {
                bf.Serialize(fStream, this);
                fStream.Close();
            }
            Debug.Log("Data saved!");
        }catch(Exception ex)
        {
            Debug.Log($"{ex.Message}");
        }
    }
    public bool LoadGame()
    {
        //if we have a file
        if(File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            try
            {
                //creating a BinaryFormatter object to deserialize the file
                BinaryFormatter bf = new BinaryFormatter();
                //open file in read mode
                using (FileStream fStream = new FileStream(Application.persistentDataPath + "/save.dat", FileMode.Open, FileAccess.Read))
                {
                    //Deserialize file and enter the data
                    SaveLoadGame sl = (SaveLoadGame)bf.Deserialize(fStream);
                    BestTime = sl.BestTime;
                    HighestScore = sl.HighestScore;
                    fStream.Close();
                }
                Debug.Log("Data loaded.");
                return true;
            }catch (Exception ex)
            {
                Debug.Log($"{ex.Message}");
            }
        }
        return false;
    }
}
