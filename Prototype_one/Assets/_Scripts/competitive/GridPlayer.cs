using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridPlayer : MonoBehaviour
{
    public Player id;
    private int gridsAdded = 0;
    private Dictionary<int, Grid> memory;
    private Grid lastGrid;
    // Start is called before the first frame update
    void Start()
    {
        lastGrid = null;
        memory = new Dictionary<int, Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Player GetId()
    {
        return this.id;
    }

    public void AddGridToMemo(Grid grid)
    {
        if (!checkAddValid(grid))
            EmptyMemo();
        this.lastGrid = grid;
        gridsAdded++;
        grid.SetSequence(this.gridsAdded);
        grid.SetPlayer(this.id);
        grid.SetPlayerScript(this);
        memory.Add(gridsAdded, grid);
    }
    private bool checkAddValid(Grid grid)
    {
        if (lastGrid == null)
            return true;
        if (Mathf.Abs(lastGrid.GetX() - grid.GetX()) + Mathf.Abs(lastGrid.GetY() - grid.GetY()) > 1)
            return false;
        return true;
    }
    public void EmptyMemo()
    {
        foreach(var g in memory)
        {
            g.Value.SetPlayer(Player.PLAYER_NULL);
            g.Value.SetSequence(-1);
            g.Value.SetPlayerScript(null);
            g.Value.GetCube().SetColor(Cube.NULL);
        }
        memory.Clear();
        gridsAdded = 0;
    }

    public void DeleteLowerMemoByGrid(Grid grid)
    {
        int sequence = grid.GetSequence();
        foreach (var m in memory)
        {
            if(m.Key <= sequence)
            {
                m.Value.SetPlayer(Player.PLAYER_NULL);
                m.Value.SetSequence(-1);
                m.Value.SetPlayerScript(null);
                m.Value.GetCube().SetColor(Cube.NULL);
                /*memory.Remove(m.Key);*/
            }
        }
        memory = memory.Where(kvp => kvp.Key > sequence).ToDictionary(k=>k.Key, k=>k.Value);
    }
    public void DeleteUpperMemoByGrid(Grid grid)
    {
        int sequence = grid.GetSequence();
        foreach (var m in memory)
        {
            if (m.Key > sequence)
            {
                m.Value.SetPlayer(Player.PLAYER_NULL);
                m.Value.SetSequence(-1);
                m.Value.SetPlayerScript(null);
                m.Value.GetCube().SetColor(Cube.NULL);
                /*memory.Remove(m.Key);*/
            }
        }
        memory = memory.Where(kvp => kvp.Key <= sequence).ToDictionary(k => k.Key, k => k.Value);
    }
}

