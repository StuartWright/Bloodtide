using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public BasePlayer Player;
    public void Level1()
    {
        Player.PlayBlackFadeOut();
        StartCoroutine(Scene1ChangeDelay());
    }
    IEnumerator Scene1ChangeDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("FirstScene");
        Destroy(gameObject);
    }
    public void Level2()
    {
        Player.PlayBlackFadeOut();
        StartCoroutine(Scene2ChangeDelay());
    }
    IEnumerator Scene2ChangeDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Level2");
        Destroy(gameObject);
    }
}
