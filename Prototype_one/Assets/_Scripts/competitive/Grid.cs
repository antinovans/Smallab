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
    private int sequence;
    private GridPlayer playerScript;
    private Cube cube;

    public Grid(int x, int y, Vector3 initPos, float gridSize, Player player)
    {
        this.x = x;
        this.y = y;
        this.initPos = initPos;
        this.gridSize = gridSize;
        this.worldPos = CalculateWorldPos(x, y, initPos, gridSize);
        this.player = player;
        this.sequence = -1;
        playerScript = null;
    }
/*    public Grid(int x, int y, Vector3 initPos, float gridSize, Player player, GameObject obj)
    {
        this.x = x;
        this.y = y;
        this.initPos = initPos;
        this.gridSize = gridSize;
        this.worldPos = CalculateWorldPos(x, y, initPos, gridSize);
        this.player = player;
        this.obj = obj;
    }*/

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

    public int GetSequence()
    {
        return this.sequence;
    }

    public void SetSequence(int i)
    {
        this.sequence = i;
    }

    public GridPlayer GetPlayerScript()
    {
        return this.playerScript;
    }

    public void SetPlayerScript(GridPlayer ps)
    {
        this.playerScript = ps;
    }
    public void SetCube(Cube obj)
    {
        this.cube = obj;
    }
    public Cube GetCube()
    {
        return this.cube;
    }



}
