using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int blockValue;
    public bool isMerging;

    public Tile myCurrentTile;
    public Block mergingBlock;

    public Vector2 Pos => transform.position;

    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private TextMeshPro blockText;

    public void Init(BlockType type)
    {
        blockValue = type.value;
        blockSpriteRenderer.sprite = type.sprite;
        blockText.text = type.value.ToString();
    }

    public void Init(BlockData data)
    {
        blockValue = data.value;
        blockSpriteRenderer.sprite = data.sprite;
        blockText.text = data.value.ToString();
    }

    // A merge can only happen if both blocks have same value,
    // and merging block is null,
    // and the are not already merging

    public void SetBlockOnTile(Tile newTile)
    {
        //Check if the current Tile is not already set but has its block occupied.
        //If so, make it null so that we can set newTile to currentTile and
        //now this block should be the currentTile's occupied block/set this block
        //as currentTile's occupied block
        if (myCurrentTile != null)
            myCurrentTile.occupiedBlock = null;

        myCurrentTile = newTile;

        //set this block as my current tile's occupied block
        myCurrentTile.occupiedBlock = this;
    }

    public bool CanMerge(int value)
    {
        return value == blockValue && mergingBlock == null && !isMerging;
    }

    public void MergeBlock(Block baseBlock)
    {
        //set our mergingBlock equal to block to merge with (base block)
        mergingBlock = baseBlock;

        //Release currentTile occupied block
        myCurrentTile.occupiedBlock = null;

        //set the base block isMerging to true so that no other blocks can merge into it 
        //that means only merging block and base block can merge 
        baseBlock.isMerging = true;
    }

}
