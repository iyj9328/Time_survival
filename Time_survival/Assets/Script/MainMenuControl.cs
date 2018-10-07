using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour {

    public bool canSelect = false;
   // private bool check = true;
    //private string sceneName;
    public Image FadeImage;

    // Use this for initialization
    void Start () {
        canSelect = true;
        //sceneName = this.gameObject.ToString();
        StartCoroutine(FadeIn());
	}
	
    public IEnumerator FadeIn()
    {
        float alpha = 1f;

        while (true)
        {
            alpha -= 0.01f;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            if (alpha <= 0f)
            {
                canSelect = true;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    /*
    public void getGazing()
    {
        check = true;
        StartCoroutine(NextScene(getScene()));
    }

    public void lostGazing()
    {
        check = false;
    }

    public string getScene()
    {
        return sceneName;
    }*/

    public IEnumerator NextScene(string scenename)
    {
        float alpha = 0f;
        
        while (true)
        {
            alpha += 0.01f;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            if(alpha > 1)
            {
                SceneManager.LoadScene(scenename);
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
