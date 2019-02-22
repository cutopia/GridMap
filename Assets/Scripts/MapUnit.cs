using UnityEngine;

/// <summary>
/// MapUnits assume that their parent is a map controller.
/// </summary>
public class MapUnit : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteHighlight;
    [SerializeField] public Color startHighlightColor;
    [SerializeField] public Color destinationHighlightColor;
    [SerializeField] public int startingMoves = 7;
    private const int INFINITY_DISTANCE = 100;
    public MapUnit[] neighbors = new MapUnit[4];
    public bool visited = false;
    public bool wall = true;
    public int currentDistance = INFINITY_DISTANCE;

    /// <summary>
    /// when mouse button press happens, if this tile is not part of an
    /// existing distance interaction, then we start a new distance
    /// interaction.
    /// </summary>
    public void OnMouseDown()
    {
        if (currentDistance == INFINITY_DISTANCE)
        {
            GetComponentInParent<MapController>().ResetMap();
            Propagate(startingMoves);
        }
    }

    /// <summary>
    /// when mouse enters this tile, we either set a mouse over highlight
    /// if the user has not selected a starting tile yet or we highlight the
    /// path from this tile if they do have a starting tile in range.
    /// </summary>
    public void OnMouseEnter()
    {
        GetComponentInParent<MapController>().ResetHighlights();
        if (currentDistance == INFINITY_DISTANCE)
        {
            spriteHighlight.color = startHighlightColor;
        }
        else
        {
            HighlightPath();
        }
    }
    
    /// <summary>
    /// Sets a highlight color on each tile involved in pathing to this one.
    /// </summary>
    public void HighlightPath()
    {
        if (wall == true || currentDistance == INFINITY_DISTANCE)
        {
            return;
        }
        spriteHighlight.color = destinationHighlightColor;
        foreach (MapUnit neighbor in neighbors)
        {
            if (neighbor == null)
            {
                continue;
            }
            if (neighbor.currentDistance == (currentDistance - 1))
            {
                neighbor.HighlightPath();
                break;
            }
        }
    }

    /// <summary>
    /// clear the number representing distance from most recently clicked tile
    /// </summary>
    public void ResetDistance()
    {
        currentDistance = INFINITY_DISTANCE;
        gameObject.GetComponentInChildren<TextMesh>().text = "";
    }
    
    /// <summary>
    /// Resets the highlight.
    /// </summary>
    public void ResetHighlight()
    {
        spriteHighlight.color = Color.clear;
    }

    /// <summary>
    /// when an initial tile is selected, we show the number of moves to each tile in range.
    /// I opted to show all the moves in range right away as a prompt to help user know where
    /// to move the mouse, but if we wanted to only show the moves numbers for the active path 
    /// we could just wait and not set the text mesh until the highlights are applied.
    /// </summary>
    /// <param name="movesLeft">Moves left.</param>
    public void Propagate(int movesLeft)
    {
        if (wall == true)
        {
            return;
        }
        if (currentDistance < (startingMoves - movesLeft))
        {
            return;
        }
        if (movesLeft > 0)
        {
            currentDistance = startingMoves - movesLeft;
            gameObject.GetComponentInChildren<TextMesh>().text = " " + currentDistance.ToString();
            foreach (MapUnit neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    neighbor.Propagate(movesLeft - 1);
                }
            }
        }
    }
}
