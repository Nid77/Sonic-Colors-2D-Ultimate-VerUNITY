using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
     private List<KeyCode> keyCodeList = new List<KeyCode>();
     public Dictionary<string, bool> keyDictionary = new Dictionary<string, bool>();
    public bool Enabled=true;
    private void Awake(){

        keyCodeList.Add(KeyCode.Space);
        keyCodeList.Add(KeyCode.LeftArrow);
        keyCodeList.Add(KeyCode.RightArrow);
        keyCodeList.Add(KeyCode.UpArrow);
        keyCodeList.Add(KeyCode.DownArrow);
        keyCodeList.Add(KeyCode.U);
        keyCodeList.Add(KeyCode.V);

        keyDictionary.Add("Jump", false);
        keyDictionary.Add("Left", false);
        keyDictionary.Add("Right", false);
        keyDictionary.Add("Up", false);
        keyDictionary.Add("Down", false);
        keyDictionary.Add("Boost", false);
        keyDictionary.Add("Wisp", false);        
        
       
    }
    void Update()
    {

        if(Enabled){
            keyDictionary["Jump"]=Input.GetKey(keyCodeList[0]);
            keyDictionary["Left"]=Input.GetKey(keyCodeList[1]);
            keyDictionary["Right"]=Input.GetKey(keyCodeList[2]);
            keyDictionary["Up"]=Input.GetKey(keyCodeList[3]);
            keyDictionary["Down"]=Input.GetKey(keyCodeList[4]);
            keyDictionary["Boost"]=Input.GetKey(keyCodeList[5]);
            keyDictionary["Wisp"]=Input.GetKey(keyCodeList[6]);

        }
        
    }
}
