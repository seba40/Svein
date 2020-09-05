using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TagSystem : MonoBehaviour {
    #region VARIABLES
    Object[] ChapterSprites; // list of all chapter images
    Object[] SlideSprites; // list of all slide images
    Object[] SlideMusic; // list of all slide music
    Object[] SlideSound; // list of all slide sounds

    string[] SoundValues= { "name","0", "1", "2" };// values in sound tag
    string[] MusicValues= { "name","1"};

    public Text TextComponent;

    IEnumerator PlaySoundRoutine, FadeInMusicRoutine,FadeOutMusicRoutine, FadeInOutMusicRoutine,FadeOutSoundRoutine;
    string TempString; //used for a text duplicate
    [HideInInspector]
    public bool isChapter;


    #endregion

    void Awake ()
    {   
        // RESOURCES SETUP

        ChapterSprites = Resources.LoadAll("ChapterImages", typeof(Sprite));//load all chapter images from resource folder
        SlideSprites = Resources.LoadAll("SlideImages", typeof(Sprite));//load all slide images from resource folder
        SlideMusic = Resources.LoadAll("Music", typeof(AudioClip));// load all music
        SlideSound = Resources.LoadAll("Sounds", typeof(AudioClip));// load all sounds

        //UI SETUP
        GameObject.Find("ChapterImage").GetComponent<Image>().enabled = false;//hide image shown on chapter title screen
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;//hide the main UI
        GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;//show the maine UI
        GameObject.Find("EndCanvas").GetComponent<Canvas>().enabled = false;//hide game ending screen

        //COROUTINES SETUP
        FadeOutMusicRoutine = FadeMusic(false, GameObject.Find("Music").GetComponent<AudioSource>());
        FadeInMusicRoutine = FadeMusic(true, GameObject.Find("Music").GetComponent<AudioSource>());
        FadeInOutMusicRoutine = FadeInOutMusic(GameObject.Find("Music").GetComponent<AudioSource>(),MusicValues[1]);
        FadeOutSoundRoutine = FadeMusic(false, GameObject.Find("Sounds").GetComponent<AudioSource>());
        PlaySoundRoutine = PlaySound(SoundValues[1], float.Parse(SoundValues[2]), float.Parse(SoundValues[3]));

        GameObject.Find("Music").GetComponent<AudioSource>().enabled = true;

        GameObject.Find("SlideImage").GetComponent<Image>().enabled = false;//hide the end chapter image (UI Element)

        foreach (AudioClip element in SlideMusic)
        {
            if (element.name == this.GetComponent<SaveLoad>().LoadMusic())
            {
                GameObject.Find("Music").GetComponent<AudioSource>().clip = element;// add audio clip to music component
                StartCoroutine(FadeInMusicRoutine);


            }
        }


    }
    #region TAG PROCESSING FUNCTIONS

    public void TAG_NEWLINE()  //find the newline tag in text and add a new line to that
    {   if (TempString.Contains("<*NEWLINE*>"))
         TempString = TempString.Replace("<*NEWLINE*>", "\n");
    }

    public void TAG_CHAPTER()
    { // chapter tag must contain <*CHAPTER_nameofimage*>
        if (TempString.Contains("<*CHAPTER"))//find text slides containing chapter tag
        {
            isChapter = true;
            string[] ChapterValues;
            string WholeString = TempString.Substring(TempString.IndexOf("<*CHAPTER"), TempString.IndexOf("*>", TempString.IndexOf("<*CHAPTER")) - TempString.IndexOf("<*CHAPTER") + 2);//variable containing the tag
            TempString = TempString.Replace(WholeString, "");//*prevent the tag from showing in app
            WholeString = WholeString.Replace("<*", "");//
            WholeString = WholeString.Replace("*>", "");//
            ChapterValues = WholeString.Split('_');//get each individual values from tag


            ChapterTitle(true);//make the text bigger and centered
            GameObject.Find("ChapterImage").GetComponent<Image>().enabled = true;//show the chapter title image (UI Element)
            foreach (Sprite element in ChapterSprites)
            {
                if (element.name == ChapterValues[1])
                {
                    GameObject.Find("ChapterImage").GetComponent<Image>().sprite = element;//change the image in UI Element
                }
            }
            
            FadeUI(GameObject.Find("ChapterImage"));
            //////////////


        }
        else
        {
            isChapter = false;
            ChapterTitle(false);// don't change text appearance if tag is not found in text
            GameObject.Find("ChapterImage").GetComponent<Image>().enabled = false;//hide chapter title image if tag is not found in text

        }

    }

    public void TAG_STYLE()
    {
        if (!TempString.Contains("<*BOLD*>") || TempString.Contains("<*ITALICS*>"))
        {
            TextStyle("normal");//if no tag is present , make text normal 
        }

        if (TempString.Contains("<*ITALICS*>"))//find text slides containing style tag
        {
            TempString = TempString.Replace("<*ITALICS*>", "");//prevent the tag from showing in app
            TextStyle("italic");//make text italic
        }

        if (TempString.Contains("<*BOLD*>"))//find text slides containing style tag
        {
            TempString = TempString.Replace("<*BOLD*>", "");//prevent the tag from showing in app
            TextStyle("bold");//make text bold
        }


    }

    public void TAG_INSLIDE_IMAGES()
    {
        if (!TempString.Contains("<*IMAGE") && !TempString.Contains("<*ENDCHAPTER*>"))
        {
            GameObject.Find("SlideImage").GetComponent<Image>().enabled = false; //hide chapter title image if tags are not found in text
        }

        if (TempString.Contains("<*ENDCHAPTER*>"))//find text slides containing endchapter tag
        {
            TempString = TempString.Replace("<*ENDCHAPTER*>", "");//prevent the tag from showing in app
            GameObject.Find("SlideImage").GetComponent<Image>().enabled = true;//show the end chapter image (UI Element)

            GameObject.Find("SlideImage").GetComponent<Image>().sprite = (Sprite)SlideSprites[0];//change the image in UI Element
            FadeUI(GameObject.Find("SlideImage"));//start the fadein effect

        }

        if (TempString.Contains("<*IMAGE")) //find text slides containing image tag
        {  // tag must contain <*IMAGE_nameof*>
            GameObject.Find("SlideImage").GetComponent<Image>().enabled = true;//show the  image (UI Element)

            string WholeString = TempString.Substring(TempString.IndexOf("<*IMAGE"), TempString.IndexOf("*>", TempString.IndexOf("<*IMAGE")) - TempString.IndexOf("<*IMAGE") + 2);//variable containing the tag
            TempString = TempString.Replace(WholeString, "");//*prevent the tag from showing in app
            WholeString = WholeString.Replace("<*", "");//
            WholeString = WholeString.Replace("*>", "");//
            string[] ImageValues;
            ImageValues = WholeString.Split('_');//get each individual values from tag


            foreach (Sprite element in SlideSprites)
            {
                if (element.name == ImageValues[1])
                {
                    GameObject.Find("SlideImage").GetComponent<Image>().sprite = element;//change the image in UI Element
                }
            }

            FadeUI(GameObject.Find("SlideImage"));//start the fadein effect

        }

    }

    public void TAG_MUSIC()
    { //tag must contain <*MUSIC_nameof*>
        if (TempString.Contains("<*MUSIC") && !TempString.Contains("<*/MUSIC*>") || TempString.Contains("<*MUSIC") && TempString.Contains("<*/MUSIC*>")) //find text slides containing music tag or containing both music and stop music tags
        {
            string WholeString = TempString.Substring(TempString.IndexOf("<*MUSIC"), TempString.IndexOf("*>", TempString.IndexOf("<*MUSIC")) - TempString.IndexOf("<*MUSIC") + 2);//variable containing the tag

            GameObject.Find("Music").GetComponent<AudioSource>().enabled = true;//enable the music component
            if (TempString.Contains("<*MUSIC") && !TempString.Contains("<*/MUSIC*>"))// if text contains only music tag
            {

                WholeString = WholeString.Replace("<*", "");//
                WholeString = WholeString.Replace("*>", "");//
                MusicValues = WholeString.Split('_');//get each individual values from tag


                foreach (AudioClip element in SlideMusic)
                {
                    if (element.name == MusicValues[1])
                    {
                        GameObject.Find("Music").GetComponent<AudioSource>().clip = element;// add audio clip to music component

                    }
                }

                StopCoroutine(FadeOutMusicRoutine);//stop any ongoing coroutines
                FadeOutMusicRoutine = FadeMusic(false, GameObject.Find("Music").GetComponent<AudioSource>());
                FadeInMusicRoutine = FadeMusic(true, GameObject.Find("Music").GetComponent<AudioSource>());

                StartCoroutine(FadeInMusicRoutine);// start fading in music


            }
            if (TempString.Contains("<*MUSIC") && TempString.Contains("<*/MUSIC*>"))// if text contains both music and music end tags
            {
                WholeString = WholeString.Replace("<*", "");//
                WholeString = WholeString.Replace("*>", "");//
                MusicValues = WholeString.Split('_');//get each individual values from tag


                foreach (AudioClip element in SlideMusic)
                {
                    if (element.name == MusicValues[1])
                    {
                        GameObject.Find("Music").GetComponent<AudioSource>().clip = element;// add audio clip to music component

                    }
                }

                StopCoroutine(FadeOutMusicRoutine);//stop any ongoing coroutines
                FadeOutMusicRoutine = FadeMusic(false, GameObject.Find("Music").GetComponent<AudioSource>());

                StopCoroutine(FadeInMusicRoutine);//stop any fading coroutines
                StopCoroutine(FadeInOutMusicRoutine);//
                FadeInOutMusicRoutine = FadeInOutMusic(GameObject.Find("Music").GetComponent<AudioSource>(),MusicValues[1]);

                StartCoroutine(FadeInOutMusicRoutine);//start fading out and in music

            }
            this.GetComponent<SaveLoad>().SaveMusic(MusicValues[1]);
            TempString = TempString.Replace("<*"+WholeString+"*>", "");//*prevent the tag from showing in app
            TempString = TempString.Replace("<*/MUSIC*>", "");//prevent the tag from showing in app


        }
        else // if text only contains end music tag
       {
            if (TempString.Contains("<*/MUSIC*>")) //find text slides containing end music tag
            {
                TempString = TempString.Replace("<*/MUSIC*>", "");//prevent the tag from showing in app

                StopCoroutine(FadeInMusicRoutine);//stop any coroutines
                FadeOutMusicRoutine = FadeMusic(false, GameObject.Find("Music").GetComponent<AudioSource>());

                StartCoroutine(FadeOutMusicRoutine); //start fading out music


           } 

        }
    }
    public void TAG_SOUND()
    {   
        // tag must contain <*SOUND+Name+duration+volume*>
        if (TempString.Contains("<*SOUND"))
        {

            string WholeString = TempString.Substring(TempString.IndexOf("<*SOUND"), TempString.IndexOf("*>", TempString.IndexOf("<*SOUND")) - TempString.IndexOf("<*SOUND") + 2);//variable containing the tag
            TempString = TempString.Replace(WholeString, "");//*prevent the tag from showing in app
            WholeString = WholeString.Replace("<*", "");//
            WholeString = WholeString.Replace("*>", "");//
            SoundValues = WholeString.Split('_');//get each individual values from tag


            StopCoroutine(PlaySoundRoutine);// stop any coroutines
            PlaySoundRoutine = PlaySound(SoundValues[1], float.Parse(SoundValues[2]), float.Parse(SoundValues[3]));

            StartCoroutine(PlaySoundRoutine);//start playing audio
            

        }
        else
            if (TempString.Contains("<*CHAPTER"))
        {

            StopCoroutine(FadeOutSoundRoutine);
            FadeOutSoundRoutine = FadeMusic(false, GameObject.Find("Sounds").GetComponent<AudioSource>());

            StartCoroutine(FadeOutSoundRoutine);//start fade out sound

        }


    }
    #endregion

    #region UTILITY FUNCTIONS

    public void SetString(string text)//set the value of the story line
    {
        TempString = text;
    }

    public string GetString() // send the modified script
    {
        return TempString;
    }

    void ChapterTitle(bool state)//change the appearance of text if it is a chapter title.

    {
        if (state)//if text slide is chapter title
        {
            TextComponent.alignment = TextAnchor.MiddleCenter;//set the text in the middle
            TextComponent.resizeTextMaxSize = 122;//MAKE IT BIGGER!!
            TextComponent.GetComponent<Shadow>().enabled = true;
        }
        else//if text slide is normal
        {
            TextComponent.alignment = TextAnchor.UpperLeft;//normal alignment
            TextComponent.resizeTextMaxSize = 60;//MAKE IT NORMAL SIZE
            TextComponent.GetComponent<Shadow>().enabled = false;


        }
    }

    public void FadeUI(GameObject UIElement)// a function which fade in images in text slides
    {
        UIElement.GetComponent<Animator>().Play("FadeIn", -1, 0);
        UIElement.GetComponent<Animator>().Play("FadeIn");
    }

    void TextStyle(string style)//change the style of text
    {
        if (style == "bold")
        {
            TextComponent.fontStyle = FontStyle.Bold;
        }
        if (style == "italic")
        {
            TextComponent.fontStyle = FontStyle.Italic;
        }
        if (style == "normal")
        {
            TextComponent.fontStyle = FontStyle.Normal;
        }
    }
    IEnumerator FadeMusic(bool dir, AudioSource audio)//fades music in or out
    {
        if (dir)// if it is fade in
        {


            audio.volume = 0;//set volume to 0

            audio.Play();//play the sound

            while (audio.volume < 1)//fade in until volume is 1
            {
                // Debug.Log(audio.volume + " 1");

                audio.volume += 0.03f;
                yield return new WaitForSecondsRealtime(0.06f);
            }
        }
        else //if it is fade out
        {
            while (audio.volume > 0)//decrease the volume until it reaches 0
            {

                audio.volume -= 0.03f;
                yield return new WaitForSecondsRealtime(0.06f);
            }
            audio.Stop();//stop the music
        }

    }

    IEnumerator FadeInOutMusic(AudioSource audio,string audioName)// fades out previous music and fades in next music 
    {
        while (audio.volume > 0) // fade out sound until it reaches volume 0
        {

            audio.volume -= 0.03f;
            yield return new WaitForSecondsRealtime(0.06f);
        }
        foreach (AudioClip element in SlideMusic)
        {
            if (element.name == audioName)
            {
                GameObject.Find("Music").GetComponent<AudioSource>().clip = element;// add audio clip to music component

            }
        }

        audio.Play();//play the sound

        while (audio.volume < 1)//fade in until volume is 1 (100%)
        {

            audio.volume += 0.03f;
            yield return new WaitForSecondsRealtime(0.06f);
        }


    }
    IEnumerator PlaySound(string name, float duration, float volume)// function that plays a specific sound over a period of time with a specific volume
    {


        StopCoroutine(FadeOutSoundRoutine);

        GameObject.Find("Sounds").GetComponent<AudioSource>().enabled = true;//enable the sound component
        GameObject.Find("Sounds").GetComponent<AudioSource>().volume = volume;//set the volume
        foreach (Object el in SlideSound)//iterate through tag values
        {
            if (el.name == name)//if name matches with an existing sound name
            {
                GameObject.Find("Sounds").GetComponent<AudioSource>().clip = (AudioClip)el;//set the audioclip to component

            }
        }
        GameObject.Find("Sounds").GetComponent<AudioSource>().Play();//play the sound
        yield return new WaitForSecondsRealtime(duration);// wait until the sound is played
        StopCoroutine(FadeOutSoundRoutine);//stop any coroutines
        FadeOutSoundRoutine = FadeMusic(false, GameObject.Find("Sounds").GetComponent<AudioSource>());

        StartCoroutine(FadeOutSoundRoutine);//start fade out sound

    }

    #endregion
}
