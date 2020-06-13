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
    [SerializeField] private GameObject overlay;

    [SerializeField] private float scrolledValueToChange = 0.02f;

    private float lastScrolledValueX = 0.5f;
    private static Cover currentClickedCover;
    private static int currentMiddleCoverIndex;
    private static GameObject overlayStatic;

    private int sideCoversCount;
    private List<string> files = new List<string>()
    {
        "@_fpjuni - Jackie Chan Stuntmaster",
        "@anactrln - Pink Panther Pinkadelic Pursuit",
        "@anarocha95 - Crash Bandicoot 3 Warped",
        "@andyargollo - Dragon Ball GT Final Bout",
        "@anselmecf - Yu Gi Oh Forbidden Memories_",
        "@atanqi - Grand Theft Auto 2",
        "@babzlullaby - Tomb Raider",
        "@betotallyart - Pepsiman",
        "@chatroleta - Digimon World 3",
        "@cynthtnyc - PaRappa the Rapper",
        "@danieldantedev - Sillent Hill",
        "@DexDrakon - The Legend of Dragoon",
        "@emanuel_rochart - Spyro",
        "@Ezequimel - Test drive 6",
        "@fitsfito - BloodyRoar II",
        "@gabirotcho - SpiderMan",
        "@giancarlozero - Final Fantasy VIII",
        "@glaubits - Tobal No.1",
        "@glaucio_sd - Final Fantasy Tactics",
        "@hafu_ilustra - Peter Pan in Return to Neverland",
        "@henryp39 - Darkstalkers",
        "@HugoOak - Megaman X5",
        "@jardellucas_art - Tony Hawk_s Pro Skater 4",
        "@javanzord - Marvel vs Capcom",
        "@jefferson83r - Chrono Cross",
        "@Kisler_Art - Megaman x6",
        "@krolkushi - Bust a Groove",
        "@lailartsy - Tony Hawk_s Pro Skater",
        "@lamascaradibuja - Capcom vs snk pro",
        "@leobergamini - Ms. PacMan Maze Madness",
        "@lipejcorrea - Klonoa  Door to Phantomile",
        "@LUFEdraws - Winning Eleven 4",
        "@meua_migo - Bomberman Fantasy Race",
        "@mrhenrik_art - Resident Evil 3 Nemesis",
        "@mthsmnds - Crash Bandicoot",
        "@nicolas_almor - TEKKEN 3",
        "@oivinnie - Tomb Raider Chronicles",
        "@paulomoreria - Crash Team Racing",
        "@pedrolheiro - Final Fantasy IX_",
        "@radice - Harry Potter and The Philosopher_s Stone",
        "@rafaelvic - Megaman X4",
        "@rodrigopims - Dino Crisis",
        "@ryansmallman - CROC  Legend of the Gobbos",
        "@Sad_e_Lonely - Diablo",
        "@saruzilla - Breath of Fire IV",
        "@senhorluluba - Twisted Metal",
        "@silvazuao - Metal Slug X",
        "@soliva_joao - Brave Fencer Musashi",
        "@tinzin_fofin - SPYRO Ripto_s Rage",
        "@tosobreojardim - Medal of honor",
        "@tucaas - Digimon World",
        "@turboelson - Crash Bandicoot 2 Cortex Strikes Back",
        "@VeridisCouraged - Final Fantasy VII",
        "@victtorcom2t - 3Xtreme",
        "@viniseeowls - Looney Tunes Sheep Raider",
        "@vito_alvess - Soul Reaver"
    };

    private void Start()
    {
        overlayStatic = overlay;
        sideCoversCount = (coversVisible - 1) / 2;

        //string[] filesPng = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.png", SearchOption.TopDirectoryOnly);
        //string[] filesJpg = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.jpg", SearchOption.TopDirectoryOnly);
        //string[] filesJpeg = Directory.GetFiles(Application.dataPath + "/Resources/Sprites", "*.jpeg", SearchOption.TopDirectoryOnly);

        //files = new List<string>();

        //AddFilenames(filesPng);
        //AddFilenames(filesJpg);
        //AddFilenames(filesJpeg);

        files = files.OrderBy(f => Guid.NewGuid()).ToList();

        LoadFirstCovers();
    }

    public bool IsCoverClicked()
    {
        return currentClickedCover != null;
    }

    public void ClickOnCurrentCover()
    {
        Cover cover = container.transform.GetChild(currentMiddleCoverIndex).GetComponent<Cover>();
        if (cover.IsIdle())
        {
            cover.OnClick();
        }
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
        currentMiddleCoverIndex = halfFilesSize - 1;

        LoadLeftCovers();

        CreateCover(files[currentMiddleCoverIndex], 0, coverWidth, 1000, files[currentMiddleCoverIndex]);

        LoadRightCovers();
    }

    private void LoadLeftCovers()
    {
        int coversToCreate = (files.Count - coversVisible) / 2;
        CreateEmptyCovers(coversToCreate, -defaultRotation);

        int startIndexFile = currentMiddleCoverIndex - sideCoversCount;
        for (int i = 0; i < sideCoversCount; i++)
        {
            CreateCover(files[startIndexFile], -defaultRotation);

            startIndexFile++;
        }
    }

    private void LoadRightCovers()
    {
        int startIndexFile = currentMiddleCoverIndex + 1;
        for (int i = 0; i < sideCoversCount; i++)
        {
            CreateCover(files[startIndexFile], defaultRotation, 0, -startIndexFile);

            startIndexFile++;
        }

        int coversToCreate = (files.Count - coversVisible) / 2;
        CreateEmptyCovers(coversToCreate, defaultRotation);

        int coversRemaining = (files.Count - coversVisible) % 2;
        CreateEmptyCovers(coversRemaining, defaultRotation);
    }

    private void CreateEmptyCovers(int coversToCreate, int rotationY)
    {
        for (int i = 0; i < coversToCreate; i++)
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

        if (zIndex != 0)
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

    public void LoadLeft()
    {
        // left
        ChangeToPreviousCover();
        currentMiddleCoverIndex -= 1;

        LoadCover(currentMiddleCoverIndex - sideCoversCount);
    }

    public void LoadRight()
    {
        // right
        ChangeToNextCover();
        currentMiddleCoverIndex += 1;

        LoadCover(currentMiddleCoverIndex + sideCoversCount, true);
    }

    private void ChangeToPreviousCover()
    {
        int previousIndex = currentMiddleCoverIndex - 1;
        int nextIndex = currentMiddleCoverIndex + 1;

        previousIndex = Mathf.Max(previousIndex, 0);
        nextIndex = Mathf.Min(nextIndex, files.Count - 1);

        string previousFilename = files[previousIndex];
        string currentFilename = files[currentMiddleCoverIndex];

        GameObject previousCover = GameObject.Find(previousFilename);
        GameObject currentCover = GameObject.Find(currentFilename);
        GameObject nextCover = GameObject.Find(files[nextIndex]);
            
        previousCover.GetComponent<Cover>().SetCoverName(previousFilename);
        currentCover.GetComponent<Cover>().SetCoverName("");

        ChangeRotationAndOrder(currentCover, previousCover, true);
        ChangeRotationAndOrder(nextCover, currentCover, true, defaultRotation, -currentMiddleCoverIndex);

        if (currentMiddleCoverIndex + sideCoversCount <= files.Count - 1)
        {
            UnloadCover(currentCover.transform.parent.GetChild(currentCover.transform.GetSiblingIndex() + sideCoversCount).gameObject);
        }
    }

    private void ChangeToNextCover()
    {
        int previousIndex = currentMiddleCoverIndex - 1;
        int nextIndex = currentMiddleCoverIndex + 1;

        previousIndex = Mathf.Max(previousIndex, 0);
        nextIndex = Mathf.Min(nextIndex, files.Count - 1);

        string currentFilename = files[currentMiddleCoverIndex];
        string nextFilename = files[nextIndex];

        GameObject previousCover = GameObject.Find(files[previousIndex]);
        GameObject currentCover = GameObject.Find(currentFilename);
        GameObject nextCover = GameObject.Find(nextFilename);
        
        currentCover.GetComponent<Cover>().SetCoverName("");
        nextCover.GetComponent<Cover>().SetCoverName(nextFilename);

        ChangeRotationAndOrder(currentCover, nextCover, true);
        ChangeRotationAndOrder(previousCover, currentCover, false, -defaultRotation);
        
        if (currentMiddleCoverIndex - sideCoversCount >= 0)
        {
            UnloadCover(currentCover.transform.parent.GetChild(currentCover.transform.GetSiblingIndex() - sideCoversCount).gameObject);
        }
    }

    private void ChangeRotationAndOrder(GameObject coverOld, GameObject coverNew, bool overrideSorting, float rotationY = 0, int sortingOrder = 0)
    {
        RectTransform rectTransformOld = coverOld.GetComponent<RectTransform>();
        RectTransform rectTransformNew = coverNew.GetComponent<RectTransform>();

        rectTransformNew.SetPositionAndRotation(rectTransformNew.position, Quaternion.Euler(0, rotationY, 0));
        rectTransformNew.sizeDelta = rectTransformOld.sizeDelta;

        Canvas canvasOld = coverOld.GetComponent<Canvas>();
        Canvas canvasNew = coverNew.GetComponent<Canvas>();
        canvasNew.overrideSorting = overrideSorting;
        if (sortingOrder != 0)
        {
            canvasNew.sortingOrder = sortingOrder;
        }
        else
        {
            canvasNew.sortingOrder = canvasOld.sortingOrder;
        }
        
    }

    private void UnloadCover(GameObject cover)
    {
        Image image = cover.GetComponent<Image>();
        image.sprite = null;
        image.color = new Color(0, 0, 0, 0);
    }

    private void LoadCover(int coverIndexToLoad, bool overrideSorting = false)
    {
        if (coverIndexToLoad < 0) return;
        if (coverIndexToLoad > files.Count - 1) return;
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

    public static void ShowOverlay()
    {
        overlayStatic.gameObject.SetActive(true);
    }

    public static void HideOverlay()
    {
        overlayStatic.gameObject.SetActive(false);
    }

    public static void SetCurrentClickedCover(Cover cover)
    {
        currentClickedCover = cover;
    }

    public void OverlayClicked()
    {
        overlayStatic.gameObject.SetActive(false);
        currentClickedCover.Close();
        currentClickedCover = null;
    }
}
