
using UnityEngine;
using UnityEngine.UI;

public class boost_bar : MonoBehaviour
{

    public Slider slider;

    
    public void SetInitBoost(){
        slider.maxValue=100;
        slider.value=0;
    }

    public void SetBoost(float amount){
        slider.value=amount;
    }

    public void AddBoost(float amount){
        if((slider.value+amount)>slider.maxValue){
            slider.value=slider.maxValue;
        }else{
            slider.value+=amount;
        }
    }

    public void RemoveBoost(float amount){
        if((slider.value-amount)<slider.minValue){
            slider.value=slider.minValue;
        }else{
            slider.value-=amount;
        }
        
    }

    public float getBoost(){
        return slider.value;
    }
}
