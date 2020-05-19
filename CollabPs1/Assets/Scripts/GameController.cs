using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private int coversVisible = 7;
    [SerializeField] private int defaultRotation = 50;
    [SerializeField] private int coverWidth = 200;

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject coverPrefab;

    [SerializeField] private float scrolledValueToChange = 0.02f;

    private float lastScrolledValueX = 0.5f;
    private static int currentMiddleCoverIndex;

    private int sideCoversCount;
    private List<string> files;

    private void Start()
    {
        sideCoversCount = (coversVisible - 1) / 2;

        string[] filesPng = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.png", SearchOption.TopDirectoryOnly);
        string[] filesJpg = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.jpg", SearchOption.TopDirectoryOnly);
        string[] filesJpeg = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.jpeg", SearchOption.TopDirectoryOnly);

        files = new List<string>();

        AddFilenames(filesPng);
        AddFilenames(filesJpg);
        AddFilenames(filesJpeg);

        files = files.OrderBy(f => Guid.NewGuid()).ToList();

        LoadFirstCovers();
    }

    private void AddFilenames(string[] arr)
    {
        foreach (string filePath in arr)
        {
            string filename = Path.GetFileName(filePath);

            string pattern = @"(.png|.PNG)|(.jpg|.JPG)|(.jpeg|.JPEG)";
            files.Add(Regex.Replace(filename, pattern, ""));
        }
    }

    private void LoadFirstCovers()
    {
        int halfFilesSize = files.Count / 2;
        currentMiddleCoverIndex = halfFilesSize;

        LoadLeftCovers(sideCoversCount, halfFilesSize - sideCoversCount);

        CreateCover(files[halfFilesSize], 0, coverWidth, 1000, files[halfFilesSize]);

        LoadRightCovers(sideCoversCount, halfFilesSize + 1);
    }

    private void LoadLeftCovers(int count, int startIndexFile)
    {
        CreateEmptyCovers(-defaultRotation);

        for (int i = 0; i < count; i++)
        {
            CreateCover(files[startIndexFile], -defaultRotation);

            startIndexFile++;
        }
    }

    private void LoadRightCovers(int count, int startIndexFile)
    {
        int zIndex = count;
        for (int i = 0; i < count; i++)
        {
            CreateCover(files[startIndexFile], defaultRotation, 0, zIndex);

            startIndexFile++;
            zIndex--;
        }

        CreateEmptyCovers(defaultRotation);
    }

    private void CreateEmptyCovers(int rotationY)
    {
        for (int i = 0; i < (files.Count - coversVisible) / 2; i++)
        {
            GameObject empty = Instantiate(coverPrefab, container.transform) as GameObject;
            empty.GetComponent<Image>().color = new Color(0, 0, 0, 0);

            RectTransform rectTransform = empty.GetComponent<RectTransform>();
            rectTransform.SetPositionAndRotation(rectTransform.position, Quaternion.Euler(0, rotationY, 0));
        }
    }

    private void CreateCover(string filename, int rotationY, int width = 0, int zIndex = 0, string coverName = null)
    {
        Sprite coverSprite = Resources.Load<Sprite>("Sprites/" + filename);

        GameObject cover = Instantiate(coverPrefab, container.transform) as GameObject;
        cover.gameObject.name = filename;
        cover.GetComponent<Image>().sprite = coverSprite;

        RectTransform rectTransform = cover.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(rectTransform.position, Quaternion.Euler(0, rotationY, 0));

        if (width > 0)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }

        if (zIndex > 0)
        {
            Canvas canvas = cover.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = zIndex;
        }

        if (coverName != null)
        {
            Cover coverObj = cover.GetComponent<Cover>();
            coverObj.SetCoverName(coverName);
        }
    }

    public void OnScrollChanged(Vector2 value)
    {
        Debug.Log(value.x);
        float valueAlreadyScrolled = lastScrolledValueX - value.x;
        if (value.x != lastScrolledValueX && Mathf.Abs(valueAlreadyScrolled) > scrolledValueToChange)
        {
            lastScrolledValueX = value.x;
            if (valueAlreadyScrolled > 0)
            {
                // left
                ChangeToPreviousCover();
                currentMiddleCoverIndex -= 1;

                LoadCover(currentMiddleCoverIndex - sideCoversCount);
            }
            else
            {
                // right
                ChangeToNextCover();
                currentMiddleCoverIndex += 1;

                LoadCover(currentMiddleCoverIndex + sideCoversCount, true);
            }
        }
    }

    private void ChangeToPreviousCover()
    {
        string previousFilename = files[currentMiddleCoverIndex - 1];
        string currentFilename = files[currentMiddleCoverIndex];

        GameObject previousCover = GameObject.Find(previousFilename);
        GameObject currentCover = GameObject.Find(currentFilename);
        GameObject nextCover = GameObject.Find(files[currentMiddleCoverIndex + 1]);

        previousCover.GetComponent<Cover>().SetCoverName(previousFilename);
        currentCover.GetComponent<Cover>().SetCoverName("");

        ChangeRotationAndOrder(currentCover, previousCover);
        ChangeRotationAndOrder(nextCover, currentCover, defaultRotation);

        UnloadCover(currentCover.transform.parent.GetChild(currentCover.transform.GetSiblingIndex() + sideCoversCount).gameObject);
    }

    private void ChangeToNextCover()
    {
        string currentFilename = files[currentMiddleCoverIndex];
        string nextFilename = files[currentMiddleCoverIndex + 1];

        GameObject previousCover = GameObject.Find(files[currentMiddleCoverIndex - 1]);
        GameObject currentCover = GameObject.Find(currentFilename);
        GameObject nextCover = GameObject.Find(nextFilename);

        currentCover.GetComponent<Cover>().SetCoverName("");
        nextCover.GetComponent<Cover>().SetCoverName(nextFilename);

        ChangeRotationAndOrder(currentCover, nextCover);
        ChangeRotationAndOrder(previousCover, currentCover, -defaultRotation);

        UnloadCover(currentCover.transform.parent.GetChild(currentCover.transform.GetSiblingIndex() - sideCoversCount).gameObject);
    }

    private void ChangeRotationAndOrder(GameObject coverOld, GameObject coverNew, float rotationY = 0)
    {
        RectTransform rectTransformOld = coverOld.GetComponent<RectTransform>();
        RectTransform rectTransformNew = coverNew.GetComponent<RectTransform>();

        rectTransformNew.SetPositionAndRotation(rectTransformNew.position, Quaternion.Euler(0, rotationY, 0));
        rectTransformNew.sizeDelta = rectTransformOld.sizeDelta;

        Canvas canvasOld = coverOld.GetComponent<Canvas>();
        Canvas canvasNew = coverNew.GetComponent<Canvas>();
        canvasNew.overrideSorting = canvasOld.overrideSorting;
        canvasNew.sortingOrder = canvasOld.sortingOrder;
    }

    private void UnloadCover(GameObject cover)
    {
        Image image = cover.GetComponent<Image>();
        image.sprite = null;
        image.color = new Color(0, 0, 0, 0);
    }

    private void LoadCover(int coverIndexToLoad, bool overrideSorting = false)
    {
        GameObject coverToLoad = container.transform.GetChild(coverIndexToLoad).gameObject;

        string filename = files[coverIndexToLoad];

        Sprite coverSprite = Resources.Load<Sprite>("Sprites/" + filename);

        coverToLoad.gameObject.name = filename;
        Image image = coverToLoad.GetComponent<Image>();
        image.sprite = coverSprite;
        image.color = new Color(1, 1, 1, 1);

        if (overrideSorting)
        {
            Canvas canvas = coverToLoad.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = -coverIndexToLoad;
        }
    }

    public static int GetCurrentIndex()
    {
        return currentMiddleCoverIndex;
    }
}
