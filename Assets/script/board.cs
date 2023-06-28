using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Tilemap tilemap { get; private set; }
    public Vector3Int spawnPosition;
    public Gamepiece activePiece { get; private set; }
    public Vector2Int BoardSize = new Vector2Int(10, 20);
    public RectInt Bounds {
        get 
        {
            Vector2Int posi = new Vector2Int(-this.BoardSize.x / 2, -this.BoardSize.y / 2);
            return new RectInt(posi, this.BoardSize);
        }

    } 

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Gamepiece>(); 

        for(int i = 0; i < tetrominos.Length; i++)
        {
            this.tetrominos[i].Initzalize();
        }  
    }

    private void Start()
    {
        spawnPiece();
    }

    public void spawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = this.tetrominos[random];
        this.activePiece.Initialize(this, spawnPosition, data);
        Set(this.activePiece);
    }

    public void Set(Gamepiece piece)
    {
        for(int i = 0;i < piece.cells.Length;i++) 
        {
            Vector3Int tileposition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tileposition,piece.data.tile);
        }
    }
    public void Clear(Gamepiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tileposition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tileposition, null);
        }
    }

    public bool IsValidPos(Gamepiece piece, Vector3Int pos)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length ; i++) 
        {
            Vector3Int tilePos = piece.cells[i] + pos;

            if (!bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }


            if (this.tilemap.HasTile(tilePos))
            {
                return false;
            }
                
            

        }
        return true;
    }

    public void clearLines()
    {
        RectInt Bounds = this.Bounds;
        int row = Bounds.yMin;

        while (row < Bounds.yMax) 
        {
            if (isLineFull(row)) { LineClear(row); }
            else{ row++; }
        }

    }

    private void LineClear(int row)
    {
        RectInt Bounds = this.Bounds;
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(pos, null);
        }

        while (row < Bounds.yMax)
        {
            for (int col = Bounds.xMin; col < Bounds.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, row+1, 0);
                TileBase above = this.tilemap.GetTile(pos);

                pos = new Vector3Int(col,row,0);
                this.tilemap.SetTile(pos, above);
            }
            row++;
        }
    }

    private bool isLineFull(int row)
    {
        RectInt Bounds = this.Bounds;
        for(int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);

            if(!this.tilemap.HasTile(pos)) { return false; }
        }
        return true;

    }
}
