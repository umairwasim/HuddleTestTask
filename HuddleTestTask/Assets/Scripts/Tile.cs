using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 Pos => transform.position;

    public Block occupiedBlock { get; set; }
}
