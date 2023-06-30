using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ghost : MonoBehaviour
{
    public Tile tile;
    public board board;
    public Gamepiece piece;

    public Tilemap tilemap {get;set;}
    public Vector3Int[] cells { get; set; }
    public Vector3Int pos { get; set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tileposition = this.cells[i] + this.pos;
            this.tilemap.SetTile(tileposition, null);
        }
    }


    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.piece.cells[i];
        }

    }

    private void Drop()
    {
        Vector3Int pos = this.piece.position;
        int current_row = pos.y;
        int bottom = -this.board.BoardSize.y / 2 - 1;

        this.board.Clear(this.piece);

        for(int row = current_row; row >= bottom; row-- )
        {
            pos.y = row;
            if (this.board.IsValidPos(piece, pos)){ this.pos = pos; }
            else{ break;}
        }

        this.board.Set(this.piece);

    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tileposition = this.cells[i] + this.pos;
            this.tilemap.SetTile(tileposition, this.tile);
        }
    }
}

