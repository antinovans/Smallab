using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public Grid parent;
    public static Color YELLOW = Color.yellow;
    public static Color GREEN = Color.green;
    public static Color BLUE = Color.blue;
    public static Color YELLOW_H = Color.yellow;
    public static Color GREEN_H = Color.green;
    public static Color BLUE_H = Color.blue;
    public static Color NULL = Color.white;
    public static float RANDOM_LOWER = 0.4f;
    public static float RANDOM_UPPER = 0.6f;
    public static Vector3 offSet = new Vector3(0, 0.2f, 0);

    public float duration;

    private int x;
    private int y;
    private IEnumerator logicCo;
    private IEnumerator translateCo;
    private IEnumerator colorCo;
    private Material mat;
    private Color initColor;
    private Vector3 initPos;
    private Vector3 endPos;
    private System.Tuple<int, int> key;
    //temp
    private bool isEnd;
    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;
        mat = gameObject.GetComponent<Renderer>().material;
        initColor = mat.color;
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
        //when occupied: should consider newcomers
        if (!this.parent.GetOccupied())
            return;
        GridPlayer player = other.GetComponent<GridPlayer>();
        if(player != null)
        {
            //change data inside grid, gridplayer
            logicCo = CountDown(player);
            StartCoroutine(logicCo);
            //handle translation effect
            translateCo = MoveUp();
            StartCoroutine(translateCo);
            //handle color transition
            colorCo = LerpColor(FindColor(player), this.duration, 10f) ;
            StartCoroutine(colorCo);
            //handle UI
            key = System.Tuple.Create(x,y);
            LoadingUIManager.instance.LoadUI(key, initPos, duration);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null)
        {
            //cases when exit shouldn't be considered:
            //leaving other's current grid shouldn't cancel out other's effect
            if (this.parent.GetPlayer() != Player.PLAYER_NULL && player.GetId() != this.parent.GetPlayer())
                return;
            //entering other's current grid shouldn't cancel out current's effect
/*            if (player.GetCurrentGrid() == this.parent)
                return;*/
            //handle translation effect
            StopCoroutine(translateCo);
            translateCo = MoveDown();
            StartCoroutine(translateCo);
            //stop data changing inside grid, gridplayer
            StopCoroutine(logicCo);
            if (this.parent.GetPlayer() != player.GetId())
            {
                //color transition back to normal
                StopCoroutine(colorCo);
            }
            colorCo = LerpColor(initColor, 0.2f, 1f);
            StartCoroutine(colorCo);
        }
        LoadingUIManager.instance.StopLoadingUI(key);
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
        if (player.GetId() != this.parent.GetPlayer() && this.parent.GetPlayer() != Player.PLAYER_NULL)
        {
            this.parent.DelinkLowerGrids();
        }
        //step to a grid that explored before
        if (player.GetId() == this.parent.GetPlayer() && player.GetCurrentGrid().CheckAddValid(this.parent))
        {
            player.PushGridToHighest(this.parent);
            return;
        }
        player.AddGridToMemo(this.parent);
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
        this.initColor = c;
    }
    public void SetColorWithLerp(Color c, float time, float intensity)
    {
        StartCoroutine(LerpColor(c, time, intensity));
    }
    IEnumerator LerpColor(Color c, float time, float intensity)
    {
        float timer = 0.0f;
        while (timer <= time)
        {
            var tempColor = Color.Lerp(initColor, c, timer / time);
            mat.SetColor("_BaseColor", tempColor);
            mat.SetColor("_EmissionColor", tempColor * intensity);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.SetColor("_BaseColor", c);
        mat.SetColor("_EmissionColor", c * intensity);
        initColor = c;
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
    public Color GetInitColor()
    {
        return this.initColor;
    }
}
