using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class AnimationSystem : MonoBehaviour
{
    #region VARIABLES
    public GameObject Logo; //Logo Text
    public GameObject PlayButton; 
    public GameObject OptionButton;
    public GameObject MapButton;
    public GameObject CreditsButton;
    public GameObject LeftBackgroundOption;
    public GameObject RightBackgroundOption;
    GameObject AllOptions; //Container with all option UI element
    public GameObject Map;
    public GameObject Board1, Board2,Board3;

    [HideInInspector]
    public bool isAnimationDone; // is the door animation done
    [HideInInspector]
    public bool isLoading = false; // is the main game loading

    IEnumerator PlayAnimationsCoroutine; 
    #endregion

    void Start()
    {
        EnableDisableUI(LeftBackgroundOption, false, false); // disable door transition UI elements
        EnableDisableUI(RightBackgroundOption, false, false);


        PlayAnimationsCoroutine = PlayAnimations(); // start the intro animations
        StartCoroutine(PlayAnimationsCoroutine);

    }

    public void AnimationDone()  
    {
        isAnimationDone = true;
    } // enable isAnimationDone variable

    public void EnableDisableUI(GameObject element, bool state,bool isText) // Enable or Disable Animator and Image/Text components of UI element
    {   
        
            element.GetComponent<Animator>().enabled = state; // enable/disable animator component
            if (!isText) // if UI element is not text
                element.GetComponent<Image>().enabled = state; // enable/disable image component
            else
                element.GetComponent<Text>().enabled = state; // enable/disable text component

    }

    public void EnableDisableButton(GameObject button, bool state) // enable or Disable Button component of UI element
    {
            button.GetComponent<Button>().interactable = state;
    }

    public void PlayAnimation(GameObject element, string name, int state) //Play Specific Animation
    {
        if (state == 0) //reset the animation by name
        {
            element.GetComponent<Animator>().Play(name, -1, 0);
        }
        if (state == 1) // Directly Play the animation by name
        {
            element.GetComponent<Animator>().Play(name);
        }
        if (state == 2) //Reset an animation and play it by name
        {
            element.GetComponent<Animator>().Play(name, -1, 0);
            element.GetComponent<Animator>().Play(name);
        }
    }

    void ChangeAlpha(float value, GameObject element) // change the alpha value of an image
    {

        var tempColor = element.GetComponent<Image>().color;
        tempColor.a = value;
        element.GetComponent<Image>().color = tempColor;

    }

    public void ZoomInAnimation(GameObject element)// a function which Play the animation called ZoomInFade . Usually it's used for logo at the start of the game
    {
        EnableDisableUI(element, true,false);
        PlayAnimation(element, "ZoomInFade", 2);
    }

    public void ZoomOutAnimation(GameObject element)// a function which Play the animation called FadeTwo .Meant for Buttons
    {
        ChangeAlpha(1f, element);

        EnableDisableButton(element, true);
        EnableDisableUI(element, true,false);
        PlayAnimation(element, "FadeTwo", 2);
    }

    public void OptionBackgroundAnimation(bool dir, GameObject element, string position) //play the door retracting/closing animation
    {  //dir true is closing animation. false is retracting

        if (dir)

        {
            if (position == "left")
            {
                PlayAnimation(element, "LeftAnim", 2);
            }
            else
            {
                PlayAnimation(element, "RightAnim", 2);
            }



        }
        else
        {
            if (position == "left")
            {
                PlayAnimation(element, "LeftAnim", 0);
                PlayAnimation(element, "Reverse", 2);

            }
            else
            {
                PlayAnimation(element, "RightAnim", 0);
                PlayAnimation(element, "Reverse", 2);

            }
        }

    }

    public void OptionLeft(bool dir) // function that calls the OptionBackgroundAnimation as button event
    {
        EnableDisableUI(LeftBackgroundOption, true,false);

        OptionBackgroundAnimation(dir, LeftBackgroundOption, "left");
    }

    public void OptionRight(bool dir) //same as above
    {
        EnableDisableUI(RightBackgroundOption, true,false);

        OptionBackgroundAnimation(dir, RightBackgroundOption, "right");
    }

    public void OptionsAnimations(GameObject AllOptions) //  function that play animation as button event for options menu
    {
        PlayAnimation(AllOptions, "OptionAnimation", 2);


    }

    public void MapAnimations() // function that play animation as button event for map menu
    {
        PlayAnimation(Map, "MapAnim", 2);


    }

    public void StartAnimations()
    {
        PlayAnimation(Board1, "BoardAnim", 2);
        PlayAnimation(Board2, "BoardAnimReverse", 2);
        PlayAnimation(Board3, "BoardAnim", 2);
    }
    IEnumerator PlayAnimations() //coroutine that plays the reveal animation at the start of the game
    {
        GameObject[] elements= { Logo,PlayButton,OptionButton,MapButton,CreditsButton};
        EnableDisableUI(Logo, false, false); //disable UI

        for (int i = 1; i < elements.Length; i++)
        {
            ChangeAlpha(0f, elements[i]); //hide all UI elements
            EnableDisableUI(elements[i], false, false); //disable UI
            EnableDisableButton(elements[i], false); //Disable Buttons


        }


        ZoomInAnimation(Logo);  //play animations in sequence
        yield return new WaitForSecondsRealtime(0.8f);
        ZoomOutAnimation(PlayButton);
        yield return new WaitForSecondsRealtime(0.5f);
        ZoomOutAnimation(OptionButton);
        yield return new WaitForSecondsRealtime(0.2f);

        ZoomOutAnimation(MapButton);
        yield return new WaitForSecondsRealtime(0.2f);
        ZoomOutAnimation(CreditsButton);

        yield return null;

    }




}
