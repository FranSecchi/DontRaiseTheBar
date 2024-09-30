using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private EventInstance musicEventInstance;

    public List<EventInstance> eventInstances;

    void Awake()
    {
       
        if (instance == null)
        {
            instance = this;
            eventInstances = new List<EventInstance>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            
        }
    }

    private void Start()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        
        
        InitializeMusic(FMODEvents.instance.music);
        
        
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    public void InitializeMusic(EventReference musicEventReference)    
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start(); 
    }

    private void OnDestroy()
    {
        CleanUp();
    }

}
