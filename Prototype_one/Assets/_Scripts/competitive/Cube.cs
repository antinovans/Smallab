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
    public static float RANDOM_LOWER = 0.8f;
    public static float RANDOM_UPPER = 0.1f;
    public static Vector3 offSet = new Vector3(0, 0.2f, 0);

    public float duration;

    private int x;
    private int y;
    private IEnumerator logicCo;
    private IEnumerator translateCo;
    private Material mat;
    private Vector3 initPos;
    private Vector3 endPos;
    private System.Tuple<int, int> key;
    //temp
    private bool isEnd;
    /*private Renderer r;*/
    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;
        mat = gameObject.GetComponent<Renderer>().material;
        initPos = gameObject.transform.position;
        endPos = initPos + offSet;
    }

    // Update is called once per frame
    // calculate winner
    void Update()
    {
        if(BoardGenerator.isEnd && !isEnd)
        {
            isEnd = true;
            switch(BoardGenerator.CalculateWinner())
            {
                case Player.PLAYER_BLUE:
                    SetColor(BLUE);
                    break;
                case Player.PLAYER_GREEN:
                    SetColor(GREEN);
                    break;
                case Player.PLAYER_YELLOW:
                    SetColor(YELLOW);
                    break;
                default:
                    break;
            }
        }
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
            //handle UI
            /*uimanager.InstantiateUI(initPos, duration);*/
            key = System.Tuple.Create(x,y);
            LoadingUIManager.instance.LoadUI(key, initPos, duration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null)
        {
            StopCoroutine(logicCo);
            StopCoroutine(translateCo);
            //handle translation effect
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
    public void SetParentGrid(Grid grid)
    {
        this.parent = grid;
        this.x = grid.GetX();
        this.y = grid.GetY();
    }
    public int GetX()
    {
        return this.x;
    }
    public int GetY()
    {
        return this.y;
    }

}
