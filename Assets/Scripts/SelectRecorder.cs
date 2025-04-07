using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRecorder : MonoBehaviour
{
    public GameObject SwordMapping;
    public GameObject ShieldMapping;
    public GameObject MagicCircleMapping;
    // Start is called before the first frame update
    public void showSwordMapping(){
        SwordMapping.SetActive(true);
    }
    public void CloseSwordMapping(){
        SwordMapping.SetActive(false);
    }

    public void showShieldMapping(){
        ShieldMapping.SetActive(true);
    }
    public void CloseShieldMapping(){
        ShieldMapping.SetActive(false);
    }
    
    public void showMagicCircleMapping(){
        MagicCircleMapping.SetActive(true);
    }
    public void CloseMagicCircleMapping(){
        MagicCircleMapping.SetActive(false);
    }
}
