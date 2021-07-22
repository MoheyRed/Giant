using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegularCanvas : MonoBehaviour
{
    public GameController game;
    public void Play()
    {
        game.click.Post(gameObject.gameObject);
        game.stopAll.Post(game.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void Replay()
    {
        game.click.Post(gameObject.gameObject);
        game.stopAll.Post(game.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Exit()
    {
        game.click.Post(game.gameObject);
        Application.Quit();
    }
}
