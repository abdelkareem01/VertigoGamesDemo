using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class inventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryItemParent;

    [SerializeField]
    private GameObject inventoryItem;

    private List<WheelSlice> collectedItems = new List<WheelSlice>();

    void Start()
    {
        transform.DOScaleX(1f, 1.5f).SetEase(Ease.OutBack);
        transform.DOScaleY(1.2f, 1.5f).SetEase(Ease.OutBack);
        transform.DOScaleZ(1.2f, 1.5f).SetEase(Ease.OutBack);

        Invoke("OnValidate", 0f);
    }

    public void addItem(WheelSlice item) {

        GameObject iT = Instantiate(inventoryItem, inventoryItem.transform.position, Quaternion.identity, inventoryItemParent.transform);
        iT.GetComponent<Image>().sprite = item.Icon;
        iT.GetComponent<Image>().rectTransform.sizeDelta = item.resolution;
        iT.GetComponent<Image>().DOFade(1f, 1f);

        collectedItems.Add(item);

    }

    private void OnValidate()
    {
        this.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SampleScene");
        });
    }

}
