using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    private Grid[,] board;
    public int numOfGrid;
    public GameObject cube;
    public Vector3 initPos;
    public float gap;

    public float X_MIN;
    public float X_MAX;
/*    public float Y_MIN;
    public float Y_MAX;*/
    private float gridSize;
    private bool isInstantiated = false;
    // Start is called before the first frame update
    void Start()
    {
        /*InitializeBoard();*/
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isInstantiated)
        {
            InitializeBoard();
            isInstantiated = true;
        }
    }

    private void InitializeBoard()
    {
        gridSize = (X_MAX - X_MIN) / numOfGrid;
        board = new Grid[numOfGrid, numOfGrid];
        for (int i = 0; i < numOfGrid; i++)
        {
            for (int j = 0; j < numOfGrid; j++)
            {
                Vector3 offset = new Vector3(i * (gridSize + gap), 0, -
                    j * (gridSize + gap));
                board[i, j] = new Grid(i, j, initPos + offset, gridSize, Player.PLAYER_NULL);

                GameObject obj = Instantiate(cube);
                obj.name = i + "," + j;
                Cube instance = obj.GetComponent<Cube>();
                if(instance != null)
                {
                    instance.SetParent(board[i, j]);
                }
                obj.transform.SetParent(gameObject.transform);
                board[i, j].SetObj(obj.GetComponent<Cube>());
                //update transform
                SetObjPos(obj, board[i, j].GetWorldPos());
                SetObjSize(obj, gridSize);
            }
        }
    }

    // Update is called once per frame

    public void SetObjSize(GameObject obj, float size)
    {
        obj.transform.localScale = new Vector3(size, size, size);
    }
    public void SetObjPos(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }
}
