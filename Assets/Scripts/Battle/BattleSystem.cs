using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
using PKMNGAMEUtils.StateMachine;


public enum BattleAction { Move,SwitchPokemon,UseItem,Run}

public enum BattleTrigger {LongGrass, Water }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
   
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveToForgetSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Audio")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;

    [Header("Images")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;

    public StateMachine<BattleSystem> StateMachine { get; private set; }

    public event Action<bool> OnBattleOver;

    public int SelectedMove { get; set; }

    public BattleAction SelectedAction { get; set; }

    public Pokemon SelectedPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }

    public bool IsBattleOver { get; private set; }


    public PokemonParty PlayerParty { get; private set; }
    public PokemonParty TrainerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    public bool IsTrainerBattle { get; private set; } = false;
    PlayerController player;
    public TrainerController Trainer { get; private set; }

    public int EscapeAttempts { get;  set; }

    BattleTrigger battleTrigger;

    public  void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        IsTrainerBattle = false;
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(wildBattleMusic);


        StartCoroutine(SetupBattle());    
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass )
    {
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;

        IsTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        Trainer = trainerParty.GetComponent<TrainerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(trainerBattleMusic);

        StartCoroutine(SetupBattle());

    }


    public IEnumerator SetupBattle()
    {
        StateMachine = new StateMachine<BattleSystem>(this);

        playerUnit.Clear();
        enemyUnit.Clear();
        //setting background image based on if its water battle or grass battle
        backgroundImage.sprite = (battleTrigger == BattleTrigger.LongGrass) ? grassBackground : waterBackground;

        if (!IsTrainerBattle)
        {
            //Wild pokemon battle
            playerUnit.Setup(PlayerParty.GetHealthyPokemon());
            enemyUnit.Setup(WildPokemon);

            dialogBox.SetMovesNames(playerUnit.Pokemon.Moves);
            yield return (dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared."));
        }
        else
        {
            //trainer battle

            //show sprites, first disable the pokemon, call in trainers, then make pokemon reappear
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = Trainer.Sprite;

            yield return dialogBox.TypeDialog($"{Trainer.Name} wants to battle");

            //send out first pokemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{Trainer.Name} sent out {enemyPokemon.Base.Name}");

            //send first pokemon for player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = PlayerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMovesNames(playerUnit.Pokemon.Moves);


        }
        IsBattleOver = false;
        EscapeAttempts = 0;
        partyScreen.Init();

        StateMachine.ChangeState(ActionSelectionState.i);
    }


     public void BattleOver(bool won)
    {
        IsBattleOver = true;
        PlayerParty.Pokemons.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);

    }

    public  void HandleUpdate()
    {
        StateMachine.Exectue();
    }

   public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            
            yield return dialogBox.TypeDialog($"Comeback {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMovesNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

    }
   public  IEnumerator SendNextTrainerPokemon()
    {

        var nextPokemon = TrainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{Trainer.Name} sent out {nextPokemon.Base.Name}!");
    }

    public IEnumerator ThrowPokeball(PokeballItems pokeballItems)
    {
        

        if (IsTrainerBattle)
        {
            yield return dialogBox.TypeDialog("Cmon big fella, don't be like Team rocket!");
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used {pokeballItems.Name.ToUpper()}!");

       var pokeballObj= Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItems.Icon;

        //implent animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
       yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon, pokeballItems);

        for (int i =0; i < Mathf.Min(shakeCount,3); ++i)
        {
          yield return new WaitForSeconds(0.5f);  
          yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            //caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught!");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            PlayerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party.");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            //not caught
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog("Almost caught it!");

            Destroy(pokeball);

        }
    }


    int TryToCatchPokemon(Pokemon pokemon, PokeballItems pokeballItems)
    {
        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate *  pokeballItems.CatchRateModifer * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHP);

        if (a >= 255)
            return 4;
        //pokemon will be caught

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680/ a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }
        return shakeCount;
    }

    public BattleDialogBox DialogBox => dialogBox;

    public BattleUnit PlayerUnit => playerUnit;

    public BattleUnit EnemyUnit => enemyUnit;

    public PartyScreen PartyScreen => partyScreen;

    public AudioClip BattleVictoryMusic => battleVictoryMusic;
}