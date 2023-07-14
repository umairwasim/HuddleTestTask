using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public enum GameState
{
    GenerateGrid,
    SpawningBlocks,
    WaitForInput,
    Moving,
    Win,
    Lose,
    Play
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState gameState;

    public event Action OnGameWon;
    public event Action OnGameLost;
    public event Action OnMoveCounter;

    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    [SerializeField] private Tile tilePrefab;               //Tile is a container for block
    [SerializeField] private Block blockPrefab;             //Actual block prefab with number and color
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private BlockTypeSO blockTypeSO;
    [SerializeField] private float snapDuration = 0.2f;
    [SerializeField] private int winValue = 2048;

    private readonly List<Tile> tiles = new();
    private readonly List<Block> blocks = new();

    private int round = 0;

    private readonly float boardOffset = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    #region Getter Functions

    private BlockData GetBlockDataByValue(int value)
    {
        foreach (BlockData data in blockTypeSO.blockDataList)
        {
            if (data.value == value)
                return data;
        }
        return null;
    }

    private Tile GetTileAtPosition(Vector2 position)
    {
        return tiles.FirstOrDefault(t => t.Pos == position);
    }

    #endregion

    public void ChangeGameState(GameState currentState)
    {
        gameState = currentState;

        switch (currentState)
        {
            case GameState.GenerateGrid:
                GenerateBoardAndTiles();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round);
                break;
            case GameState.WaitForInput:
                WaitForInput();
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                GameWon();
                break;
            case GameState.Lose:
                GameLost();
                break;
        }
    }

    private void WaitForInput()
    {
        //Do nothing and show waiting UI
    }

    private void GameLost()
    {
        OnGameLost?.Invoke();
    }

    private void GameWon()
    {
        OnGameWon?.Invoke();
    }

    private void Start()
    {
        ChangeGameState(GameState.WaitForInput);
    }

    private void Update()
    {
        if (gameState != GameState.WaitForInput)
            return;

        //Arrow Keys Input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ShiftBlocks(Vector2.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ShiftBlocks(Vector2.right);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ShiftBlocks(Vector2.up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ShiftBlocks(Vector2.down);
    }

    #region Generate Board

    private void GenerateBoardAndTiles()
    {
        //center board value, store in a vector
        var center = new Vector2(width * 0.5f - boardOffset, height * 0.5f - boardOffset);

        //Instantiate board at the center of the screen, 
        var board = Instantiate(boardPrefab, center, Quaternion.identity);

        //define board size equal to our grid width and height
        board.size = new Vector2(width, height);

        //center the camera , -10 along z axis
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        #region Tiles Instantiation

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity);
                tiles.Add(tile);
            }
        }

        #endregion

        ChangeGameState(GameState.SpawningBlocks);
    }

    #endregion

    #region Shift Block

    private void SpawnBlock(Tile freeTile, int value)
    {
 

        //Spawn given number of blocks at our freeTile position
        var spawnBlock = Instantiate(blockPrefab, freeTile.Pos, Quaternion.identity);

        //Add spawned block to the list of blocks
        blocks.Add(spawnBlock);

        //Set the relevant freeTile as Tile of the spawned block
        spawnBlock.SetBlockOnTile(freeTile);

        //Set the value of the spawned block 
        // spawnBlock.Init(GetBlockByValue(value));

        spawnBlock.Init(GetBlockDataByValue(value));
    }

    private void SpawnBlocks(int numberOfBlocks)
    {
        numberOfBlocks = round == 0 ? numberOfBlocks = 2 : numberOfBlocks = 1;

        //Randomly choose tiles with no occupied block 
        var freeTiles = tiles
            .Where(t => t.occupiedBlock == null)
            .OrderBy(t => Random.value)
            .ToList();

        foreach (var freeTile in freeTiles.Take(numberOfBlocks))
        {
            SpawnBlock(freeTile, Random.value > 0.8f ? 4 : 2);
        }

        if (freeTiles.Count() < 1)
        {
            //GAME OVER
            ChangeGameState(GameState.Lose);

            //TODO: 
            return;
        }

        //Increment round at the very end so for the next iteration, it is incremented
        round++;

        ChangeGameState(blocks.Any(b => b.blockValue == winValue) ? GameState.Win : GameState.WaitForInput);
    }

    #endregion

    #region Shift Block

    void ShiftBlocks(Vector2 direction)
    {
        ChangeGameState(GameState.Moving);

        //Increment Moves counter 
        OnMoveCounter?.Invoke();

        #region Order Blocks
        //order the blocks by X and then by Y
        var orderedBlocks = blocks
            .OrderBy(b => b.Pos.x)
            .ThenBy(b => b.Pos.y)
            .ToList();

        //For opposite direction Inputs
        if (direction == Vector2.right || direction == Vector2.up)
        {
            orderedBlocks.Reverse();
        }
        #endregion

        #region Shift Blocks
        foreach (Block newBlock in orderedBlocks)
        {
            //store block's current tile reference
            Tile currentTile = newBlock.myCurrentTile;
            do
            {
                //set block's own tile as current tile
                newBlock.SetBlockOnTile(currentTile);

                //look for next possible tile at position (which is our current tile's position
                //and the direction it is moving in)
                Tile nextPossibleTile = GetTileAtPosition(currentTile.Pos + direction);

                //if we have a possible next tile
                if (nextPossibleTile != null)
                {
                    //if next possible tile's occupied block is already occupied and
                    //its value is equal to our block value. Check if we can merge into it
                    if (nextPossibleTile.occupiedBlock != null &&
                        nextPossibleTile.occupiedBlock.CanMerge(newBlock.blockValue))
                    {
                        //this block's merging block is equal to nextPossibleTile's occupied block
                        newBlock.MergeBlock(nextPossibleTile.occupiedBlock);
                    }
                    //if next possible tile's occupied block is not already occupied 
                    else if (nextPossibleTile.occupiedBlock == null)
                    {
                        //set our current tile to next possible tile
                        currentTile = nextPossibleTile;
                    }
                }

                
            }
            //loop through until my currentTile and block's currentTile aren't the same 
            while (currentTile != newBlock.myCurrentTile);

        }

        #endregion

        #region Moving Blocks

        var sequence = DOTween.Sequence();

        foreach (Block moveBlock in orderedBlocks)
        {
            //if the block's merging block is occupied,
            //our move position will be block's merging block Tile's position
            //else our position will be block's tile position
            var movePosition = moveBlock.mergingBlock != null ?
                moveBlock.mergingBlock.myCurrentTile.Pos : moveBlock.myCurrentTile.Pos;

            // (move block) set block's position to it's current tile position
            sequence.Insert(0, moveBlock.transform.DOMove(movePosition, snapDuration));
            sequence.OnComplete(() =>
           {
               foreach (var block in orderedBlocks.Where(b => b.mergingBlock != null))
               {
                   MergeBlock(block.mergingBlock, block);
               }
           });

        }
        ChangeGameState(GameState.SpawningBlocks);

        #endregion
    }

    #endregion

    #region Merge Block

    void MergeBlock(Block baseBlock, Block mergingBlock)
    {
        //spawn a new block at base block's tile and base block value multiplied by 2 (since merging the values too)
        SpawnBlock(baseBlock.myCurrentTile, baseBlock.blockValue * 2);

        //Destroy both base and merging blocks after spawning a new merged block 
        DestroyBlock(baseBlock, 0.0001f);
        DestroyBlock(mergingBlock, 0.0001f);
    }

    #endregion

    #region Destroy Block

    void DestroyBlock(Block block, float delay)
    {
        //remove from the list of blocks
        blocks.Remove(block);

        //then destroy its game object
        Destroy(block.gameObject, delay);
    }

    #endregion
}




