using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    [SerializeField] private Text coverName;
    [SerializeField] private Text authoredBy;

    private bool isClicked = false;
    private Animator animator;
    private Image coverImage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        coverImage = GetComponent<Image>();
    }

    public void SetImage(Sprite sprite)
    {
        coverImage.sprite = sprite;
    }

    public void SetCoverName(string coverName)
    {
        this.coverName.text = coverName;
        this.authoredBy.text = "ilustração por:\n" + coverName;
    }

    public void OnClick()
    {
        if (transform.GetSiblingIndex() == GameController.GetCurrentIndex() && !isClicked)
        {
            GameController.ShowOverlay();
            GameController.SetCurrentClickedCover(this);
            PlayAnimation();
            isClicked = true;
        } else if (isClicked) {
            GameController.HideOverlay();
            GameController.SetCurrentClickedCover(null);
            Close();
            isClicked = false;
        }
    }

    public void PlayAnimation()
    {
        animator.SetBool("isClicked", true);
    }   

    public void Close()
    {
        animator.SetBool("isClicked", false);
    }

    public bool IsIdle()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("ZoomIn");
    }
}
