using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTriggable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    public bool TriggerRepeatedly => false;

    Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PausedGame(true);
        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

       var portalDes=  FindObjectsOfType<Portal>().First(x => x!=this && x.destinationPortal==this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(portalDes.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);

        Destroy(gameObject);


    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier { A,B,C,D,E}
