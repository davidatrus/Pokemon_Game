using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour,ISavable
{

    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

   

    private Vector2 input;
    public static PlayerController i { get; private set; }

    private Character character;



    private void Awake()
    { //assigning instance of playerController to be used in other places 
        i = this;
        character = GetComponent<Character>();
    }
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //removing diagonal movement
            if (input.x != 0) input.y = 0; 


            //checking for input
            if(input != Vector2.zero)
            {
               // GameController.Instance.Buddy.Follow(GameController.Instance.PlayerController.transform.position);

                StartCoroutine(character.Move(input, OnMoveOver));
            
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
           StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
       var interactPos= transform.position + facingDir;

        

        var collider= Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);
        if (collider != null)
        {
           yield return collider.GetComponent<Interactable>()?.Interact(transform);  
        }
    }

    IPlayerTriggable currentlyInTrigger;

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        IPlayerTriggable triggerable=null;

        foreach (var collider in colliders)
        {
           triggerable= collider.GetComponent<IPlayerTriggable>();
            if(triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }
        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            pokemons= GetComponent<PokemonParty>().Pokemons.Select(p=>p.GetSaveData()).ToList()
        };
     return saveData;
        //save //object can be used to return any other type, including classes
        //store the values of the transform.positions in a var because wont be able to acess them due to it not being a system serialiable but acc through unity
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
        //load position
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);


        //load pokemon party
       GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }

    public string Name
    {
        get => name;
    }
    public Sprite Sprite
    {
        get => sprite;
    }

    public Character Character => character;
}


[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemons;  

}