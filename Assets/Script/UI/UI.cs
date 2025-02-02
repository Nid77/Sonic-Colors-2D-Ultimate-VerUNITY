using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public int ringsCount;

    public int lifesCount;

    public static UI instance;

    public TextMeshProUGUI ringsCountText;

    public TextMeshProUGUI lifesCountText;


    private void Awake(){
        if(instance==null){
            instance = this;
        }

        
        ringsCountText.text=ringsCount.ToString();
        lifesCountText.text=lifesCount.ToString();
    }

    public void AddRing(int amount ){
        ringsCount+=amount ;
        ringsCountText.text=ringsCount.ToString();
    }

    public void AddLife(int amount ){
        lifesCount+=amount ;
        lifesCountText.text=lifesCount.ToString();
    }

    public void voidAddTime(int amount){

    }

    public void RemoveLife(){
        lifesCount+=-1;
        lifesCountText.text=lifesCount.ToString();
    }

    public void RemoveRings(){
        ringsCount=0;
        ringsCountText.text=ringsCount.ToString();
    }
}
