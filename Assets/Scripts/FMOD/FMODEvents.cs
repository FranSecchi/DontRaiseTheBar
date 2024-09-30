using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }
    [field:SerializeField] public EventReference sucess { get; private set; }
    [field:SerializeField]public EventReference fail { get; private set; }
    [field:SerializeField]public EventReference liquid_pour { get; private set; }
    [field:SerializeField]public EventReference knife_cut { get; private set; }
    [field:SerializeField]public EventReference coin_bonus { get; private set; }
    [field:SerializeField]public EventReference music { get; private set; }
    [field:SerializeField]public EventReference glass { get; private set; }
    

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more FMODEvents");
        }
        instance = this;
    }
}
