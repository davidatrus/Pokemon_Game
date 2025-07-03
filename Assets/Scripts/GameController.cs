using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PKMNGAMEUtils.StateMachine;


public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

   // private BuddyController buddy;


    public StateMachine<GameController> StateMachine { get; private set;}

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }


   public static GameController Instance { get; private set;}
   //public BuddyController Buddy { get => buddy; set => buddy = value; }
    //public PlayerController PlayerController { get => PlayerController; set => PlayerController = value; }
   


    private void Awake()
    {
        Instance = this;

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
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);

        battleSystem.OnBattleOver += EndBattle;


        partyScreen.Init();

        DIalogManager.Instance.OnShowDialog += () =>
         {
             StateMachine.Push(DialogState.i);
         };
       DIalogManager.Instance.OnDialogFinished += () =>
        {
            StateMachine.Pop();
        };


    }

    public void PausedGame(bool paused)
    {
        if (paused)
        {
            StateMachine.Push(PauseState.i);
        }
        else
        {
            StateMachine.Pop();
        }
    }


   public void StartBattle(BattleTrigger trigger) 
    {
        //battle trigger just checking if its water or grass we started battle from
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
       
    }

    TrainerController trainer;

  public  void StartTrainerBattle(TrainerController trainer)
    {
        BattleState.i.trainer = trainer;
        StateMachine.Push(BattleState.i);
    }
    public void OnEnterTrainersView(TrainerController trainer)
    {

        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if(trainer!=null&& won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

       var playerParty = playerController.GetComponent<PokemonParty>();
      bool hasEvolutions =  playerParty.CheckForEvoultions();

        if (hasEvolutions)
            StartCoroutine(playerParty.RunEvolutions());
        else
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic,fade:true);
    }
    private void Update()
    {
        StateMachine.Exectue(); 
        /*if (Input.GetKeyDown(KeyCode.H))
            PlayerController.GetComponent<PokemonParty>().HealParty(); */
    }

    public void SetCurrentScene(SceneDetails currscene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currscene;
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
    //visual indicator in game of statemachine and current states and whatever states get pushed & popped.
    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 22;

        GUILayout.Label("STATE STACK",style);

        foreach(var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style );
        }
    }

    public PlayerController PlayerController => playerController;
    public Camera WorldCamera => worldCamera;
    public PartyScreen PartyScreen => partyScreen;
}

