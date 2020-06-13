using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private float stepValue = 0.02f;

    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {

        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);
        bool enter = Input.GetKeyDown(KeyCode.KeypadEnter);
        bool space = Input.GetKeyDown(KeyCode.Space);

        if (enter || space)
        {
            gameController.ClickOnCurrentCover();
        }

        if (gameController.IsCoverClicked()) return;

        GoToLeft(left);

        GoToRight(right);
    }

    public void GoToLeft(bool left)
    {
        if (left && scrollRect.horizontalNormalizedPosition > -0.04)
        {
            scrollRect.horizontalNormalizedPosition -= stepValue;
            gameController.LoadLeft();
        }
    }

    public void GoToRight(bool right)
    {
        if (right && scrollRect.horizontalNormalizedPosition < 1.06)
        {
            scrollRect.horizontalNormalizedPosition += stepValue;
            gameController.LoadRight();
        }
    }
}
