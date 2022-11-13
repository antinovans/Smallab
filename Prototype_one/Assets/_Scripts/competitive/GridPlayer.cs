using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridPlayer : MonoBehaviour
{
    [SerializeField]
    private Player id;
    private Grid lastGrid;
    public bool isAdding;
    // Start is called before the first frame update
    void Start()
    {
        lastGrid = null;
        isAdding = false;
    }

    public Player GetId()
    {
        return this.id;
    }
    //adding a grid to the memory
    public void AddGridToMemo(Grid grid)
    {
        if(lastGrid == null)
        {
            this.lastGrid = grid;
            this.lastGrid.SetOccupied(false);
            this.lastGrid.SetPlayer(this.id);
            return;
        }
        this.lastGrid = this.lastGrid.AddGrid(grid);
        this.lastGrid.SetPlayer(this.id);
    }

    public void PushGridToHighest(Grid grid)
    {
        grid.PushGridToRightest();
        this.lastGrid = grid;
    }
    //return the current grid
    public Grid GetCurrentGrid()
    {
        return this.lastGrid;
    }
}

