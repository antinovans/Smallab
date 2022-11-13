using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    PLAYER_BLUE,
    PLAYER_YELLOW,
    PLAYER_GREEN,
    PLAYER_NULL
}
public class Grid
{
    private int x;
    private int y;
    private Player player;
    private Vector3 initPos;
    private float gridSize;
    private Vector3 worldPos;
    private Cube cube;
    private bool canOccupy;
    //doubled link list like
    public Grid higher;
    public Grid lower;

    public Grid(int x, int y, Vector3 initPos, float gridSize, Player player)
    {
        this.x = x;
        this.y = y;
        this.initPos = initPos;
        this.gridSize = gridSize;
        this.worldPos = CalculateWorldPos(x, y, initPos, gridSize);
        this.player = player;
        this.canOccupy = true;
        this.higher = null;
        this.lower = null;
    }
    //get the world position of the grid
    private Vector3 CalculateWorldPos(int x, int y, Vector3 initPos, float gridSize)
    {
        Vector3 offset = new Vector3((gridSize / 2), 0, (-gridSize / 2));
        return initPos + offset;
    }
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    public Player GetPlayer()
    {
        return this.player;
    }
    public Vector3 GetWorldPos()
    {
        return this.worldPos;
    }
    public int GetX()
    {
        return this.x;
    }
    public int GetY()
    {
        return this.y;
    }
    public void SetCube(Cube obj)
    {
        this.cube = obj;
    }
    public Cube GetCube()
    {
        return this.cube;
    }
    public void SetOccupied(bool input) 
    {
        this.canOccupy = input;
    }
    public bool GetOccupied()
    {
        return this.canOccupy;
    }
    public void DelinkLowerGrids()
    {
        Grid temp = this;
        Grid right = temp.higher;
        if(right != null)
        {
            right.lower = null;
            temp.higher = null;
        }
        List<Cube> cubes = new List<Cube>();
        while (temp != null)
        {
            if (temp != this)
                cubes.Add(temp.GetCube());
            Grid prevGrid = temp.lower;
            temp.lower = null;
            if(prevGrid != null)
                prevGrid.higher = null;
            temp.SetPlayer(Player.PLAYER_NULL);
            temp.SetOccupied(true);
            temp = prevGrid;
        }
        CubeManager.instance.HandleCubesColor(cubes, Cube.NULL, 0.1f);
    }
    public void DelinkHigherGrids()
    {
        Grid temp = this;
        while (temp != null)
        {
            Grid nextGrid = temp.higher;
            temp.higher = null;
            if(nextGrid != null)
                nextGrid.lower = null;
            temp.SetPlayer(Player.PLAYER_NULL);
/*            temp.SetSequence(-1);*/
            temp.SetOccupied(true);
/*            temp.SetPlayerScript(null);*/
            temp.GetCube().SetColor(Cube.NULL);
            temp = nextGrid;
        }
    }
    public void DelinkWholeGrids()
    {
        List<Cube> cubes = new List<Cube>();
        Grid highest = GetHighest();
        while(highest.higher != null)
        {
            highest = highest.higher;
        }
        /*highest.SetAllLowerGridColor(Cube.NULL, 0.5f);*/
        while(highest != null)
        {
            Grid left = highest.lower;
            if (left != null)
                left.higher = null;
            highest.lower = null;
            cubes.Add(highest.GetCube());
            highest.SetPlayer(Player.PLAYER_NULL);
            highest.SetOccupied(true);
            /*highest.GetCube().SetColor(Cube.NULL);*/
            highest = left;
        }
        CubeManager.instance.HandleCubesColor(cubes, Cube.NULL, 0.1f);
    }
    public Grid GetHighest()
    {
        Grid highest = this;
        while (highest.higher != null)
        {
            highest = highest.higher;
        }
        return highest;
    }
    public void PushGridToRightest()
    {
        if (this.higher == null)
            return;
        var left = this.lower;
        var right = this.higher;
        var highest = this;
        while(highest.higher != null)
        {
            highest = highest.higher;
        }
        if (left != null)
            left.higher = right;
        right.lower = left;
        highest.higher = this;
        this.lower = highest;
        this.higher = null;
        highest.SetOccupied(true);
        this.SetOccupied(false);
    }
    public bool CheckAddValid(Grid grid)
    {
        if (Mathf.Abs(this.GetX() - grid.GetX()) + Mathf.Abs(this.GetY() - grid.GetY()) > 1)
        {
            Debug.Log("invalid!");
            return false;
        }
        return true;
    }
    public Grid AddGrid(Grid grid)
    {
        if (!CheckAddValid(grid))
        {
            /*DelinkHigherGrids();
            DelinkLowerGrids();*/
            DelinkWholeGrids();
        }
        else
        {
            this.higher = grid;
            grid.lower = this;
            this.canOccupy = true;
            /*grid.canOccupy = false;*/
        }
        grid.canOccupy = false;
        return grid;
    }
}
