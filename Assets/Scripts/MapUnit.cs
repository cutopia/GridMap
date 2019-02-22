using UnityEngine;

public class MapUnit : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteHighlight;
    [SerializeField] public Color startHighlightColor;
    [SerializeField] public Color destinationHighlightColor;
    private const int INFINITY_DISTANCE = 100;
    public GameObject[] neighbors = new GameObject[4];
    public bool visited = false;
    public bool wall = true;
    public int col;
    public int row;
    public int startingMoves = 7;
    public int currentDistance = INFINITY_DISTANCE;

    public void OnMouseDown()
    {
        Propagate(startingMoves);
    }

    public void OnMouseOver()
    {
        Debug.Log("OnMouseOVer");
        if (currentDistance == INFINITY_DISTANCE)
        {
            spriteHighlight.color = startHighlightColor;
        }
        else
        {
            spriteHighlight.color = destinationHighlightColor;
        }
    }

    public void OnMouseExit()
    {
        spriteHighlight.color = Color.clear;
    }

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
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != null)
                {
                    neighbors[i].GetComponent<MapUnit>().Propagate(movesLeft - 1);
                }
            }
        }
    }
}
