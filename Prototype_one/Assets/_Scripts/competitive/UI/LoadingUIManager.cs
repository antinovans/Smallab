using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    public GameObject loadingUi;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    public void InstantiateUI(Vector3 position, float time)
    {
        Debug.Log("haha");
        Vector3 pos = cam.WorldToScreenPoint(position);

        var ui = Instantiate(loadingUi, gameObject.transform);
        ui.transform.position = pos;
        ui.GetComponent<LoadingUI>().StartLoading(time);
    }
}
