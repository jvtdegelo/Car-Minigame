using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RacingTrack : MonoBehaviour
{
    public  Map _map;
    private MapLoader _mapLoader;

    private int[] dx = {0, 0,-1, 1};
    private int[] dy = {1,-1, 0, 0};
    
    private void Awake()
    {
        _mapLoader = GetComponent<MapLoader>();
    }
    private void Start()
    {
        // int[] values = {31,31,31,31};
        // _map = _mapLoader.loadSpecific(values);
        do{
            _map = _mapLoader.loadRandom();
        } while(!_map.PreviousTile(3,0).Equals(new Vector2Int(0, -1)) || !_map.NextTile(4,0).Equals(new Vector2Int(0, 1)));

        SetupTiles();
    }
    private void SetupTiles()
    {
        var roadLevel = GetComponentsInChildren<Tilemap>()[0];
        var barrierLevel = GetComponentsInChildren<Tilemap>()[1];
        var decorationLevel = GetComponentsInChildren<Tilemap>()[3];
        
        var localTilesPositions = new List<Vector3Int>(64);
        foreach (var pos in roadLevel.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            localTilesPositions.Add(localPlace);
        }
        
        roadLevel.ClearAllTiles();
        barrierLevel.ClearAllTiles();
        decorationLevel.ClearAllTiles();
        SetupRoad(localTilesPositions, roadLevel, barrierLevel);
        SetupDecoration(localTilesPositions, decorationLevel);
        roadLevel.CompressBounds();
        barrierLevel.CompressBounds();
        
    }
    private void SetupRoad(List<Vector3Int> localTilesPositions, Tilemap roadLevel, Tilemap barrierLevel)
    {   
        var partida = TilesResourcesLoader.GetTileByName("partida");
        var chegada = TilesResourcesLoader.GetTileByName("chegada");
        var verticalIn  = TilesResourcesLoader.GetTileByName("vertical-in");
        var verticalOut = TilesResourcesLoader.GetTileByName("vertical-out");
        var horizontalIn  = TilesResourcesLoader.GetTileByName("horizontal-in");
        var horizontalOut = TilesResourcesLoader.GetTileByName("horizontal-out");
        var rightTopIn  = TilesResourcesLoader.GetTileByName("rightTop-in");
        var rightTopOut = TilesResourcesLoader.GetTileByName("rightTop-out");
        var rightDownIn  = TilesResourcesLoader.GetTileByName("rightDown-in");
        var rightDownOut = TilesResourcesLoader.GetTileByName("rightDown-out");
        var leftTopIn  = TilesResourcesLoader.GetTileByName("leftTop-in");
        var leftTopOut = TilesResourcesLoader.GetTileByName("leftTop-out");
        var leftDownIn  = TilesResourcesLoader.GetTileByName("leftDown-in");
        var leftDownOut = TilesResourcesLoader.GetTileByName("leftDown-out");
        
        foreach(var localPosition in localTilesPositions)
        {
            var x = localPosition.x-1;
            var y = localPosition.y-1;
            if (_map.map[y, x] != 0){
                var type = _map.TileType(y, x);
                if (type==1){
                    roadLevel.SetTile(localPosition, rightTopIn);
                    barrierLevel.SetTile(localPosition, rightTopOut);
                }
                else if (type==2){
                    roadLevel.SetTile(localPosition, rightDownIn);
                    barrierLevel.SetTile(localPosition, rightDownOut);    
                }
                else if (type==3){
                    roadLevel.SetTile(localPosition, leftTopIn);
                    barrierLevel.SetTile(localPosition, leftTopOut);    
                }
                else if (type==4){
                    roadLevel.SetTile(localPosition, leftDownIn);
                    barrierLevel.SetTile(localPosition, leftDownOut);    
                }
                else if (type==5){
                    roadLevel.SetTile(localPosition, horizontalIn);
                    barrierLevel.SetTile(localPosition, horizontalOut);    
                }
                else if (type==6){
                    roadLevel.SetTile(localPosition, verticalIn);
                    barrierLevel.SetTile(localPosition, verticalOut);    
                }
                else if (type==-1)
                    Debug.Log("Erro ao colocar Tile");
                
            }
        }
        roadLevel.SetTile(new Vector3Int(1, 4, 0), partida);
        roadLevel.SetTile(new Vector3Int(1, 5, 0), chegada); 
    }
    private void SetupDecoration(List<Vector3Int> localTilesPositions, Tilemap decorationLevel)
    {
        Vector3Int oneX = new Vector3Int(1,0,0);
        Vector3Int oneY = new Vector3Int(0,1,0);
        Tile[] tribuneFull = {
            TilesResourcesLoader.GetTileByName("tribune_full_top"), 
            TilesResourcesLoader.GetTileByName("tribune_full_down"),
            TilesResourcesLoader.GetTileByName("tribune_full_left"),
            TilesResourcesLoader.GetTileByName("tribune_full_right")
        };
        Tile[] tribuneEmpty = {
            TilesResourcesLoader.GetTileByName("tribune_empty_top"), 
            TilesResourcesLoader.GetTileByName("tribune_empty_down"),
            TilesResourcesLoader.GetTileByName("tribune_empty_left"),
            TilesResourcesLoader.GetTileByName("tribune_empty_right")
        };

        System.Random random = new System.Random();

        foreach(var localPosition in localTilesPositions)
        {
            var x = localPosition.x-1;
            var y = localPosition.y-1;
            if (_map.map[y, x] != 0 && _map.PreviousTile(y, x).Equals(-_map.NextTile(y,x))){
                for(int i=0; i<4; i++){
                    var nx = x+dx[i];
                    var ny = y+dy[i];
                    if(nx>=9 || nx<-1 || ny>=9 || ny<-1)
                        continue;
                    
                    if((nx == -1 || nx == 8 || ny == -1 || ny == 8) || _map.map[ny,nx] == 0){
                        var probability = random.NextDouble();
                        if(probability<0.55)
                            decorationLevel.SetTile(localPosition + oneX*dx[i] + oneY*dy[i], tribuneFull[i]);
                        else if(probability<0.7)
                            decorationLevel.SetTile(localPosition + oneX*dx[i] + oneY*dy[i], tribuneEmpty[i]);

                    }
                        
                }
            }
        }
    }
    // private void SetupTiles()
    // {
    //     var envLevel = GetComponentsInChildren<Tilemap>()[0];
    //     var wallLevel = GetComponentsInChildren<Tilemap>()[1];
    //     var localTilesPositions = new List<Vector3Int>(100);
    //     foreach (var pos in envLevel.cellBounds.allPositionsWithin)
    //     {
    //         Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
    //         localTilesPositions.Add(localPlace);
    //     }
    //     SetupPath(localTilesPositions, envLevel);
    //     SetupWalls(localTilesPositions, wallLevel);
    // }
    // private void SetupPath(List<Vector3Int> localTilesPositions, Tilemap envLevel)
    // {
    //     var floor = TilesResourcesLoader.GetFloor();
    //     var barrier = TilesResourcesLoader.GetBarrier();

    //     foreach (var localPosition in localTilesPositions)
    //     {
    //         var x = localPosition.x;
    //         var y = localPosition.y;
    //         if (x == 0 || x == 9 || y == 0 || y == 9)
    //             envLevel.SetTile(localPosition, barrier);
    //         else
    //         {
    //             var tileToPlace = _map.map[y-1, x-1] == 0 ? barrier: floor;
    //             envLevel.SetTile(localPosition, tileToPlace);
    //         }
    //     }
    // }
    // private void SetupWalls(List<Vector3Int> localTilesPositions, Tilemap wallLevel)
    // {
    //     foreach (var localPosition in localTilesPositions)
    //     {
    //         var x = localPosition.x;
    //         var y = localPosition.y;

    //         bool isIni = x == 1 && y == 5;
    //         bool isEnd = x == 1 && y == 4;

    //         bool isPath = x>0 && x<9 && y>0 && y<9;
    //         var tile = isPath? _map.map[y-1,x-1] : 0;
    //         if (tile != 0)
    //         {
    //             int i = y-1;
    //             int j = x-1;
    //             int bottom = i>0 ? _map.map[i-1,j] : 0;
    //             int left = j>0 ? _map.map[i,j-1] : 0;
    //             int top = i<7 ? _map.map[i+1,j] : 0;
    //             int right = j<7 ? _map.map[i,j+1] : 0;

    //             bool fillBottom = (!isIni)&&((bottom == 0) || (Mathf.Abs(bottom-tile)!=1));
    //             bool fillLeft = (left == 0) || (Mathf.Abs(left-tile)!=1);
    //             bool fillTop = (!isEnd)&&((top == 0) || (Mathf.Abs(top-tile)!=1));
    //             bool fillRight = (right == 0) || (Mathf.Abs(right-tile)!=1);

    //             if(fillBottom)
    //                 fill(0, x, y, wallLevel);
    //             if(fillLeft)
    //                 fill(1, x, y, wallLevel);
    //             if(fillTop)
    //                 fill(2, x, y, wallLevel);
    //             if(fillRight)
    //                 fill(3, x, y, wallLevel);

    //             checkCorners(fillBottom, fillRight, fillTop, fillLeft, x, y, wallLevel);

    //         }
    //     }
    // }
    // private void fill(int side, int x, int y, Tilemap wallLevel)
    // {
    //     var posx = 8*x;
    //     var posy = 8*y;

    //     var wallTile = TilesResourcesLoader.GetWall(); 
    //     for (int i=0; i<8; i++)
    //     {
    //         Vector3Int posTile;
    //         if (side==0)
    //             posTile = new Vector3Int(posx + i, posy, 0);
    //         else if (side==1)
    //             posTile = new Vector3Int(posx, posy + i, 0);
    //         else if (side==2)
    //             posTile = new Vector3Int(posx + i, posy + 7, 0);
    //         else
    //             posTile = new Vector3Int(posx + 7, posy + i, 0); 

    //         wallLevel.SetTile(posTile, wallTile);
    //     } 
    // }
    // private void checkCorners(bool fillBottom, bool fillRight, bool fillTop, bool fillLeft, int x, int y, Tilemap wallLevel)
    // {
    //     if (fillBottom && fillRight)
    //     {
    //         Vector3Int posTile = posTile = new Vector3Int(8*x, 8*y+7, 0);;
    //         var wallTile = TilesResourcesLoader.GetWall();
    //         wallLevel.SetTile(posTile, wallTile);
    //     }  
    //     if (fillRight && fillTop)
    //     {
    //         Vector3Int posTile = posTile = new Vector3Int(8*x, 8*y, 0);;
    //         var wallTile = TilesResourcesLoader.GetWall();
    //         wallLevel.SetTile(posTile, wallTile);
    //     }  
    //     if (fillTop && fillLeft)
    //     {
    //         Vector3Int posTile = posTile = new Vector3Int(8*x+7, 8*y, 0);;
    //         var wallTile = TilesResourcesLoader.GetWall();
    //         wallLevel.SetTile(posTile, wallTile);
    //     }  
    //     if (fillLeft && fillBottom)
    //     {
    //         Vector3Int posTile = posTile = new Vector3Int(8*x+7, 8*y+7, 0);;
    //         var wallTile = TilesResourcesLoader.GetWall();
    //         wallLevel.SetTile(posTile, wallTile);                
    //     }
    // }
}

