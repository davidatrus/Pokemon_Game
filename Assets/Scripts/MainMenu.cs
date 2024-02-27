using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;

    public void Play()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void Load()
    {
        SceneManager.LoadScene("GamePlay");
        SavingSystem.i.Load("saveSlot1");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
