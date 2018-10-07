using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public enum Menu
    {
        Tutorial,
        Game
    };

    public Menu menu;
    public Image GazingImage;
    public MainMenuControl MenuCtrl;
    public bool isChecked = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void check()
    {
        if (MenuCtrl.canSelect == true)
        {
            isChecked = true;
            StartCoroutine(ShrinkBar());
        }
    }

    public void uncheck()
    {
        isChecked = false;
        GazingImage.fillAmount = 1;

    }

    IEnumerator ShrinkBar()
    {
        while (true) {
            GazingImage.fillAmount -= 0.0025f;
            if (isChecked == false)
            {
                yield break;
            }
            if(GazingImage.fillAmount <= 0)
            {
                if(menu == Menu.Tutorial)
                {
                    MenuCtrl.canSelect = false;
                    MenuCtrl.StartCoroutine(MenuCtrl.NextScene("Tutorial"));
                    yield break;
                }
                else if(menu == Menu.Game)
                {
                    MenuCtrl.canSelect = false;
                    MenuCtrl.StartCoroutine(MenuCtrl.NextScene("Game"));
                    yield break;
                }
            }
            yield return new WaitForEndOfFrame();
       }
    }
}
