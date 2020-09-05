using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Recap : MonoBehaviour {
    public TextAsset StoryText;
    public TextAsset RecapText;
    string[] lines;
    string[] NodeNames;
    string[] RecapLines;
    public Text RecapUI;
    int i = 0;
    // Use this for initialization
    void Start () {
            lines = StoryText.text.Split('\n');
            NodeNames = new string[lines.Length];
            foreach (string line in lines)
            {
                if (line.Contains("title"))
                {
                    NodeNames[i] = line;
                    NodeNames[i] = NodeNames[i].Substring(NodeNames[i].IndexOf("title") + 9);
                    NodeNames[i] = NodeNames[i].Replace("\"", string.Empty);
                    NodeNames[i] = NodeNames[i].Replace(",", string.Empty);

                    i++;
                }
            }
            createEntries();
            createRecap();
        

    }
	
	// Update is called once per frame
    public void initialize()
    {
        foreach (string node in NodeNames)
        {
                PlayerPrefs.SetInt(node, 0);
            
        }

    }
    void createEntries()
    {
        foreach (string node in NodeNames)
        {
            if (!PlayerPrefs.HasKey(node))
            {
                PlayerPrefs.SetInt(node, 0);
            }
        }
    }
    void createRecap()
    {
        RecapLines = RecapText.text.Split('\n');
    }
    public void AddLinesToUI()
    {
        string WholeRecap="";
        int index = 0;
        foreach (string node in NodeNames)
        {
            if (PlayerPrefs.GetInt(node)==1)
            {   
                WholeRecap += RecapLines[index].Substring(RecapLines[index].IndexOf(':')+1) + "\n"+"\n";
            }
            index++;
        }
        if (WholeRecap == "")
        {
            RecapUI.text = "Progress further in the story to get entries here";
            RecapUI.alignment = TextAnchor.MiddleCenter;
        }
        else
        { RecapUI.text = WholeRecap;
            RecapUI.alignment = TextAnchor.UpperLeft;
        }

    }
    void Update () {
		
	}
}
