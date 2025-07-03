using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMNGAMEUtils.StateMachine;
using System.Linq;

public class RunTurnState : State<BattleSystem>
{
    public static RunTurnState i { get; private set; }

    BattleSystem bs;
    private void Awake()
    {
        i = this;
    }

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    BattleDialogBox dialogBox;
    PartyScreen partyScreen;
    bool isTrainerBattle;
    PokemonParty playerParty;
    PokemonParty trainerParty;


    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        playerUnit = bs.PlayerUnit;
        enemyUnit = bs.EnemyUnit;
        dialogBox = bs.DialogBox;
        partyScreen = bs.PartyScreen;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;
        isTrainerBattle = bs.IsTrainerBattle;

        StartCoroutine(RunTurns(bs.SelectedAction));

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        base.Execute();
    }
    IEnumerator RunTurns(BattleAction playerAction)
    {
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[bs.SelectedMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            //now check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed > enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (bs.IsBattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                //second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (bs.IsBattleOver) yield break;
            }
        }
        else
        { 
            if (playerAction == BattleAction.SwitchPokemon)
            {
                yield return bs.SwitchPokemon(bs.SelectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                if(bs.SelectedItem is PokeballItems)
                {
                    //use pokeball
                   yield return bs.ThrowPokeball(bs.SelectedItem as PokeballItems);
                    if (bs.IsBattleOver) yield break;
                }
                else
                {
                    //do nothing and move to enemy turn
                }
      

            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }


            //after player switches pokemon should be enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (bs.IsBattleOver) yield break;
        }

        if (!bs.IsBattleOver)
            bs.StateMachine.ChangeState(ActionSelectionState.i);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //invoking onbeforeMove, checking if pokemon can use move, if it cant stop the run move func.
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);


        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {

            sourceUnit.PlayAttackAnimation();

            AudioManager.i.PlaySFX(move.Base.Sound);

            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            AudioManager.i.PlaySFX(AudioID.Hit);

            if (move.Base.Category == MoveCategory.Status)
            {

                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed.");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {   //stat boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }
        //status condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        //volatile status condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);

    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.IsBattleOver) yield break;
        

        //burn and posion will hurt pokemon after each turn 
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuarcy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];
        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuarcy *= boostValues[accuracy];
        else
            moveAccuarcy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuarcy /= boostValues[evasion];
        else
            moveAccuarcy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuarcy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }

    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyPokemon() == null;

            if (battleWon)
                AudioManager.i.PlayMusic(bs.BattleVictoryMusic);

            //gain exp
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} experience.");
            yield return playerUnit.Hud.SetExpSmooth();

            //check if player unit levels up

            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");


                //trying to learn a new move
                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.Pokemon.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}");
                        dialogBox.SetMovesNames(playerUnit.Pokemon.Moves);
                    }
                    //if = to 4 unit has to forget a move to learn new move
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is trying to learn {newMove.Base.Name}");
                        yield return dialogBox.TypeDialog($"But it can't learn more than {PokemonBase.MaxNumOfMoves} moves");
                        yield return dialogBox.TypeDialog($"Chose a move to forget!");

                        MoveToForgetState.i.CurrentMoves = playerUnit.Pokemon.Moves.Select(x => x.Base).ToList();
                        MoveToForgetState.i.NewMove = newMove.Base;
                        yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                        int moveIndex = MoveToForgetState.i.Selection;
                        if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                        {
                            //dont learn new move
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {newMove.Base.Name}.");
                        }
                        else
                        {
                            //learn the new move by forgeting selected move.
                            var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learned {newMove.Base.Name}.");

                            playerUnit.Pokemon.Moves[moveIndex] = new Move(newMove.Base);
                        }

                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }



            yield return new WaitForSeconds(1f);
        }

       yield return CheckForBattleOver(faintedUnit);
    }

    IEnumerator CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return bs.SwitchPokemon(PartyState.i.SelectedPokemon);
            }
        else
            bs.BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                bs.BattleOver(true);
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    AboutToUseState.i.NewPokemon = nextPokemon;
                   yield return bs.StateMachine.PushAndWait(AboutToUseState.i);
                }
                else
                    bs.BattleOver(true);
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A Critical hit!");

        //  if (damageDetails.TypeEffectiveness > 1f)
        // AudioManager.i.PlaySFX(AudioID.SuperHit);
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            // AudioManager.i.PlaySFX(AudioID.WeakHit);
            yield return dialogBox.TypeDialog("It's not very effective. ");
    }
    IEnumerator TryToEscape()
    {

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run away from trainer battles!");
 
            yield break;
        }

        ++bs.EscapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("You got away safely.");
            bs.BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * bs.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog("You got away safely.");
                bs.BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Can't Escape!");
            }
        }
    }
}
