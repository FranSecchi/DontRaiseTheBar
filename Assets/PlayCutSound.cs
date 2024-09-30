using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCutSound : MonoBehaviour
{
    void playCutSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.knife_cut, new Vector3(0,0,0));
    }
}
