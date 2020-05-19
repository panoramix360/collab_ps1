using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    [SerializeField] private Text coverName;

    private Image coverImage;

    private void Start()
    {
        coverImage = GetComponent<Image>();
    }

    public void SetImage(Sprite sprite)
    {
        coverImage.sprite = sprite;
    }

    public void SetCoverName(string coverName)
    {
        this.coverName.text = coverName;
    }

    public void OnClick()
    {
        if (transform.GetSiblingIndex() == GameController.GetCurrentIndex())
        {
            Debug.Log("Curretn");
        }
    }
}
