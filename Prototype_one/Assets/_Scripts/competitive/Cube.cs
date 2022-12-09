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
    public static float RANDOM_LOWER = 0.4f;
    public static float RANDOM_UPPER = 0.6f;
    public static Vector3 offSet = new Vector3(0, 0.2f, 0);

    private int x;
    private int y;
    private float duration;
    private bool isActive;
    private IEnumerator logicCo;
    private IEnumerator translateCo;
    private IEnumerator colorCo;
    private Material mat;
    private Color initColor;
    private Vector3 initPos;
    private Vector3 endPos;
    private System.Tuple<int, int> key;
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        initColor = mat.color;
        initPos = gameObject.transform.position;
        endPos = initPos + offSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;
        //when occupied: should consider newcomers
        if (!this.parent.GetOccupied())
            return;
        GridPlayer player = other.GetComponent<GridPlayer>();
        if(player != null)
        {
            if(player.occupyingGrid != null)
            {
                player.occupyingGrid.GetCube().ResetCube();
            }
            player.occupyingGrid = this.parent;
            //change data inside grid, gridplayer
            logicCo = CountDown(player);
            StartCoroutine(logicCo);
            //handle translation effect
            translateCo = MoveUp();
            StartCoroutine(translateCo);
            //set cube color to shiny
            colorCo = LerpColor(BoardManager.instance.FindColor(player), this.duration, 10f) ;
            StartCoroutine(colorCo);
            //handle UI
            key = System.Tuple.Create(x,y);
            LoadingUIManager.instance.LoadUI(key, initPos, duration);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isActive)
            return;
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null)
        {
            //cases when exit shouldn't be considered:
            //leaving other's current grid shouldn't cancel out other's effect
            if (this.parent.GetPlayer() != Player.PLAYER_NULL && player.GetId() != this.parent.GetPlayer())
                return;
            ResetCube();
        }
        LoadingUIManager.instance.StopLoadingUI(key);
    }
    public void ResetCube()
    {
        //stop all the coroutines in OnTriggerEnter()
        StopAllCoroutines();
        //move cube back to original position
        translateCo = MoveDown();
        StartCoroutine(translateCo);
        //set cube color to not shiny
        colorCo = LerpColor(initColor, 0.2f, 1f);
        StartCoroutine(colorCo);
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
        player.occupyingGrid = null;
        HandleGridUpdate(player);
        translateCo = MoveDown();
        StartCoroutine(translateCo);
    }
    IEnumerator CountDown(GridPlayer player, Cube prevCube)
    {
        duration = Random.Range(RANDOM_LOWER, RANDOM_UPPER);
        float timer = 0.0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        //set current player's previous cube to not shiny mat
        prevCube.SetColor(prevCube.GetInitColor(), 1f);
        //update grid information
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
    public void SetColor(Color c, float intensity)
    {
        mat.SetColor("_EmissionColor", c * intensity);
        mat.SetColor("_BaseColor", c * intensity);
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

    public void Reset()
    {
        this.initColor = NULL;
        SetColor(this.initColor, 1.0f);
    }
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
}
