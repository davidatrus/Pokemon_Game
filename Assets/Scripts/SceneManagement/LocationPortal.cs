using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//switching player position w/o switching scenes. so use this whenever ur doing additive scene loading.
public class LocationPortal : MonoBehaviour, IPlayerTriggable
{

    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(TeleportPlayer());
    }
    public bool TriggerRepeatedly => false;

    Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator TeleportPlayer()
    {
        GameController.Instance.PausedGame(true);
        yield return fader.FadeIn(0.5f);


        var portalDes = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(portalDes.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
