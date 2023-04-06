using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class MapLoader : MonoBehaviour
{
    private const int NUM_MAPS = 31;
    private const string PATH = "Assets/Maps/";

    public Map loadRandom()
    {
        int [] sectionsNumbers = generateRandomValues(4);    

        Map[] mapPieces = getMapPiecesFromNumbers(sectionsNumbers);

        Map map = glueAllPieces(mapPieces);
        
        return map;
    }

    public Map loadSpecific(int[] sectionNumbers)
    {    
        Map[] mapPieces = getMapPiecesFromNumbers(sectionNumbers);

        Map map = glueAllPieces(mapPieces);

        return map;
    }

    private int[] generateRandomValues(int size)
    {
        System.Random random = new System.Random();
        int [] values = new int[size];
        for(int i=0; i<size; i++)
        {
            values[i] = random.Next(NUM_MAPS);
        }
        return values;
    }

    private Map[] getMapPiecesFromNumbers(int[] nums)
    {
        Map[] mapPieces = new Map[4];
        for(int i=0; i<4; i++)
        {
            mapPieces[i] = getMapPieceFromNumber(nums[i]);
        }
        return mapPieces;
    }

    private Map getMapPieceFromNumber(int num)
    {
        string filename = "map_" + num.ToString();
        
        string text = Resources.Load<TextAsset>($"{filename}").text;

        if (string.IsNullOrEmpty(text)) throw new Exception("Current file is null or empty: " + filename);
        
        string[] lines = text.Split('\n');

        int[,] map = new int[4,4];

        for(int i=0; i<4; i++)
        {
            string line = lines[3-i];
            string[] stringNums = line.Split(' ');
            for( int j=0; j<4; j++ )
            {
                var stringNum = stringNums[j];
                map[i,j] = Int32.Parse(stringNum);
            }
        }

        Map m = new Map();
        m.map = map;
        return m;        
    }

    private Map glueAllPieces(Map[] pieces)
    {
        Map map = new Map();
        int [,] m = new int[8,8];
        
        for( int i=0; i<4; i++ )
            for ( int j=0; j<4; j++ )
                m[4+i,j] = pieces[0].map[i,j];

        var offset = m[7,3];

        for( int i=0; i<4; i++ )
            for ( int j=0; j<4; j++ )
                m[4+i,4+j] = pieces[1].map[j, 3-i]==0 ? 0: pieces[1].map[j, 3-i]+offset;

        offset = m[4,7];
        for( int i=0; i<4; i++ )
            for ( int j=0; j<4; j++ )
                m[i,4+j] = pieces[2].map[3-i,3-j]==0 ? 0: pieces[2].map[3-i,3-j] + offset;

        offset = m[0,4];
        for( int i=0; i<4; i++ )
            for ( int j=0; j<4; j++ )
                m[i,j] = pieces[3].map[3-j, i]==0 ? 0 : pieces[3].map[3-j, i]+offset;

        map.map = m;
        return map; 
    }    
}
