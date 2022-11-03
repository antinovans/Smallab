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

    private IEnumerator co;
    private Material mat;
    /*private Renderer r;*/
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
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
            co = CountDown(player);
            StartCoroutine(co);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null)
        {
            StopCoroutine(co);
        }
    }
    IEnumerator CountDown(GridPlayer player)
    {
        float duration = Random.Range(RANDOM_LOWER, RANDOM_UPPER);
        float timer = 0.0f;
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        HandleGridUpdate(player);
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
