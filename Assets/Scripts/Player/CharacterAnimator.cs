using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> surfSprites;
    [SerializeField] FacingDirection defultDirection = FacingDirection.Down;


    //parameters
    public  float MoveX { get; set; }

 public float MoveY { get; set; }
 public bool IsMoving { get; set; }
    public bool IsSurfing { get; set; }
    public bool IsJumping { get; set; }

    //states

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;


    //references
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;
        //check to see if player is surfing or walking. if not surfing do normal walking animations
        if (!IsSurfing)
        {
            if (MoveX == 1)
                currentAnim = walkRightAnim;
            else if (MoveX == -1)
                currentAnim = walkLeftAnim;
            else if (MoveY == 1)
                currentAnim = walkUpAnim;
            else if (MoveY == -1)
                currentAnim = walkDownAnim;

            if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start();

            if (IsJumping)
                spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count - 1]; //setting jump animation to last frame

            else if (IsMoving)
                currentAnim.HandleUpdate();
            else
                spriteRenderer.sprite = currentAnim.Frames[0];
        }
        //we are surfing so use the sprites for those  
        else 
        {
            if (MoveX == 1)
                spriteRenderer.sprite = surfSprites[2];
            else if (MoveX == -1)
                spriteRenderer.sprite = surfSprites[3];
            else if (MoveY == 1)
                spriteRenderer.sprite = surfSprites[1];
            else if (MoveY == -1)
                spriteRenderer.sprite = surfSprites[0];
        }

        

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        MoveX = 0;
        MoveY = 0;

        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
    }

    public  FacingDirection DefaultDirection{
        get => defultDirection;
    }

}

public enum FacingDirection { Up,Down,Left,Right}