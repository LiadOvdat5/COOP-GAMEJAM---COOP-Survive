using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public bool someoneDied;
    public void StartBoutton()
    {
        SceneManager.LoadScene("FirstFloor");
    }

    private void Update()
    {
        SomeoneDied();
    }

    public void SomeoneDied()
    {
        if (someoneDied)
        {
            StartCoroutine(DeathScene());
        }
    }
    IEnumerator DeathScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Death");
    }
    public void HomeScreen()
    {
        SceneManager.LoadScene("Openning Scene");
    }

}
