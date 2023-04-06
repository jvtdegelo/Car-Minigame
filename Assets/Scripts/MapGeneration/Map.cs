using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Map
{
    public int[,] map;

    public Vector2Int Return(Vector2Int position, int n)
    {
        if (n==0)
            return position;

        return Return(position+PreviousTile(position.y, position.x), n-1);
    }

    public int TileType(int i, int j)
    {
        bool isCell = map[i, j] != 0;
        if (!isCell) 
            return -1;
        var previous = PreviousTile(i, j); 
        var next = NextTile(i, j);
        var up = new Vector2Int(0, 1);
        var down = new Vector2Int(0, -1);
        var right = new Vector2Int(1, 0);
        var left = new Vector2Int(-1, 0);

        if (next.Equals(up) && previous.Equals(up))
            return -1;
        else if (next.Equals(up) && previous.Equals(down))
            return 6;    
        else if (next.Equals(up) && previous.Equals(left))
            return 2;    
        else if (next.Equals(up) && previous.Equals(right))
            return 4;    
        else if (next.Equals(down) && previous.Equals(up))
            return 6;    
        else if (next.Equals(down) && previous.Equals(down))
            return -1;    
        else if (next.Equals(down) && previous.Equals(left))
            return 3;    
        else if (next.Equals(down) && previous.Equals(right))
            return 1;    
        else if (next.Equals(left) && previous.Equals(up))
            return 2;    
        else if (next.Equals(left) && previous.Equals(down))
            return 3;    
        else if (next.Equals(left) && previous.Equals(left))
            return -1;    
        else if (next.Equals(left) && previous.Equals(right))
            return 5;    
        else if (next.Equals(right) && previous.Equals(up))
            return 4;    
        else if (next.Equals(right) && previous.Equals(down))
            return 1;    
        else if (next.Equals(right) && previous.Equals(left))
            return 5;
        else if (next.Equals(right) && previous.Equals(right))
            return -1;    

        return -1;
    }
    public Vector2Int PreviousTile(int i, int j)
    {
        bool isIni = (i==4) && (j==0);
        bool isEnd = (i==3) && (j==0);

        int currentBlock = map[i,j];

        int top = (i+1)<8 ? map[i+1,j]: 0;
        int down = (i-1)>=0 ? map[i-1,j]: 0;
        int right = (j+1)<8 ? map[i,j+1]: 0;
        int left = (j-1)>=0 ? map[i,j-1]: 0;

        Vector2Int previousBlock;
        if (isIni || down == currentBlock-1)
            previousBlock = new Vector2Int(0, -1);

        else if (top == currentBlock-1)
            previousBlock = new Vector2Int(0, 1);

        else if (right == currentBlock-1)
            previousBlock = new Vector2Int(1, 0);
        else
            previousBlock = new Vector2Int(-1, 0);

        return previousBlock;
    }
    public Vector2Int NextTile(int i, int j)
    {
        bool isIni = (i==4) && (j==0);
        bool isEnd = (i==3) && (j==0);

        int currentBlock = map[i,j];

        int top = (i+1)<8 ? map[i+1,j]: 0;
        int down = (i-1)>=0 ? map[i-1,j]: 0;
        int right = (j+1)<8 ? map[i,j+1]: 0;
        int left = (j-1)>=0 ? map[i,j-1]: 0;

        Vector2Int nextBlock;
        if (isEnd || top == currentBlock+1)
            nextBlock = new Vector2Int(0, 1);

        else if (down == currentBlock+1)
            nextBlock = new Vector2Int(0, -1);

        else if (right == currentBlock+1)
            nextBlock = new Vector2Int(1, 0);

        else
            nextBlock = new Vector2Int(-1, 0);

        return nextBlock;
    }

    public bool IsValidPosition(int i, int j)
    {
        return map[i, j] != 0;
    }

    public Vector2Int WallDirectionFromTrack(int i, int j)
    {
        Vector2Int direction = NextTile(i, j);
        if (direction.x == 1 && direction.y == 0)
            return new Vector2Int(0,-1);

        else if (direction.x == -1 && direction.y == 0)
            return new Vector2Int(0, 1);

        else if (direction.x == 0 && direction.y == 1)
            return new Vector2Int(1,0);

        else
            return new Vector2Int(-1,0);
    }

    public Vector2Int DirectionNextTurn(int i, int j)
    {
        Vector2Int direction = NextTile(i, j);
        int tilesInDirection = 0;
        Vector2Int nextTileDirection = direction;
        while (nextTileDirection != null && nextTileDirection.Equals(direction))
        {
            i += direction.y;
            j += direction.x;
            if (i<0 || i>=8 || j<0 || j>=8)
                break;
            nextTileDirection = NextTile(i, j);
            tilesInDirection+=1;
            
        }
        return tilesInDirection*direction;
    }

    public int NextTurnSide(int i, int j)
    {
        Vector2Int distance = DirectionNextTurn(i, j);

        int turnI = i + distance.y;
        int turnJ = j + distance.x;

        Vector2Int nextTurnDirection = NextTile(turnI, turnJ);

        if (distance.x > 0 && distance.y == 0)
            return nextTurnDirection.y<0? 1: -1;

        else if(distance.y > 0 && distance.x == 0)
            return nextTurnDirection.x>0? 1: -1;

        else if(distance.x < 0 && distance.y == 0)
            return nextTurnDirection.y>0? 1: -1;        
        else
            return nextTurnDirection.x<0? 1: -1;
        
    }

    public int NextTurnTurnSide(int i, int j)
    {
        Vector2Int distance = DirectionNextTurn(i, j);

        int turnI = i + distance.y;
        int turnJ = j + distance.x;

        Vector2Int nextTurnDistance = DirectionNextTurn(turnI, turnJ);

        int turnTurnI = turnI + nextTurnDistance.y;
        int turnTurnJ = turnJ + nextTurnDistance.x;

        Vector2Int nextTurnTurnDirection = NextTile(turnTurnI, turnTurnJ);

        if (nextTurnDistance.x > 0 && nextTurnDistance.y == 0)
            return nextTurnTurnDirection.y<0? 1: -1;

        else if(nextTurnDistance.y > 0 && nextTurnDistance.x == 0)
            return nextTurnTurnDirection.x>0? 1: -1;

        else if(nextTurnDistance.x < 0 && nextTurnDistance.y == 0)
            return nextTurnTurnDirection.y>0? 1: -1;        
        else
            return nextTurnTurnDirection.x<0? 1: -1;
        
    }

    public Vector2Int NextTurnLength(int i, int j)
    {
        Vector2Int distance = DirectionNextTurn(i, j);

        int turnI = i + distance.y;
        int turnJ = j + distance.x;

        Vector2Int nextTurnDistance = DirectionNextTurn(turnI, turnJ);

        return nextTurnDistance;        
    }


}
