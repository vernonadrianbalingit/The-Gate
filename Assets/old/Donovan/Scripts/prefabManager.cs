using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class prefabManager : MonoBehaviour
{

    private InputField inField;
    private PrefabDetection pd;
    private string input;
    private string[] inputInfo = new string[2];
    private int i;

    // Start is called before the first frame update
    void Start()
    {
        pd = FindObjectOfType<PrefabDetection>();
        input = "";
        i = 0;
    }

    //When button is pressed confirming selection
    public void buttonPressed()
    {
        inField = FindObjectOfType<InputField>();
        input = inField.text;
        inField.text = "";

        //need to check for space for 2 step commands?
        if (input.Contains("cd"))
        {
            inputInfo[0] = input.Substring(0, 2);
            inputInfo[1] = input.Substring(3);
        }
        else if (input.Contains(" "))
        {
            inputInfo = input.Split(' ');
        }
        else
        {
            inputInfo[0] = input;
        }

        //need to check if contains space and has extra command for second input like cd
        switch ((inputInfo[0]))
        {
            case "prnt":
                pd.printObjects();
                break;

            case "cd":
                Debug.Log(inputInfo[1]);
                pd.changeFolder(inputInfo[1]);
                Debug.Log("Changed Folder to " + inputInfo[1]);
                break;

            case "setobj":
                //tries to turn second input into int
                if (int.TryParse(inputInfo[1], out int pos))
                {
                    i = pos;
                    Debug.Log("set object");
                    break;
                }
                Debug.Log("Failed");
                break;

            case "getobj":
                Debug.Log(pd.getObject(i));
                break;

            case "update":
                pd.updateObjects();
                Debug.Log("Updated List");
                break;

            case "comp":
//                pd.changeComponentCheck(inputInfo[1]);
                Debug.Log("Changed comp to search for to " + inputInfo[1]);
                break;

            default:
                Debug.LogWarning("Not a command");
                break;
        }
    }

}
