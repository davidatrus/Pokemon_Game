using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
  [SerializeField] new string name;

  [TextArea]
  [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;

    [SerializeField] List<Evolution> evolutions; 

    public static int MaxNumOfMoves { get; set; } = 4;

    public int GetExpForLevel(int level)
    {
        if(growthRate== GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level) / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            return GetFluctuating(level);
        }
        return -1;
    }

    public int GetFluctuating(int level)
    {
        if (level <= 15)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
        }
        else if (level >= 15 && level <= 36)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50));
        }
        else
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
        }
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public PokemonType Type1
    {
        get { return type1; }
    }
    public PokemonType Type2
    {
        get { return type2; }
    }
    public int MaxHP
    {
        get { return maxHP; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defence
    {
        get { return defence; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int SpDefence
    {
        get { return spDefence; }
    }
    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public List<MoveBase> LearnableByItems => learnableByItems;

    public List<Evolution> Evolutions => evolutions;

    public int CatchRate => catchRate;

    public int ExpYield => expYield;

    public GrowthRate GrowthRate => growthRate;



    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int level;

        public MoveBase Base
        {
            get { return moveBase; }
        }
        public int Level
        {
            get { return level; }
        }
    }

    [System.Serializable]
    public class Evolution
    {
        [SerializeField] PokemonBase evolvesInto;
        [SerializeField] int requiredLevel;
        [SerializeField] EvolutionItem requiredItem;

        public PokemonBase EvolvesInto => evolvesInto;
        public int RequiredLevel => requiredLevel;
        public EvolutionItem RequiredItem => requiredItem;
    }




    

    public class TypeChart
    {//2d array containing damage for each types. follows pokemon list given above.
        static float[][] chart =
        {
new float[] { 1f,  1f,   1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f, 0.5f, 0f,  1f,  1f, 0.5f },
new float[] { 1f, 0.5f, 0.5f, 1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f, 0.5f, 1f, 0.5f, 1f,  2f },
new float[] { 1f,  2f,  0.5f, 2f, 0.5f, 1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f, 0.5f, 1f,  1f },
new float[] { 1f,  1f,   2f, 0.5f,0.5f, 2f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f, 0.5f, 1f,  1f },
new float[] { 1f, 0.5f,  2f,  2f, 0.5f, 1f,  1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f,  1f, 0.5f, 1f, 0.5f },
new float[] { 1f, 0.5f, 0.5f, 1f,  2f, 0.5f, 1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f,  1f, 0.5f },
new float[] { 2f,  1f,   1f,  1f,  1f,  2f,  1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f,  2f,  2f },
new float[] { 1f,  1f,   1f,  1f,  2f,  1f,  1f, 0.5f, 0.5f, 1f, 1f,  1f, 0.5f, 0.5f, 1f, 1f,  0f },
new float[] { 1f,  2f,   1f,  2f, 0.5f, 1f,  1f,  2f,  1f,  0f,  1f, 0.5f, 2f,  1f,  1f,  1f,  2f },
new float[] { 1f,  1f,   1f, 0.5f, 2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f, 0.5f, 1f,  1f,  1f, 0.5f },
new float[] { 1f,  1f,   1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f, 0.5f, 1f,  1f,  1f,  1f,  0f, 0.5f },
new float[] { 1f, 0.5f,  1f,  1f,  2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f,  1f,  1f, 0.5f, 1f,  2f, 0.5f },
new float[] { 1f,  2f,   1f,  1f,  1f,  2f, 0.5f, 1f, 0.5f, 2f,  1f,  2f,  1f,  1f,  1f,  1f, 0.5f },
new float[] { 0f,  1f,   1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  2f, 1f, 0.5f, 0.5f },
new float[] { 1f,  1f,   1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  0.5f},
new float[] { 1f,  1f,   1f,  1f,  1f,  1f, 0.5f, 1f,  1f,  1f,  2f,  1f,  1f,  2f, 1f, 0.5f, 0.5f },
new float[] { 1f, 0.5f, 0.5f, 0.5f, 1f, 2f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  0.5f }

        };

        public static float GetEffectiveness(PokemonType attackType, PokemonType defenceType)
        {
            if (attackType == PokemonType.None || defenceType == PokemonType.None)
                return 1;
            int row = (int)attackType - 1;
            int col = (int)defenceType - 1;
            return chart[row][col];
        }

    }



}
public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Posion,
    Ground,
    Flying,
    Physic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel
}

public enum GrowthRate
{
    Fast, MediumFast, MediumSlow, Slow, Fluctuating
}

public enum Stat
{
    Attack,
    Defence,
    SpAttack,
    SpDefence,
    Speed,

    //not acc stats placeholders for boost moveAccurary.
    Accuracy,
    Evasion
}
