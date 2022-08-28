using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private Image logo;

    [SerializeField]
    private Image text;

    void Start()
    {
        StartCoroutine(Intro());
    }

    private IEnumerator Intro() {
        yield return new WaitForSeconds(2f);

        logo.DOFade(1f, 2f);

        text.DOFade(1f, 2f);

        yield return new WaitForSeconds(4f);

        logo.DOFade(0f, 2f);

        text.DOFade(0f, 2f);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("SampleScene");

    }
}
