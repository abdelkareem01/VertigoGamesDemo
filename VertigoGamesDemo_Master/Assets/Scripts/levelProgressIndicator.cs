using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class levelProgressIndicator : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollBar;

    [SerializeField]
    private GameObject elementsParent;

    [SerializeField]
    private GameObject elementCell;

    public List<Sprite> cellSprites = new List<Sprite>();

    [HideInInspector]
    public List<GameObject> spawnedCells = new List<GameObject>();

    private bool scroll = false;

    private float nextX, scrollOffset;

    private int lastIndex = 1;

    void Awake()
    {
        setScrollOffset();
    }

    void Start()
    {
        scrollBar.enabled = false;
        for (int i = 0; i < 5; i++) spawnCell();
    }

    void Update()
    {
        if (scroll)
        {
           elementsParent.transform.position = Vector3.Lerp(new Vector3(elementsParent.transform.position.x, elementsParent.transform.position.y, elementsParent.transform.position.z),
                                                new Vector3(nextX - (scrollOffset), elementsParent.transform.position.y, elementsParent.transform.position.z), 2f * Time.deltaTime);

           
        }

    }

    public IEnumerator moveNext() {
        yield return new WaitForSeconds(1f);
        this.transform.GetChild(0).GetComponent<Image>().DOFade(0, 1f);
        nextX = elementsParent.transform.position.x;
        scroll = true;
        yield return new WaitForSeconds(3f);
        this.transform.GetChild(0).GetComponent<Image>().DOFade(0.5f, 0.5f);
        scroll = false;
        yield return new WaitForSeconds(1f);
        spawnCell();
    }

    public void spawnCell() {

        GameObject cell = Instantiate(elementCell, elementsParent.transform.position, Quaternion.identity, elementsParent.transform);

        if ((lastIndex % 5 == 0 && lastIndex % 30 != 0))
        {

            cell.GetComponent<Image>().sprite = cellSprites[1];
        }

        else if (lastIndex % 30 == 0)
        {

            cell.GetComponent<Image>().sprite = cellSprites[2];
        }
        else if(lastIndex == 1)
        {
            cell.GetComponent<Image>().sprite = cellSprites[2];
        }
        else {

            cell.GetComponent<Image>().sprite = cellSprites[0];
        }

        cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lastIndex < 10 ? " " + lastIndex + "" : lastIndex + "";

        lastIndex++;
        spawnedCells.Add(cell);

    }


    public void setScrollOffset() {

        string res = Display.main.systemWidth.ToString() + " " + Display.main.systemHeight.ToString();
        if (res.Contains("1280") && res.Contains("720"))
        {
            scrollOffset = 81.5f * this.transform.localScale.x;
        }

        else if (res.Contains("1920") && res.Contains("1080"))
        {
            scrollOffset = 122.5f * this.transform.localScale.x;
        }
        else if (res.Contains("800") && (res.Contains("600") || res.Contains("480")))
        {
            scrollOffset = 51f * this.transform.localScale.x;
        }
        else if (res.Contains("1535"))
        {
            scrollOffset = 98f * this.transform.localScale.x;
        }
        else if (res.Contains("1382"))
        {
            scrollOffset = 88f * this.transform.localScale.x;
        }
        else if (res.Contains("1151"))
        {
            scrollOffset = 73.5f * this.transform.localScale.x;
        }
        else if (res.Contains("1919"))
        {
            scrollOffset = 122.5f * this.transform.localScale.x;
        }
        else
        {
            scrollBar.gameObject.GetComponentInParent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scrollOffset = 122.5f * this.transform.localScale.x;
        }

    }


}

