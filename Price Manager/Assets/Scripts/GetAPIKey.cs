using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GetAPIKey : MonoBehaviour
{

    public static string GetKey()
    {
        if(File.Exists(Application.persistentDataPath + "/key.txt"))
        {
            return File.ReadAllText(Application.persistentDataPath + "/key.txt");
        }
        else
        {
            return string.Empty;
        }
    }

}
