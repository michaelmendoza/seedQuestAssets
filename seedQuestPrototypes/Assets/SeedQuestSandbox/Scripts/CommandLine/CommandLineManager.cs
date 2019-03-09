﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SeedQuest.SeedEncoder;
using SeedQuest.Interactables;
using SeedQuest.Debugger;


public static class CommandLineManager
{
    public static bool isInUse = false;

    // Initialize the dictionary 
    public static Dictionary<string, Func<string, string>> commands =
        new Dictionary<string, Func<string, string>>
    {
        {"help", help},
        {"print", print},
        {"get", getValue},
        {"gethelp", getHelp},
        {"moveplayer", movePlayer},
        {"loadscene", loadScene},
        {"random", random},
        {"gamestate", setGameState},
        {"showcolliders", showBoxColliders}
    };

    // Here's a template for an example of command. 
    //  For a command to work, it needs to be added to the above dictionary,
    //  and the dictionary key for the function needs to be all lowercase
    public static string templateCommand(string input)
    {
        // Put code here
        return input;
    }

    // Prints out a list of available command line commands
    public static string help(string input)
    {
        string returnString = "Available commands:";
        foreach (string key in commands.Keys)
        {
            returnString += "\n" + key;
        }
        return returnString;
    }

    // Just used for displaying information to the user
    public static string print(string input)
    {
        Debug.Log("Hello from print() " + input);

        return input;
    }

    // Loads the scene specified by input, if it exists. A scene must be in the build settings
    //  for this command to work
    public static string loadScene(string input)
    {
        if (input == "")
            return "No scene specified";

        SceneManager.LoadScene(input);
        return "Loading scene: " + input;
    }

    // Example of running tests in command line, not actually funcitonal yet.
    public static string seedTests(string input)
    {
        MonoBehaviour seedBehavior = new SeedToByteTests();

        // TO DO: this can potentially cause memory problems, since Destroy() can't be used here
        string passedString = seedBehavior.GetComponent<SeedToByteTests>().runAllTests();
        return passedString;
    }

    // Show box colliders on all interactables
    public static string showBoxColliders(string input)
    {
        if (input == "true")
            DebugManager.Instance.showBoundingBoxes = true;
        else if (input == "false")
            DebugManager.Instance.showBoundingBoxes = false;
        else
            DebugManager.Instance.showBoundingBoxes = !DebugManager.Instance.showBoundingBoxes;

        return "";
    }

    // Placeholder function to move the player when playerManager gets imported into seedquest-sandbox
    public static string movePlayer(string input)
    {
        // Replace this line with the reference to the player object to move
        //GameObject player = new GameObject();

        string[] stringInputs = input.Split(null);
        int[] intInput = new int[3];
        bool validInts = false;
        for (int i = 0; i < intInput.Length; i++)
        {
            validInts = int.TryParse(stringInputs[i], out intInput[i]);
            Debug.Log("int " + i + ": " + intInput[i]);
        }

        Vector3 coordinates = new Vector3(intInput[0], intInput[1], intInput[2]);

        if (!validInts)
        {
            return "Invalid coordinates entered";
        }

        // Replace this with code relevant to changing the player position
        //else
        //{ player.transform.position = coordinates; }

        return "Moving player to " + intInput[0] + " " + intInput[1] + " " + intInput[2];
    }

    // Generates random number between 1 and 100
    public static string random(string input)
    {
        float rand = UnityEngine.Random.Range(1.0f, 100.0f);
        int randI = (int)rand;
        return "Your random number is: " + randI;
    }

    // Returns values from various manager scripts, for example 'get gamestate' returns the gamestate
    public static string getValue(string input)
    {
        string returnStr = "";
        if (CommandLineGetValues.values.ContainsKey(input))
            returnStr = CommandLineGetValues.values[input](input);
        else
            returnStr = "Value not found";
        
        return returnStr;
    }

    // Returns a list of the values available through the 'get' command
    public static string getHelp(string input)
    {
        string returnString = "Available values:";
        foreach (string key in CommandLineGetValues.values.Keys)
        {
            returnString += "\n" + key;
        }
        return returnString;
    }

    // Set the gamestate. string.StartsWith() is used so that the user input doesn't need to be
    //  perfectly correct to set some states (ex: 'rehears' will work with either 'rehearsal' 
    //  or 'rehearse' as the user input. Some states are commented out because they don't exist
    //  in this build.
    public static string setGameState(string input)
    {
        if (input.StartsWith("prev"))
        {
            GameManager.State = GameManager.PrevState;
            return "Game state set to previous state.";
        }
        if (input.StartsWith("paus"))
        {
            GameManager.State = GameState.Pause;
            return "Game state set to Pause.";
        }
        if (input.StartsWith("inter"))
        {
            GameManager.State = GameState.Interact;
            return "Game state set to Interact.";
        }
        if (input.StartsWith("end"))
        {
            GameManager.State = GameState.End;
            return "Game state set to End.";
        }
        if (input.StartsWith("menu"))
        {
            GameManager.State = GameState.Menu;
            return "Game state set to Menu.";
        }
        if (input.StartsWith("play"))
        {
            GameManager.State = GameState.Play;
            return "Game state set to Play.";
        }

        return "Game state by name of '" + input + "' not found.";
    }

    // From here all functions are 'fluff' functions - they are just here for fun, and 
    //  are not necessary for debug purposes

    // Dictionary for 'fluff' functions. They are not necessary, and are just for fun
    //  These are in a separate dictionary so that they won't be displayed to the 
    //  user from the 'help' command output.
    public static Dictionary<string, Func<string, string>> fluffCommands =
        new Dictionary<string, Func<string, string>>
    {
        {"helpfluff", helpFluff},
        {"hello", hello},
        {"jello", jello},
        {"hi", hi},
        {"quick", quickBrown},
        {"lorem", loremIpsum},
        {"dog", asciiDog}
    };

    // Help - for fluff functions
    public static string helpFluff(string input)
    {
        string returnString = "Available commands:";
        foreach (string key in fluffCommands.Keys)
        {
            returnString += "\n" + key;
        }
        return returnString;
    }

    // Say hello to the user - fluff function
    public static string hello(string input)
    {
        return "Hello world!";
    }

    // Say jello to the user - fluff funciton
    public static string jello(string input)
    {
        return "Well jello to you too! Nice to meet you!";
    }

    // Ask them how they're doing - fluff funciton
    public static string hi(string input)
    {
        return "Hi! How are you today?";
    }

    // Prints the quick brown fox line - fluff funciton
    public static string quickBrown(string input)
    {
        return "The quick brown fox jumps over the lazy dog.";
    }

    // Prints out lorem ipsum - fluff funciton
    public static string loremIpsum(string input)
    {
        return "Lorem ipsum dolor sit amet";
    }

    // This ascii dog is actually pretty helpful in debugging the UI part of the command line
    public static string asciiDog(string input)
    {
        string ascii = "      ,";
        ascii += "\n      |`-.__ ";
        ascii += "\n      /   '  _/ ";
        ascii += "\n      ****`";
        ascii += "\n     /       } ";
        ascii += "\n    /    \\  / ";
        ascii += "\n \\ /`     \\\\\\ ";
        ascii += "\n  `\\      /_\\\\ ";
        ascii += "\n   `~~~~``~`";
        return ascii;
    }

}
