using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState { FreeRoam,Battle,Dialog,Menu,PartyScreen,Bag, CutScene,Paused, Evolution,Shop}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController PlayerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

   // private BuddyController buddy;
    GameState state;

    GameState prevState;
    GameState stateBeforeEvolution;
    

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

   public static GameController Instance { get; private set;}
   //public BuddyController Buddy { get => buddy; set => buddy = value; }
    //public PlayerController PlayerController { get => PlayerController; set => PlayerController = value; }
   


    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        // Cursor.lockState = CursorLockMode.Locked; //lock the cursor.
       //  Cursor.visible = false; //hide the cursor

        PokemonDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        QuestDB.Init();
       // buddy = FindObjectOfType<BuddyController>().GetComponent<BuddyController>();
    }


    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;


        partyScreen.Init();

        DIalogManager.Instance.OnShowDialog += () =>
         {
             prevState = state;
             state = GameState.Dialog;
         };
       DIalogManager.Instance.OnDialogFinished += () =>
        {
            if(state== GameState.Dialog)
            state = prevState;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;

        EvolutionManager.i.OnStartEvolution += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };


        EvolutionManager.i.OnCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;

            AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade:true);

        };

        ShopController.i.OnStart += () => state = GameState.Shop;
        ShopController.i.OnFinish += () => state = GameState.FreeRoam;

    }

    public void PausedGame(bool paused)
    {
        if (paused)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

   public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = PlayerController.GetComponent < PokemonParty>();
        var wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty,wildPokemonCopy);
    }

    TrainerController trainer;

  public  void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);


        this.trainer = trainer;
        var playerParty = PlayerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();
       

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }
    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.CutScene;
        StartCoroutine(trainer.TriggerTrainerBattle(PlayerController));
    }

    void EndBattle(bool won)
    {
        if(trainer!=null&& won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();


        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

       var playerParty = PlayerController.GetComponent<PokemonParty>();
      bool hasEvolutions =  playerParty.CheckForEvoultions();

        if (hasEvolutions)
            StartCoroutine(playerParty.RunEvolutions());
        else
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic,fade:true);
    }
    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            PlayerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state== GameState.Dialog)
        {
            DIalogManager.Instance.HandleUpdate();
        }
        else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                // Pokemon stats screen
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected,onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        }
        /*if (Input.GetKeyDown(KeyCode.H))
            PlayerController.GetComponent<PokemonParty>().HealParty(); */
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }
    }

    public void SetCurrentScene(SceneDetails currscene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currscene;
    }

    void OnMenuSelected(int selectedItems)
    {
        if (selectedItems == 0)
        {
            //pokemon party selected

            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;

        }
        else if (selectedItems == 1)
        {
            //bag was selected
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;


        }
        else if (selectedItems == 2)
        {
            //saving was selected
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if(selectedItems==3)
        {
            //loading was selected
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut=false)
    {
        yield return Fader.i.FadeIn(0.5f);

        worldCamera.transform.position +=  new Vector3(moveOffset.x,moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);

        else
            StartCoroutine(Fader.i.FadeOut(0.5f));

    }

    public GameState State => state;
}

