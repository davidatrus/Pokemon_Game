 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    [SerializeField] AudioClip sceneMusic;

    public bool IsLoaded { get; private set;  }
   List<SavableEntity> savableEntities;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            //play music of scene once since is loaded
            if(sceneMusic!=null)
                AudioManager.i.PlayMusic(sceneMusic, fade:true);

            //load all connected scenes.

            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            //unload scenes
           var prevScene = GameController.Instance.PrevScene;
            if (prevScene != null)
            {
                var previouslyLoadedScenes = prevScene.connectedScenes;
                foreach(var scene in previouslyLoadedScenes)
                {
                    //if not currently loaded scene and  is not in the connected scene of currently loaded scene unload that scene.
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

             if(!connectedScenes.Contains(prevScene)) //line 42&43 is error in the code
                prevScene.UnloadScene();
            }
        }
    }

    

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

           operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };  
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
           SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        } 
    }

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();
        return savableEntities;
    }

    public AudioClip SceneMusic => sceneMusic;
}
