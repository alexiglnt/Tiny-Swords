using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : MonoBehaviour
{
    public static PawnController selectedPawn = null;

    public Animator animator;
    private Controls controls;
    private TreeController currentTargetTree;
    private SheepController currentTargetSheep;
    public AudioSource audioSource;
    public AudioClip chopSound;
    public AudioClip killSound;

    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private bool isSelected = false;
    private void Awake()
    {
        controls = new Controls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.SelectPawn.performed += ctx => HandleSelection();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        controls.Gameplay.SelectPawn.performed -= ctx => HandleSelection();
    }

    private void UpdateSelectionVisual(bool isSelected)
    {
        spriteRenderer.color = isSelected ? selectedColor : normalColor;
    }

    void HandleSelection()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool isPawnClicked = ColliderIsPawn(hit);
        
        if (isPawnClicked && !IsBusy())
        {
            TogglePawnSelection();
        }
        else if (selectedPawn != null)
        {
            // Check if a tree is clicked
            if (hit.collider != null && hit.collider.GetComponent<TreeController>() != null)
            {
                selectedPawn.StartAction(hit.collider.GetComponent<TreeController>());
            }
            // Check if a sheep is clicked
            else if (hit.collider != null && hit.collider.GetComponent<SheepController>() != null)
            {
                selectedPawn.StartAction(hit.collider.GetComponent<SheepController>());
            }
        }
    }

    private bool ColliderIsPawn(RaycastHit2D hit)
    {
        Collider2D collider = GetComponent<Collider2D>();
        return collider != null && collider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void TogglePawnSelection()
    {
        if (selectedPawn != null && selectedPawn != this)
        {
            selectedPawn.DeselectPawn();
        }

        isSelected = !isSelected;
        selectedPawn = isSelected ? this : null;
        UpdateSelectionVisual(isSelected);
        Debug.Log(isSelected ? "Pawn selected" : "Pawn deselected");
    }

    private void DeselectPawn()
    {
        isSelected = false;
        UpdateSelectionVisual(false);
    }

    public void StartChopping(TreeController tree)
    {
        currentTargetTree = tree;
        animator.Play("Chop");

        // Deselect the pawn and update visuals
        isSelected = false;
        UpdateSelectionVisual(isSelected);
        selectedPawn = null; // Clear the static reference to this pawn

        Debug.Log("Pawn starts chopping and is deselected");
    }

    public void StartButchering(SheepController sheep)
    {
        currentTargetSheep = sheep;
        animator.Play("Kill");

        // Deselect the pawn and update visuals
        isSelected = false;
        UpdateSelectionVisual(isSelected);
        selectedPawn = null; // Clear the static reference to this pawn

        Debug.Log("Pawn starts butchering and is deselected");
    }

    public void OnAnimationHit()
    {
        if (currentTargetTree != null && currentTargetTree.IsAlive)
        {
            currentTargetTree.ReceiveDamage();
            PlayChopSound();
        }
        else if (currentTargetSheep != null && currentTargetSheep.IsAlive)
        {
            currentTargetSheep.ReceiveDamage();
            PlayKillSound();
        }
        else
        {
            StopAction();
        }
    }

    private void PlayChopSound()
    {
        if (audioSource != null && chopSound != null)
        {
            audioSource.PlayOneShot(chopSound);
        }
    }

    private void PlayKillSound()
    {
        if (audioSource != null && killSound != null)
        {
            audioSource.PlayOneShot(killSound);
        }
    }

    public void StartAction(object target)
    {
        if (target is TreeController tree)
        {
            StartChopping(tree);
        }
        else if (target is SheepController sheep)
        {
            StartButchering(sheep); // Reuse chopping action for sheep
        }
    }

    private bool IsBusy()
    {
        return currentTargetTree != null;
    }

    //used in the animation event of the chop animation to check if the tree is still alive
    private void CheckTreeAlive()
    {
        if (currentTargetTree != null && !currentTargetTree.IsAlive)
        {
            StopAction();
        }
    }    
    
    private void CheckSheepAlive()
    {
        if (currentTargetSheep != null && !currentTargetSheep.IsAlive)
        {
            StopAction();
        }
    }

    private void StopAction()
    {
        animator.Play("Idle");
        currentTargetTree = null;
        currentTargetSheep = null;
    }
}
