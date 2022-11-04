using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    RectTransform fxHolder;
    [SerializeField]
    Image circleImageFill;

    private void Awake()
    {
        circleImageFill = transform.GetChild(0).GetComponent<Image>();
        fxHolder = transform.GetChild(1).GetComponent<RectTransform>();
    }
    private void Start()
    {
        StartLoading(1);
    }
    public void StartLoading(float time)
    {
        StartCoroutine(Load(time));
    }

    IEnumerator Load(float time)
    {
        float timer = 0f;
        while(timer <= time)
        {
            float progress = timer / time;
            circleImageFill.fillAmount = progress;
            fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
