using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public Grid parent;
    public static Color YELLOW = Color.yellow;
    public static Color GREEN = Color.green;
    public static Color BLUE = Color.blue;
    public static Color NULL = Color.white;
    public static float RANDOM_LOWER = 1f;
    public static float RANDOM_UPPER = 1.2f;
    public static Vector3 offSet = new Vector3(0, 0.2f, 0);

    private IEnumerator logicCo;
    private IEnumerator translateCo;
    private Material mat;
    private Vector3 initPos;
    private Vector3 endPos;
    private float duration;
    /*private Renderer r;*/
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        initPos = gameObject.transform.position;
        endPos = initPos + offSet;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetParent(Grid grid)
    {
        this.parent = grid;
    }

    private void OnTriggerEnter(Collider other)
    {
        GridPlayer player = other.GetComponent<GridPlayer>();
        if(player != null)
        {
            //change date inside grid, gridplayer
            logicCo = CountDown(player);
            StartCoroutine(logicCo);
            //handle translation effect
            translateCo = MoveUp();
            StartCoroutine(translateCo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null)
        {
            StopCoroutine(logicCo);
            StopCoroutine(translateCo);
            translateCo = MoveDown();
            StartCoroutine(translateCo);
        }
    }
    IEnumerator CountDown(GridPlayer player)
    {
        duration = Random.Range(RANDOM_LOWER, RANDOM_UPPER);
        float timer = 0.0f;
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        HandleGridUpdate(player);
        translateCo = MoveDown();
        StartCoroutine(translateCo);
    }

    IEnumerator MoveUp()
    {
        float timer = 0.0f;
        while(timer <= duration)
        {
            transform.position = Vector3.Lerp(initPos, endPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }

    IEnumerator MoveDown()
    {
        float timer = 0.0f;
        Vector3 beginPos = transform.position;
        while (timer <= 0.2f)
        {
            transform.position = Vector3.Lerp(beginPos, initPos, timer / 0.2f);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = initPos;
    }

    private void HandleGridUpdate(GridPlayer player)
    {
        //robbing other's grid
        if (player.GetId() != this.parent.GetPlayer())
        {
            if (this.parent.GetPlayerScript() != null)
                this.parent.GetPlayerScript().DeleteLowerMemoByGrid(this.parent);
        }
        //step to a grid that explored before
        if (player.GetId() == this.parent.GetPlayer())
        {
            if (this.parent.GetPlayerScript() != null)
                this.parent.GetPlayerScript().DeleteUpperMemoByGrid(this.parent);
        }
        //manipulating grid color
        Color c = FindColor(player);
        SetColor(c);
        player.AddGridToMemo(parent);
    }
    private Color FindColor(GridPlayer p)
    {
        if (p.GetId() == Player.PLAYER_YELLOW)
        {
            return YELLOW;
        }
        if (p.GetId() == Player.PLAYER_BLUE)
        {
            return BLUE;
        }
        if (p.GetId() == Player.PLAYER_GREEN)
        {
            return GREEN;
        }
        return NULL;
    }
    public void SetColor(Color c)
    {
        mat.SetColor("_EmissionColor", c);
        mat.SetColor("_BaseColor", c);
    }
}
