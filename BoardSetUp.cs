using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetUp : MonoBehaviour
{
    private System.Random random = new System.Random();
    private Stack<System.Tuple<int, int>> coordinates =
        new Stack<System.Tuple<int, int>>();// holds coords to revisit.
    private Transform[,] grid;
    private int horiz = 0;
    private int vert = 0;

    public GameObject key; //key prefab
    public GameObject pickUpManager; // holds key prefabs  

    Transform[,] CreateGrid()
    {
        Transform[,] grid = new Transform[14, 14];

        int x = 0;
        int y = 0;

        //adds cells to the grid array.
        foreach (Transform child in transform)
        {
            grid[x, y] = child;

            x += 1;
            if (x == 14)
            {
                x = 0;
                y += 1;
            }
        }
        return grid;
    }

    void GenerateMazePath()
    {
        //runs until every cell has been iterated over twice.
        //once toward the finish and then back to the start.
        while (coordinates.Count > 0)
        {
            PickRandomDirection();
            System.Tuple<int, int> previousCoord = coordinates.Pop();
            horiz = previousCoord.Item1;
            vert = previousCoord.Item2;
        }
    }

    void PickRandomDirection()
    {
        char[] possibleMoves = new char[] { 'N', 'E', 'S', 'W' };
        List<char> notValidMoves = new List<char>();

        while (true)
        {
            int randomDirection = random.Next(0, possibleMoves.Length);//pick random direction

            if (notValidMoves.Count >= 4)
            {
                break;
            }
            
            if (!notValidMoves.Contains(possibleMoves[randomDirection]))
            {
                if (vert > 0 && possibleMoves[randomDirection] == 'N' && grid[horiz, vert - 1].Find("Visited").gameObject.activeSelf)
                {
                    // Deactivate the visited object in cell that is being moved to.
                    grid[horiz, vert - 1].Find("Visited").gameObject.SetActive(false);

                    // deactivate wall from current cell and cell being moved to.
                    grid[horiz, vert].Find("NorthWall").gameObject.SetActive(false);
                    grid[horiz, vert - 1].Find("SouthWall").gameObject.SetActive(false);

                    vert -= 1;

                    // Add coords of cell that is being moved to to the stack.
                    coordinates.Push(System.Tuple.Create(horiz, vert));
                    notValidMoves.Clear(); //empty notValidMoves for next iteration.
                }
                else if (horiz < 13 && possibleMoves[randomDirection] == 'E' && grid[horiz + 1, vert].Find("Visited").gameObject.activeSelf)
                {
                    grid[horiz + 1, vert].Find("Visited").gameObject.SetActive(false);

                    grid[horiz, vert].Find("EastWall").gameObject.SetActive(false);
                    grid[horiz + 1, vert].Find("WestWall").gameObject.SetActive(false);
                    
                    horiz += 1;

                    coordinates.Push(System.Tuple.Create(horiz, vert));
                    notValidMoves.Clear();
                }
                else if (vert < 13 && possibleMoves[randomDirection] == 'S' && grid[horiz, vert + 1].Find("Visited").gameObject.activeSelf)
                {
                    grid[horiz, vert + 1].Find("Visited").gameObject.SetActive(false);

                    grid[horiz, vert].Find("SouthWall").gameObject.SetActive(false);
                    grid[horiz, vert + 1].Find("NorthWall").gameObject.SetActive(false);

                    vert += 1;

                    coordinates.Push(System.Tuple.Create(horiz, vert));
                    notValidMoves.Clear();
                }
                else if (horiz > 0 && possibleMoves[randomDirection] == 'W' && grid[horiz - 1, vert].Find("Visited").gameObject.activeSelf)
                {
                    grid[horiz - 1, vert].Find("Visited").gameObject.SetActive(false);
                    grid[horiz, vert].Find("WestWall").gameObject.SetActive(false);

                    grid[horiz - 1, vert].Find("EastWall").gameObject.SetActive(false);
                    horiz -= 1;

                    coordinates.Push(System.Tuple.Create(horiz, vert));
                    notValidMoves.Clear();
                }
                else
                {
                    notValidMoves.Add(possibleMoves[randomDirection]);
                }
            }
        }
    }

    //randomly places the key pick up prefab object in the maze.
    void PlaceKeys()
    {
        for (int i = 0; i <= 4; i++)
        {
            int randomHoriz = random.Next(1, grid.GetLength(0));
            int randomVert = random.Next(1, grid.GetLength(0));

            Vector3 cellPosition = grid[randomHoriz, randomVert].gameObject.transform.position;

            GameObject pickUp = Instantiate(key, new Vector3(cellPosition.x, 0.5f, cellPosition.z), Quaternion.identity);
            pickUp.transform.parent = pickUpManager.transform; // set the pickup to be a child of the manager.
        }
    }

    void SetUp()
    {
        // creates grid
        grid = CreateGrid();

        coordinates.Push(System.Tuple.Create(horiz, vert));
        grid[horiz, vert].Find("Visited").gameObject.SetActive(false);

        GenerateMazePath();

        PlaceKeys();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }
}
