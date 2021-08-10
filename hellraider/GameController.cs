using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public static GameController controller;
    [HideInInspector]
    public PlayerData playerData;

    void Awake()
    {
        // If controller exists, delete. Otherwise, set current to singleton controller
        if (controller == null)
        {
            DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }

        this.Load(); // Load initial player data
    }

    // Load player data from binary file
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);
            playerData = (PlayerData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // Initialize empty data and save the first time
            playerData = new PlayerData();
            playerData.newPlayer = true;
            this.Save();
        }
    }

    // Save player data to binary file
    public void Save()
    {
        // Create file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerData.dat");
        // Serialize file
        bf.Serialize(file, playerData);
        file.Close();
    }

    // Helper method for loading a given scene
    public void LoadScene(string scene)
    {
        // Save current scene
        this.Save();
        // Load the next scene and close all others
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
