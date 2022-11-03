using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridPlayer : MonoBehaviour
{
    public Player id;
    private int gridsAdded = 0;
    private Dictionary<int, Grid> memory;
    // Start is called before the first frame update
    void Start()
    {
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
        gridsAdded++;
        grid.SetSequence(this.gridsAdded);
        grid.SetPlayer(this.id);
        grid.SetPlayerScript(this);
        memory.Add(gridsAdded, grid);
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

