using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [Header("Board Setting")]
    public int numOfGrid;
    public GameObject cube;
    public Vector3 initPos;
    public float gap;

    public float X_MIN;
    public float X_MAX;
    //memory
    private static Grid[,] board;
    private static Cube[,] cubes;
    private float gridSize;
    private bool isInstantiated = false;
    private List<Player> winners;
    public static bool isEnd = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        winners = new List<Player>();
    }

    public void InitializeBoard()
    {
        if (board == null)
        {
            gridSize = (X_MAX - X_MIN) / numOfGrid;
            board = new Grid[numOfGrid, numOfGrid];
            cubes = new Cube[numOfGrid, numOfGrid];
            for (int i = 0; i < numOfGrid; i++)
            {
                for (int j = 0; j < numOfGrid; j++)
                {
                    //calculating grid position
                    Vector3 offset = new Vector3(i * (gridSize + gap), 0, -
                        j * (gridSize + gap));
                    board[i, j] = new Grid(i, j, initPos + offset, gridSize, Player.PLAYER_NULL);

                    //instantiate cube and configure cube's position
                    GameObject obj = Instantiate(cube);
                    obj.name = i + "," + j;
                    Cube instance = obj.GetComponent<Cube>();
                    if (instance != null)
                    {
                        instance.SetParentGrid(board[i, j]);
                    }

                    //update transform
                    obj.transform.SetParent(gameObject.transform);
                    SetCubePos(obj, board[i, j].GetWorldPos());
                    SetCubeSize(obj, gridSize);
                    //update Cube.cs
                    board[i, j].SetCube(instance);
                    cubes[i, j] = instance;
                }
            }
        }
        else
        {
            ClearBoard();
            ClearCubes();
        }
    }

    private void ClearCubes()
    {
        if(cubes != null)
        {
            foreach (var c in cubes)
                c.Reset();
        }
    }

    private void ClearBoard()
    {
        if (board != null)
        {
            foreach (var b in board)
                b.Reset();
        }
    }

    // Update is called once per frame
    public void SetCubeSize(GameObject obj, float size)
    {
        obj.transform.localScale = new Vector3(size, size, size);
    }
    public void SetCubePos(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }
    public void UpdateWinners()
    {
        this.winners.Clear();
        int blue = 0, yellow = 0, green = 0;

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j].GetPlayer() == Player.PLAYER_BLUE)
                {
                    blue++;
                }
                if (board[i, j].GetPlayer() == Player.PLAYER_GREEN)
                {
                    green++;
                }
                if (board[i, j].GetPlayer() == Player.PLAYER_YELLOW)
                {
                    yellow++;
                }
            }
        }
        int max = Mathf.Max(blue, Mathf.Max(yellow, green));
        if (max == blue)
        {
            winners.Add(Player.PLAYER_BLUE);
        }
        if (max == green)
        {
            winners.Add(Player.PLAYER_GREEN);
        }
        if (max == yellow)
        {
            winners.Add(Player.PLAYER_YELLOW);
        }
    }

    public void HandleCubesColor(List<Cube> cubes, Color color, float time)
    {
        StartCoroutine(LerpCubesColor(cubes, color, time));
    }
    IEnumerator LerpCubesColor(List<Cube> cubes, Color color, float time)
    {
        foreach (var c in cubes)
        {
            c.SetColorWithLerp(color, time, 1f);
            yield return new WaitForSeconds(time);
        }
    }

    public Color FindColor(GridPlayer p)
    {
        return FindColor(p.GetId());
    }
    public Color FindColor(Player p)
    {
        Color c;
        switch(p)
        {
            case Player.PLAYER_YELLOW:
            c = Cube.YELLOW;
            break;
            case Player.PLAYER_BLUE:
            c = Cube.BLUE;
            break;
            case Player.PLAYER_GREEN:
            c = Cube.GREEN;
            break;
            default:
            c = Cube.NULL;
            break;
        }
        return c;
    }

    public void DisableAllCubes()
    {
        for(int i = 0; i < numOfGrid; i++)
        {
            for(int j = 0; j < numOfGrid; j++)
            {
                cubes[i, j].SetActive(false);
            }
        }
    }

    public void EnableAllCubes()
    {
        for (int i = 0; i < numOfGrid; i++)
        {
            for (int j = 0; j < numOfGrid; j++)
            {
                cubes[i, j].SetActive(true);
            }
        }
    }

    public void ShowWinners()
    {
        UpdateWinners();
        StartCoroutine(CubeWinnerColor());
    }
    IEnumerator CubeWinnerColor()
    {
        for(int i = 0; i < numOfGrid; i++)
        {
            Color c = FindColor(winners[i % winners.Count]);
            var tempCubes = GetRow(cubes, i, c);
            HandleCubesColor(tempCubes, c, 0.1f);
            yield return new WaitForSeconds(0.1f * tempCubes.Count);
        }
    }

    private List<T> GetRow<T>(T[,] matrix, int row)
    {
        var colLength = matrix.GetLength(0);
        var colVector = new List<T>();

        for (var i = 0; i < colLength; i++)
            colVector.Add(matrix[i, row]);
        return colVector;
    }

    private List<Cube> GetRow(Cube[,] matrix, int row, Color c)
    {
        var colLength = matrix.GetLength(0);
        var colVector = new List<Cube>();

        for (var i = 0; i < colLength; i++)
        {
            if(matrix[i, row].GetInitColor() != c)
                colVector.Add(matrix[i, row]);
        }
        return colVector;
    }
}
