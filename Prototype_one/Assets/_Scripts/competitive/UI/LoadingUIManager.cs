using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject loadingUi;

    private Stack<GameObject> uiPool;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        uiPool = new Stack<GameObject>();
    }
    public void InstantiateUI(Vector3 position, float time)
    {
        Vector3 pos = cam.WorldToScreenPoint(position);

        var ui = Instantiate(loadingUi, gameObject.transform);
        ui.transform.position = pos;
        var script = ui.GetComponent<LoadingUI>();
        script.StartLoading(time);
    }
}
