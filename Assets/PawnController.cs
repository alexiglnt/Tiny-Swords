using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : MonoBehaviour
{
    public static PawnController selectedPawn = null;

    public Animator animator;
    private Controls controls;
    private TreeController currentTargetTree;
    public AudioSource audioSource;
    public AudioClip chopSound;

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

        if (isPawnClicked && !IsChopping())
        {
            TogglePawnSelection();
        }
        else if (selectedPawn != null && hit.collider != null && hit.collider.GetComponent<TreeController>() != null)
        {
            selectedPawn.StartChopping(hit.collider.GetComponent<TreeController>());
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

    public void OnChopAnimationHit()
    {
        if (currentTargetTree != null && currentTargetTree.IsAlive)
        {
            currentTargetTree.ReceiveDamage();
            PlayChopSound();
        }
        else
        {
            StopChopping();
        }
    }

    private void PlayChopSound()
    {
        if (audioSource != null && chopSound != null)
        {
            audioSource.PlayOneShot(chopSound);
        }
    }

    private void StopChopping()
    {
        animator.Play("Idle");
        currentTargetTree = null;
    }

    private bool IsChopping()
    {
        return currentTargetTree != null;
    }

    //used in the animation event of the chop animation to check if the tree is still alive
    private void CheckTreeAlive()
    {
        if (currentTargetTree != null && !currentTargetTree.IsAlive)
        {
            StopChopping();
        }
    }
}
