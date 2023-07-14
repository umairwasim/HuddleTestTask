using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "", menuName = "Block Type Data")]
public class BlockTypeSO : ScriptableObject
{
    public List<BlockData> blockDataList;
}

[System.Serializable]
public class BlockData
{
    public Sprite sprite;
    public int value;
}