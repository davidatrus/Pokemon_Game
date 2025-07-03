using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
//make this a class cuz it will allow us to expand upon quest stuff later. 
public class Dialog 
{
    [SerializeField] List<string> lines;

    public List <string> Lines
    {
        get { return lines; }
    }
}
