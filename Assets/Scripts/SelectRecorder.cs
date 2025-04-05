using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRecorder : MonoBehaviour
{
    public GameObject SwordMapping;
    public GameObject ShieldMapping;
    public GameObject MagicCircleMapping;
    // Start is called before the first frame update
    void showSwordMapping(){
        SwordMapping.SetActive(true);
    }

    void showShieldMapping(){
        ShieldMapping.SetActive(true);
    }
    
    void showMagicCircleMapping(){
        MagicCircleMapping.SetActive(true);
    }
}
