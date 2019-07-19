using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIController : MonoBehaviour
{
    #region VARIABLES
    public GameObject slide;// the loading bar for game loading
    public GameObject button; //the start button
    GameObject[] MenuList; //a list of all menus 
    public Text SpeedText; // The UI text for speed setting
    public string[] SpeedOptions; // the name of Speed setting values
    public int SpeedIndex = 0; // index of current text speed value
    public Toggle SkipToggle;
    public Toggle MusicToggle;
    public Toggle SoundToggle;
    public GameObject Music;
    public float FadeSpeed;
    public float MaxVolume;
    AnimationSystem AnimSys;
    public GameObject NewButton;
    public GameObject ContinueButton;
    public GameObject RecapButton;
    public GameObject StartMenuBackButton;
    public GameObject MapCanvas;
    #endregion
    #region FUNCTIONS
    public void Load(bool isNew) //Function that load the main game
    { 
        StopAllCoroutines();
        StartCoroutine(FadeMusic(false));//starts the fade out effect and after that async loading
        if (isNew)
        {
            GetComponent<SaveLoad>().SaveText("Start", 0);
        }
    }
    public void LoadInstant()
    {
        if (PlayerPrefs.GetString("Node") == "Start" && PlayerPrefs.GetInt("Line") == 0)
        {
            MenuChange("StartMenu");
            StartMenuBackButton.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(FadeMusic(false));//starts the fade out effect and after that async loading

        }
        else
        {
            StartMenuBackButton.SetActive(true);
            AnimSys.StartAnimations();
            MenuChange("StartMenu");
        }

    }
    IEnumerator LoadAs()//Function that load main game async
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(1);

        AnimSys.isLoading = true;
        slide.SetActive(true);//slider gets shown
        AnimSys.isAnimationDone = false;

        
        AnimSys.EnableDisableButton(button,false); //start button gets deactivated when is pressed for preventing problems
        AnimSys.EnableDisableButton(AnimSys.OptionButton, false);
        AnimSys.EnableDisableButton(AnimSys.MapButton, false);
        
        op.allowSceneActivation = false;

        AnimSys.PlayAnimation(AnimSys.LeftBackgroundOption, "LeftAnim", 0);
        AnimSys.PlayAnimation(AnimSys.RightBackgroundOption, "RightAnim", 0);


        while (!op.isDone)// while loading is not complete, fill loading bar percentage
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slide.GetComponent<Slider>().value = progress * 4;
            if (op.progress >= 0.9f)
            {
                AnimSys.EnableDisableUI(AnimSys.LeftBackgroundOption, true,false);
                AnimSys.EnableDisableUI(AnimSys.RightBackgroundOption, true,false);

                AnimSys.PlayAnimation(AnimSys.LeftBackgroundOption, "LeftAnim", 1);
                AnimSys.PlayAnimation(AnimSys.RightBackgroundOption, "RightAnim", 1);




                if (AnimSys.isAnimationDone)
                    op.allowSceneActivation = true;

            }

            yield return null;
        }

    }
    IEnumerator FadeMusic(bool dir)
    {   
        AudioSource AudioElement = Music.GetComponent<AudioSource>();
        if (dir)
        {
           // yield return new WaitForSecondsRealtime(1.5f);

            AudioElement.volume = 0;

            Music.GetComponent<AudioSource>().Play();

            while (AudioElement.volume <= MaxVolume)
            {
                AudioElement.volume += FadeSpeed;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (AudioElement.volume >0)
            {
                AudioElement.volume -= FadeSpeed*4;
                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(LoadAs());
        }
         
    }
    void ChangeSettings(string val, bool direction)//Options menu behaviour , parameters : val represents type of option and direction: direction of arrow pressed
    {
        if (val == "Speed")
        {
            if (direction)
            {
                SpeedIndex++;
                if (SpeedIndex > SpeedOptions.Length - 1)
                {
                    SpeedIndex = 0;
                }
            }
            else
            {

                SpeedIndex--;
                if (SpeedIndex < 0)
                {
                    SpeedIndex = SpeedOptions.Length - 1;
                }

            }
            SpeedText.GetComponent<Text>().text = SpeedOptions[SpeedIndex];
        }

    }
    public void ChangeSpeedText(bool Dir)// function called when user wants to change the speed settings, dir represents the pressed arrow direction 
    {
        ChangeSettings("Speed", Dir);
    }

    public void MenuChange(string name)// change between different menus . name represents menu name
    {
        foreach (GameObject menu in MenuList)// iterate through menus
        {
            if (menu.name == name)// if name parameter matches the menu name
            {
                menu.GetComponent<Canvas>().enabled = true;// show it
            }
            else
                menu.GetComponent<Canvas>().enabled = false;//hide it


        }

    }
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }




    #endregion
    void Start()
    {
        AnimSys = this.GetComponent<AnimationSystem>();

        slide.SetActive(false);//hide loading bar
        Application.targetFrameRate = 30;//set target framerate to 30
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//prevent screen from sleep
        MenuList = GameObject.FindGameObjectsWithTag("UIMenu");// add every menu with UIMenu tag to menulist
        GameObject.Find("Background").GetComponent<Canvas>().enabled = true;//show background
        MenuChange("");//hide all menus
        MenuChange("MainMenu");//show main menu
        for (int i = 0; i < this.GetComponent<PrefSettings>().SpeedValues.Length; i++)//update UI to show options correctly in option menu
        {
            if (PlayerPrefs.GetFloat("Speed") == this.GetComponent<PrefSettings>().SpeedValues[i])
            {
                SpeedIndex = i;
            }
        }
        SpeedText.GetComponent<Text>().text = SpeedOptions[SpeedIndex];//set saved speed value to option text
        if (PlayerPrefs.GetInt("Skip") == 1)
        {
            SkipToggle.isOn = true;
        }
        else
            SkipToggle.isOn = false;

        if (PlayerPrefs.GetInt("Music") == 1)
        {
            MusicToggle.isOn = true;
        }
        else
            MusicToggle.isOn = false;

        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            SoundToggle.isOn = true;
        }
        else
            SoundToggle.isOn = false;


        StartCoroutine(FadeMusic(true));

        if (PlayerPrefs.GetString("Node") == "Start")
        {
            RecapButton.SetActive(false);
        }
        else
        {
            RecapButton.SetActive(true);
        }

        if (PlayerPrefs.GetString("Node") == "Start" && PlayerPrefs.GetInt("Line") == 0)
        {
            ContinueButton.SetActive(false);
            NewButton.SetActive(false);

        }
        else
        {
            ContinueButton.SetActive(true);
            NewButton.SetActive(true);

        }

        MapCanvas.GetComponent<RectTransform>().localPosition = new Vector3(MapCanvas.GetComponent<RectTransform>().localPosition.x, 0, MapCanvas.GetComponent<RectTransform>().localPosition.z);

    }

    void Update()
    {
        if (PlayerPrefs.GetFloat("Speed") == 3000)
        {
            SkipToggle.interactable = false;
            GameObject.Find("SkipWarning").GetComponent<Text>().enabled = true;

        }
        else
        {
            SkipToggle.interactable = true;
            GameObject.Find("SkipWarning").GetComponent<Text>().enabled = false;
        }

        if (PlayerPrefs.GetInt("Music") == 1)
        {
            Music.GetComponent<AudioSource>().volume = 1.0f;
        }
        else
            Music.GetComponent<AudioSource>().volume = 0f;

    }

}
