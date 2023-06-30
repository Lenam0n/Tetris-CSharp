using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class Gamepiece : MonoBehaviour
{
    public board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    public bool gameover { get; set; } 

    private float stepTime;
    private float lockTime;

    public void Initialize(board Board, Vector3Int position, TetrominoData data)
    {
        this.board = Board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepTime;
        this.lockTime = 0f;

        if(this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private void Update()
    {
        this.lockTime += Time.deltaTime;
        this.gameover = gameover;
        this.board.Clear(this);

        if (!this.gameover) { 

            if(Input.GetKeyDown(KeyCode.A)) 
            {
                Move(Vector2Int.left);
             }
            else if(Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Rotate(1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Rotate(-1);
            }

            if (Time.time >= this.stepTime)
            {
                Step();
            }
        }

        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.clearLines();
        if (board.IsValidPos(this,new Vector3Int(0,7)))
        {
            this.board.spawnPiece();
        }
        else { board.GameOver(); }
        
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPos = this.position;
        newPos.x += translation.x;
        newPos.y += translation.y;

        bool valid = this.board.IsValidPos(this,newPos);

        if( valid )
        {
            this.position = newPos;
            this.lockTime = 0f;
        }

        return valid;
    }
    private void Rotate(int direction)
    {
        int original_Rotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction , 0 , 4);

            ApplyRotationMatrix(direction);

            if (!testWallKicks(rotationIndex,direction))
            {
                this.rotationIndex = original_Rotation;
                ApplyRotationMatrix(-direction);
            }

        
    }
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    



    private bool testWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = getWallkickIndex(rotationIndex, rotationDirection);
        for(int i = 0; i < this.data.wallkicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallkicks[wallKickIndex, i];
            if (Move(translation)){ return true;}
        }
        return false;
    }

    private int getWallkickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0)
        {
            wallKickIndex --;
        }
        return Wrap(wallKickIndex , 0 , this.data.wallkicks.GetLength(0));

    }


    private int Wrap(int input, int min, int max)
    {
        if (input < min){ return max - (min - input) % (max - min); }
        else { return min + (input - min) % (max - min); }
    }
  
}
