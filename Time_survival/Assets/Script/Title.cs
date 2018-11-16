using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    public Text TitleText;
    public Text NoticeText;
    public Image PadImage;
    public Image WaitBarImage;
	// Use this for initialization
	IEnumerator Start () {
        float alpha = 0;
        while (true)
        {
            TitleText.color = new Color(1, 1, 1, alpha);
            alpha += 0.01f;
            if (alpha > 1) break;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2f);
        while (true)
        {
            
            TitleText.color = new Color(1, 1, 1, alpha);
            alpha -= 0.01f;
            if (alpha < 0) break;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(WaitInput());        //유저의 입력을 기다리며 패드의 입력이 없을 시에 AutoFire모드 전환    
    }

    IEnumerator WaitInput()
    {
        WaitBarImage.fillAmount = 1;
        PadImage.color = new Color(1, 1, 1, 1);
        NoticeText.text = "블루투스 게임패드의 버튼을 눌러주세요. \n누르지 않고 대기 시 게임패드를 사용하지 않습니다.";
        float fill = 200.0f;
        while (true)
        {
            fill -= 0.2f;
            WaitBarImage.fillAmount = fill / 200.0f;
            if (Input.GetButton("Fire1") || OVRInput.GetDown(OVRInput.RawButton.A))
            {
                Option.inputoption = Option.InputOption.Pad;
                break;
            }
            else if (fill < 0)
            {
                Option.inputoption = Option.InputOption.NoPad;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }

}
