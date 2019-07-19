using System.Collections;
using System.Collections.Generic;
using Firebase.TestLab;
using System;
using UnityEngine;
using UnityEngine.UI;


public class TestLoops : MonoBehaviour
{
    private TestLabManager _testLabManager;
    private const int TestIndex = 1;
    private bool[] StopArray;
    private int fNumber = 4;
    bool isTestingInEditor = true;

    // Use this for initialization
    void Start()
    {
        _testLabManager = TestLabManager.Instantiate();
        StopArray = new bool[fNumber];
        for (int i = 0; i < fNumber; i++)
            StopArray[i] = false;

    }

    // Update is called once per frame
    void Update()
    {
        // only change the behavior of the app if there is a scenario being tested
        #region FIREBASE TEST
           if (!_testLabManager.IsTestingScenario ) return;

           switch (_testLabManager.ScenarioNumber)
           {
               case TestIndex:
                   OpenMap();
                   CloseMap();
                   OpenOptions();
                   CloseOptions();
                   CloseTesting();

                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           } 
        #endregion
        #region EDITOR TEST
       /*  if (isTestingInEditor == false) return;
        OpenMap();
        CloseMap();
        OpenOptions();
        CloseOptions();
        CloseTesting();
        //isTestingInEditor = false;*/

        #endregion
    }

    private void OpenMap()
    {
        if (!(Time.time > 3.0f)) return;
        if (StopArray[0] == false)
        {
            GetComponent<UIController>().MenuChange("Map");
            GetComponent<AnimationSystem>().MapAnimations();
            StopArray[0] = true;
        }







    }

    private void CloseMap()
    {
        if (!(Time.time > 5.0f)) return;
        if (StopArray[1] == false)
        {
            GetComponent<UIController>().MenuChange("MainMenu");
            StopArray[1] = true;
        }







    }
    private void OpenOptions()
    {
        if (!(Time.time > 7.0f)) return;
        if (StopArray[2] == false)
        {
            GetComponent<UIController>().MenuChange("Options");
            GetComponent<AnimationSystem>().OptionLeft(true);
            GetComponent<AnimationSystem>().OptionRight(true);
          //  GetComponent<AnimationSystem>().OptionsAnimations();
            if (!GetComponent<UIController>().SkipToggle.isOn)
            {
                GetComponent<UIController>().SkipToggle.isOn = true;
                GetComponent<PrefSettings>().setSkip();
            }

            StopArray[2] = true;
        }







    }
    private void CloseOptions()
    {
        if (!(Time.time > 9.0f)) return;
        if (StopArray[3] == false)
        {
            GetComponent<UIController>().MenuChange("MainMenu");
            GetComponent<AnimationSystem>().OptionLeft(false);
            GetComponent<AnimationSystem>().OptionRight(false);

            StopArray[3] = true;
        }

    }

    private void CloseTesting()
    {
        if (!(Time.time > 11.0f)) return;

        bool done = true;
        for (int i = 0; i < fNumber; i++)
        {
            if (StopArray[i] == false)
                done = false;


        }
        if (done)
        {
            _testLabManager.NotifyHarnessTestIsComplete();
            isTestingInEditor = false;
           // GetComponent<UIController>().Load();
        }


    }

    private void OpenGame()
    {
        if (!(Time.time > 7.0f)) return;
        if (StopArray[2] == false)
        {
            GetComponent<UIController>().MenuChange("Options");
            GetComponent<AnimationSystem>().OptionLeft(true);
            GetComponent<AnimationSystem>().OptionRight(true);
           // GetComponent<AnimationSystem>().OptionsAnimations();

            StopArray[2] = true;
        }

    }



}
