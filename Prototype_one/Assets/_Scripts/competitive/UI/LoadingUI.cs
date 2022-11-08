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

    private IEnumerator co;

    private void Awake()
    {
        circleImageFill = transform.GetChild(0).GetComponent<Image>();
        fxHolder = transform.GetChild(1).GetComponent<RectTransform>();
    }
    public void StartLoading(float time)
    {
        co = Load(time);
        StartCoroutine(co);
    }

    public IEnumerator Load(float time)
    {
        if (circleImageFill == null)
            Debug.Log("null");
        else
        {
            Debug.Log("not null");
        }
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
