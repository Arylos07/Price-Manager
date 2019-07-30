using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public string storeAdminURL = "https://the-underground-games.myshopify.com/admin";

    public void Quit()
    {
        Application.Quit();
    }

    public void StoreAdmin()
    {
        Application.OpenURL(storeAdminURL);   //URL subject to change
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
