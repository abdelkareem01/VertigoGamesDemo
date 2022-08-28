using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField]
    private WheelSlice bomb;

    [SerializeField]
    private List<GameObject> wheels = new List<GameObject>();

    [SerializeField]
    private List<WheelSlice> Tier1_Items = new List<WheelSlice>();

    [SerializeField]
    private List<WheelSlice> Tier2_Items = new List<WheelSlice>();

    [SerializeField]
    private List<WheelSlice> Tier3_Items = new List<WheelSlice>();

    private List<WheelSlice> Tier1_Items_backUp = new List<WheelSlice>();

    private List<WheelSlice> Tier2_Items_backUp = new List<WheelSlice>();

    private List<WheelSlice> Tier3_Items_backUp = new List<WheelSlice>();

    [SerializeField]
    private GameObject wheelCanvas, levelProgressUI;

    [SerializeField]
    private int maxSlots;

    [HideInInspector]
    public int level = 1;

    [HideInInspector]
    public bool premiumLoaded = false, normalLoaded = false;

    private List<WheelSlice> itemsPool = new List<WheelSlice>();

    private GameObject itemMenu;

    private int type;

    [HideInInspector]
    public WheelSlice obtainedItem = new WheelSlice();

    [SerializeField]
    private inventoryManager iM;

    void Awake()
    {
        
        Invoke("OnValidate", 0f);

        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        #endregion

        type = 0;

        listFiller(Tier1_Items, Tier1_Items_backUp);

        listFiller(Tier2_Items, Tier2_Items_backUp);

        listFiller(Tier3_Items, Tier3_Items_backUp);
        
    }

    void Update()
    {
        if (level % 5 == 0 && level % 30 != 0 && !premiumLoaded)
        {
            this.type = 1;
            StartCoroutine(loadWheel());
        }
        else if (level % 30 == 0 && !premiumLoaded)
        {
            this.type = 2;
            StartCoroutine(loadWheel());
        }
        else if (level % 5 != 0 && level % 30 != 0 && !normalLoaded)
        {
            this.type = 0;
            StartCoroutine(loadWheel());
        }


        Tier1_Items.Clear();
        listFiller(Tier1_Items_backUp, Tier1_Items);

        Tier2_Items.Clear();
        listFiller(Tier2_Items_backUp, Tier2_Items);

        Tier3_Items.Clear();
        listFiller(Tier3_Items_backUp, Tier3_Items);

    }

    public IEnumerator loadWheel()
    {
        normalLoaded = true;
        premiumLoaded = true;

        if (level != 1)
        {
            getActiveWheel().transform.DOScale(0, 1.5f);
            yield return new WaitForSeconds(1.5f);
            Destroy(getActiveWheel());

        }

        if (level == 1) StartCoroutine(loadW());

    }

    public void splitAdd(List<WheelSlice> list1, List<WheelSlice> list2)
    {
        for (int i = 0; i < maxSlots / 2; i++)
        {

            int index = Random.Range(0, list1.Count);
            itemsPool.Add(list1[index]);
            list1.RemoveAt(index);
        }

        for (int i = 0; i < maxSlots / 2; i++)
        {
            int index = Random.Range(0, list2.Count);
            itemsPool.Add(list2[index]);
            list2.RemoveAt(index);

        }

    }

    public void mixPool()
    {
        for (int i = 0; i < itemsPool.Count; i++)
        {
            WheelSlice tmPiece = itemsPool[i];
            int randIndex = Random.Range(i, itemsPool.Count);
            itemsPool[i] = itemsPool[randIndex];
            itemsPool[randIndex] = tmPiece;
        }

    }

    public void listFiller(List<WheelSlice> source, List<WheelSlice> dest)
    {

        foreach (WheelSlice p in source)
        {

            dest.Add(p);
        }

    }

    public GameObject getActiveWheel()
    {

        for (int i = 0; i < wheelCanvas.transform.childCount; i++)
        {
            if (wheelCanvas.transform.GetChild(i).gameObject.activeSelf && wheelCanvas.transform.GetChild(i).gameObject.tag == "wheel")
                return wheelCanvas.transform.GetChild(i).gameObject;
        }
        return null;
    }

    public void loadItemMenu()
    {
        if (obtainedItem.name == "bomb")
        {
            itemMenu.transform.GetChild(1).gameObject.SetActive(true);
            itemMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "OH NO! TRY AGAIN";
            itemMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.red;
            itemMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().DOFade(1f, 2f);
            itemMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.black;
            itemMenu.transform.DOScale(1f, 1f);

        }
        else
        {
            itemMenu.transform.GetChild(1).gameObject.SetActive(false);
            itemMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = obtainedItem.name;
            itemMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().DOFade(1f, 2f);
            itemMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = obtainedItem.rarity;
            itemMenu.transform.DOScale(1f, 1f);
            StartCoroutine(dissipateItemMenu());
        }

    }

    public IEnumerator dissipateItemMenu()
    {
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(loadW());
        StartCoroutine(levelProgressUI.GetComponent<levelProgressIndicator>().moveNext());
        iM.addItem(obtainedItem);
    }


    public IEnumerator loadW()
    {
        if (level != 1)
        {
            itemMenu.transform.DOScale(0f, 1f);
            GameObject.Find(obtainedItem.Index + "").transform.GetChild(0).GetComponent<Image>().DOFade(0, 1.5f);
            yield return new WaitForSeconds(1.5f);
            Destroy(GameObject.Find(obtainedItem.Index + ""));
        }
        GameObject wheel = Instantiate(wheels[type], new Vector3(wheelCanvas.transform.position.x, wheelCanvas.transform.position.y - 5f, wheelCanvas.transform.position.z),
        Quaternion.identity, wheelCanvas.transform);
        wheel.SetActive(false);
        if (type == 0)
        {
            if (level != 1)
            {
                itemsPool.Add(bomb);
                for (int i = 0; i < maxSlots - 1; i++)
                {
                    int index = Random.Range(0, Tier1_Items.Count);
                    itemsPool.Add(Tier1_Items[index]);
                    Tier1_Items.RemoveAt(index);
                }
            }

            else
            {
                for (int i = 0; i < maxSlots; i++)
                {
                    int index = Random.Range(0, Tier1_Items.Count);
                    itemsPool.Add(Tier1_Items[index]);
                    Tier1_Items.RemoveAt(index);
                }

            }

        }
        else if (type == 1)
        {
            splitAdd(Tier1_Items, Tier2_Items);
        }

        else
        {
            splitAdd(Tier2_Items, Tier3_Items);
        }

        mixPool();
        yield return new WaitForSeconds(0.5f);
        wheel.SetActive(true);
        wheel.GetComponent<RouletteWheel>().setPool(itemsPool);
        itemsPool.Clear();

    }
    public Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    private void OnValidate()
    {
        itemMenu = GameObject.Find("ui_obtainedItem_Canvas").transform.GetChild(0).gameObject;
        itemMenu.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SampleScene");
        });

        itemMenu.transform.GetChild(1).gameObject.SetActive(false);
    }
}
