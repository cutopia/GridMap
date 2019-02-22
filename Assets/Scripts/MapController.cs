using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    [SerializeField] public int width = 16;
    [SerializeField] public int height = 12;
    [SerializeField] public GameObject mapTilePrefab;
    [SerializeField] public Color wallColor;
    [SerializeField] public Color pathColor;
    [SerializeField] public int smoothIterations = 5;
    MapUnit[,] gameMap;
    
	/// <summary>
    /// Create the map of tiles on start
    /// </summary>
	void Start () {
        InitializeGameMap();
        LinkMap();
        GenerateMap(1, 1);
        SmoothMap(smoothIterations);
	}
    
    /// <summary>
    /// Link the map units together so they know their neighbors for quick calculating.
    /// </summary>
    void LinkMap()
    {
        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                var tileInfo = gameMap[col, row];
                if (row - 1 >= 0)
                {
                    tileInfo.neighbors[0] = gameMap[col, row - 1];
                }
                if (row + 1 < height)
                {
                    tileInfo.neighbors[1] = gameMap[col, row + 1];
                }
                if (col - 1 >= 0)
                {
                    tileInfo.neighbors[2] = gameMap[col - 1, row];
                }
                if (col + 1 < width)
                {
                    tileInfo.neighbors[3] = gameMap[col + 1, row];
                }
            }
        }
    }
    
    /// <summary>
    /// Resets the numbers shown on the map representing move distances.
    /// </summary>
    public void ResetMap()
    {
        MapUnit[] allChildren = GetComponentsInChildren<MapUnit>();
        foreach (MapUnit m in allChildren)
        {
            m.ResetDistance();
        }
    }
    
    /// <summary>
    /// Reset the highlight color of all map units
    /// </summary>
    public void ResetHighlights()
    {
        MapUnit[] allChildren = GetComponentsInChildren<MapUnit>();
        foreach (MapUnit m in allChildren)
        {
            m.ResetHighlight();
        }
    }

    /// <summary>
    /// Initializes the game map.
    /// </summary>
    void InitializeGameMap()
    {
        gameMap = new MapUnit[width, height];
        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                GameObject tile = Instantiate(mapTilePrefab) as GameObject;
                tile.transform.position = new Vector3(col, row, 0);
                tile.GetComponent<SpriteRenderer>().color = wallColor;
                tile.transform.SetParent(transform);
                gameMap[col, row] = tile.GetComponent<MapUnit>();
            }
        }
    }
    
    /// <summary>
    /// Run a smoothing passes on the map after it is generated to 
    /// give it an airier feel.
    /// </summary>
    void SmoothMap(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (gameMap[col, row].wall)
                    {
                        if (GetWallNeighbors(col, row) == 1)
                        {
                            gameMap[col, row].wall = false;
                            gameMap[col, row].GetComponent<SpriteRenderer>().color = pathColor;
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Count number of neighbors that are walls. This version does not count where the map meets the outside world as a wall.
    /// </summary>
    /// <returns>The wall neighbors.</returns>
    /// <param name="locCol">Location col.</param>
    /// <param name="locRow">Location row.</param>
    int GetWallNeighbors(int locCol, int locRow)
    {
        int total = 0;
        for (int col = (locCol - 1 >= 0) ? locCol - 1 : 0; (col < locCol + 2) && (col < width); col++)
        {
            for (int row = (locRow - 1 >= 0) ? locRow - 1 : 0; (row < locRow + 2) && (row < height); row++)
            {
                if (row == locRow && col == locCol)
                {
                    continue;
                }
                if (gameMap[col, row].wall)
                {
                    total++;
                }
            }
        }
        return total;
    }

    /// <summary>
    /// Let's carve out a random maze map for the demo.
    /// </summary>
    /// <param name="col">Col.</param>
    /// <param name="row">Row.</param>
    void GenerateMap(int col, int row)
    {
        var tileInfo = gameMap[col, row];
        gameMap[col, row].GetComponent<SpriteRenderer>().color = pathColor;
        tileInfo.visited = true;
        tileInfo.wall = false;
        
        List<int[]> validDirs = GetValidDirs(col, row);
        while (validDirs.Count > 0)
        {
            int[] chosenDir = validDirs[Random.Range(0, validDirs.Count)];
            int nextCol = (int)((chosenDir[0] + col) * 0.5);
            int nextRow = (int)((chosenDir[1] + row) * 0.5);
            gameMap[nextCol, nextRow].wall = false;
            gameMap[nextCol, nextRow].GetComponent<SpriteRenderer>().color = pathColor;
            GenerateMap(chosenDir[0], chosenDir[1]);
            validDirs = GetValidDirs(col, row);
        }
    }

    /// <summary>
    /// find current valid dirs for maze generation.
    /// </summary>
    /// <returns>The valid dirs.</returns>
    /// <param name="col">Col.</param>
    /// <param name="row">Row.</param>
    List<int[]> GetValidDirs(int col, int row)
    {
        List<int[]> validDirs = new List<int[]>();
        if ((col - 2 >= 0) && !gameMap[col - 2, row].visited)
        {
            validDirs.Add(new int[] { col - 2, row });
        }
        if ((col + 2 < width) && !gameMap[col + 2, row].visited)
        {
            validDirs.Add(new int[] { col + 2, row });
        }
        if ((row - 2 >= 0) && !gameMap[col, row - 2].visited)
        {
            validDirs.Add(new int[] { col, row - 2 });
        }
        if ((row + 2 < height) && !gameMap[col, row + 2].visited)
        {
            validDirs.Add(new int[] { col, row + 2 });
        }
        return validDirs;
    }
}
