using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    public static LoadingUIManager instance;

    [SerializeField]
    private GameObject loadingUi;

    private static Stack<GameObject> uiPool;
    private Dictionary<System.Tuple<int, int>, LoadingUI> memory;

    private Camera cam;

    private void Awake()
    {
        /*if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }*/
        instance = this;
        cam = Camera.main;
        uiPool = new Stack<GameObject>();
        memory = new Dictionary<System.Tuple<int, int>, LoadingUI>();
    }

    private void Start()
    {
        /*cam = Camera.main;
        uiPool = new Stack<GameObject>();
        memory = new Dictionary<System.Tuple<int, int>, LoadingUI>();*/
    }
    public void LoadUI(System.Tuple<int, int> key, Vector3 position, float time)
    {
        if (memory.ContainsKey(key))
            return;
        if (uiPool.Count == 0)
            ExpendStack(2);
        var instance = uiPool.Pop();
        instance.SetActive(true);


        //position mapping
        Vector3 pos = cam.WorldToScreenPoint(position);

        instance.transform.position = pos;
        var script = instance.GetComponent<LoadingUI>();
        script.setKey(key);
        memory.Add(key, script);
        script.StartLoading(time);
    }

    public void StopLoadingUI(System.Tuple<int, int> key)
    {
        if (memory.ContainsKey(key))
            memory[key].StopLoading();
    }

    public void ExpendStack(int num)
    {
        for(int i = 0; i < num; i++)
        {
            var instance = Instantiate(loadingUi);
            instance.transform.SetParent(gameObject.transform);
            instance.SetActive(false);
            uiPool.Push(instance);
        }
    }

    public void AddToPool(GameObject obj)
    {
        uiPool.Push(obj);
    }

    public void RemoveFromMemo(System.Tuple<int, int> key)
    {
        this.memory.Remove(key);
    }
}
