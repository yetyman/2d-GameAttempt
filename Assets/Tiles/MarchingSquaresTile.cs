using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Tiles/Marching Squares Tile")]
public class MarchingSquaresTile : RuleTile<MarchingSquaresTile.Neighbor> {

    public GameObject o0000;
    public GameObject o0001;
    public GameObject o0011;
    public GameObject o0111;
    public GameObject o1111;
    public GameObject o0101;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public static Dictionary<int, int> neighbors = new Dictionary<int, int>();
    static Dictionary<int, Vector3Int> neighboring = new Dictionary<int, Vector3Int>()
    {
        {0,new Vector3Int( 0, 1,0)},
        {1,new Vector3Int( 1, 1,0)},
        {2,new Vector3Int( 1, 0,0)},
        {3,new Vector3Int( 1,-1,0)},
        {4,new Vector3Int( 0,-1,0)},
        {5,new Vector3Int(-1,-1,0)},
        {6,new Vector3Int(-1, 0,0)},
        {7,new Vector3Int(-1, 1,0)},
    };

    static Dictionary<int, GameObject> squarePatterns = new Dictionary<int, GameObject>();
    static ITilemap CachedMap = null; 
    static Tilemap CachedBehaviour = null; 
    static Transform CachedTilemapLocation = null;
    static Vector3Int CachedTowardCamera;
    static List<byte> corners = new List<byte>();
    static Dictionary<Vector3Int,Dictionary<int,int>> extCorners = new Dictionary<Vector3Int, Dictionary<int, int>>();
    static Vector3[] cornerLocs = new Vector3[] {
        new Vector3Int(1,1,0),
        new Vector3Int(-1,1,0),
        new Vector3Int(-1,-1,0),
        new Vector3Int(1,-1,0),
    };
    static Vector3 CachedCellSize;
    static Vector3 center = new Vector3(.5f, .5f, 0);
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    {

        Debug.Log($"I start {position}");
        if (!tilemap.Equals(CachedMap))
            squarePatterns.Clear();
        if (!squarePatterns.Any())
        {
            foreach(var kv in neighboring)
            {
                if (!extCorners.ContainsKey(kv.Value))
                    extCorners.Add(kv.Value, new Dictionary<int, int>());
            }
            CachedMap = tilemap;
            CachedBehaviour = tilemap.GetComponent<Tilemap>();
            CachedTilemapLocation = tilemap.GetComponent<Transform>();
            CachedCellSize = CachedBehaviour.cellSize;
            CachedTowardCamera = new Vector3Int(0, 0, CachedBehaviour.transform.position.z - Camera.main.transform.position.z < 0 ? 1 : -1);
            Debug.Log($"I found things for this tilemap");
            squarePatterns.Add(0b_0000_0000, o0000);
            squarePatterns.Add(0b_0000_0001, o0001);
            squarePatterns.Add(0b_0000_0011, o0011);
            squarePatterns.Add(0b_0000_0111, o0111);
            squarePatterns.Add(0b_0000_1111, o1111);
            squarePatterns.Add(0b_0000_0101, o0101);
        }
        neighbors.Clear();


        //bit arr of neighbors
        //7 0 1
        //6   2
        //5 4 3

        byte myBytes = 0;
        byte neighborByte = 1;

        foreach (var adj in neighboring)
        {
            var pos = adj.Value + position;
            var posTile = CachedMap.GetTile(pos);
            if (posTile != null)
                myBytes += (byte)(neighborByte << adj.Key);
        }
        Debug.Log($"at {position} but really {CachedBehaviour.GetCellCenterLocal(position)}\n{Convert.ToString(myBytes, 2)} (should be anything a byte can be 0-255)");

        //made bit arr
        //now make four corners for marching squares shape resolution

        //7 0  0 1
        //6 -  - 2
        //
        //6 -  - 2
        //5 4  4 3
        corners.Clear();
        foreach (var kv in extCorners)
            extCorners[kv.Key].Clear();
        for (int i = 0, b = 0; i < 4; i++, b += 2)//0,2,4,6
        {
            corners.Add( (byte)(((myBytes << b) | (byte)((uint)myBytes >> (8 - b))) & 0b_0000_0111)); //get surrounding //looping left shift... cast to uint so that right shift fills with 0.
            corners[i] |= 0b_0000_1000;//include self
            corners[i] = (byte)(((corners[i] << 3) | (byte)((uint)corners[i] >> (1))) & 0b_0000_1111); //rotate back 1
            corners[i] %= 16;

            //0 1 -> 3 0
            //c 2    2 1
            if (corners[i] == 14)
                corners[i] = 15;
            //above
            //  x
            //0 1
            //c 1
            if (corners[i] == 5 || corners[i] == 7)
            {
                var destinationSquare = neighboring[(8-2 * i) % 8];
                var destinationSubSquare = (i + 3) % 4;
                var destinationRotation = LoopingRightShift(1, 4, i + 1);
                Debug.Log($"placing extra corner at {destinationSquare} in subsquare {destinationSubSquare} with the following orientation\n{destinationRotation % 2}{(destinationRotation >> 1) % 2}\n{(destinationRotation >> 2) % 2}{(destinationRotation >> 3) % 2}");
                extCorners[destinationSquare].Add(destinationSubSquare, destinationRotation);
            }
            //to right
            //1 1 x
            //c 0
            if (corners[i] == 5 || corners[i] == 13)
                extCorners[neighboring[(8 - 2 * i + 2) % 8]].Add((i + 1) % 4, LoopingRightShift(4, 4, i+1));
            corners[i] = (byte)(((corners[i] << (4 - i)) | (byte)((uint)corners[i] >> i)) & 0b_0000_1111); //rotate  
            Debug.Log($"corner {i} is {Convert.ToString(corners[i])} (should be 0-15)");
        }

        //now each corner is rotated to the same orientation relative to the whole block.
        //every square spatially is represented as binary values with bit indexes as below
        //3 0  3 0
        //2 1  2 1
        //
        //3 0  3 0
        //2 1  2 1

        //the order of squares in the corners arr is currently 
        //1 0
        //2 3
        //corners = {0,1,2,3}
        MakeCorners(position, corners);
        foreach (var kv in extCorners)
            foreach (var vkv in kv.Value)
                MakeCorner(kv.Key+position, vkv.Key, vkv.Value, false);
        
        //alright, time for the fun part. figure out which game object each square relates to.

        Debug.Log($"I finish {position}");

        //i think the math is all there so lets see what happens.
        //if this works on my first try, i'll shit my pants.
        return base.StartUp(position, tilemap, instantiatedGameObject);
    }
    private byte LoopingLeftShift(byte val, int loopLength, int shift)
    {
        shift %= loopLength;
        return (byte)(((val << shift) | (byte)((uint)val >> (loopLength - shift))) % (1<<loopLength));
    }
    private byte LoopingRightShift(byte val, int loopLength, int shift)
    {
        shift %= loopLength;
        return (byte)(((val << (loopLength - shift)) | (byte)((uint)val >> shift)) % (1 << loopLength));
    }
    private void MakeCorners(Vector3Int position, List<byte> corners)
    {
        for (int i = 0; i < 4; i++)
            MakeCorner(position, (byte)i, corners[i]);
    }
    private void MakeCorner(Vector3Int position, int i, int corner, bool inside = true)
    {
        GameObject go;
        int r = 0;
        while (!squarePatterns.ContainsKey(corner))
        {
            r++;
            //corner = LoopingLeftShift(corner, 4, 1);
            corner = (byte)(((corner << 1) | (byte)((uint)corner >> (4 - 1))) & 0b_0000_1111);
        }
        Debug.Log($"{position} is a {Convert.ToString(corner, 2)}");
        go = squarePatterns[corner];
        if (go != null)
        {
            string name = $"3d{ position.ToString()}({i})";
            string partial = $"({ Convert.ToString(corners[i], 2)})";
            //don't like this for efficiency...
            if (CachedBehaviour.transform.Find(name+partial) == null)
                foreach (var c in CachedBehaviour.transform)
                    if (c is Transform t)
                        if (t.name.Contains(name)) DestroyImmediate(t.gameObject);

            name += partial;
            if (CachedTilemapLocation.Find(name) == null)
            {
                //instantiate the game object at tile position plus the right transform to center on the correct portion of the square, rotate by r*90
                go = Instantiate<GameObject>(go, CachedBehaviour.transform);
                go.transform.localPosition = CachedBehaviour.GetCellCenterLocal(position) + Vector3.Scale(CachedCellSize, (center - CachedBehaviour.tileAnchor + .25f * cornerLocs[i]));
                go.transform.localRotation = Quaternion.AngleAxis(-r * 90 + (inside ? 180 : 0), CachedTowardCamera);
                go.transform.localScale = new Vector3(.5f * CachedCellSize.x, .5f * CachedCellSize.y, .5f * CachedCellSize.y); //assume GameObject is 1 unit scale. because standards. depending on usage, cachedCellSize may shift to a vec3 of the smallest aspect of the CachedCellSize // just aspect ratio things
                go.name = name;
            }
        }
    }
}
