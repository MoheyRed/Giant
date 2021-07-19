using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject[] HowToPlay;
    int index=0;
    public AK.Wwise.Event intro, click, titlemusic,stopAll;
    private void Start()
    {
        titlemusic.Post(gameObject);
    }
    public void next()
    {
        if (index==0||index==1)
        {
            intro.Stop(gameObject);
            intro.Post(gameObject);
        }
        click.Post(gameObject);
        HowToPlay[index].SetActive(false);
        index++;
        HowToPlay[index].SetActive(true);
    }
    public void play()
    {
        click.Post(gameObject);
        stopAll.Post(gameObject); 
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        click.Post(gameObject);
        Application.Quit();
    }
}
