﻿using IVFI3X.Cells;
using IVFI3X.Player;
using System.Xml.Serialization;
using IVFI3X.Map;

Console.WriteLine("Please input your name:");
string name = Console.ReadLine();
Player player = new Player(name);

XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
List<Player> existingPlayers = null;

string projectDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\"));
string path = Path.Combine(projectDirectory, "player_data.xml");



bool noPlayersSaved = false;
try
{
    using (StreamReader reader = new StreamReader(path))
    {
        try
        {
            existingPlayers = (List<Player>)serializer.Deserialize(reader);
        }
        catch (InvalidOperationException e)
        {
            existingPlayers = new List<Player>();
            noPlayersSaved = true;
        }
    }
}
catch (FileNotFoundException)
{
    throw new FileNotFoundException("The player_data.xml is missing!");
}




if (!noPlayersSaved && existingPlayers.Any(p => p.Name == player.Name))
{
    player = existingPlayers.First(p => p.Name == player.Name);
    Console.WriteLine($"Player {player.Name} is already in the database. Best score: {player.BestScore}");
}
else
{

    existingPlayers.Add(player);

    await using (StreamWriter writer = new StreamWriter(path))
    {
        serializer.Serialize(writer, existingPlayers);
    }

    Console.WriteLine("Saved your player in the database.");
}


while (true)
{
    Console.WriteLine();
    Console.WriteLine("Enter the number of what you want to do:");
    Console.WriteLine("1 => Start a new game");
    Console.WriteLine("2 => Look at my top scores");
    Console.WriteLine("3 => Exit the app");

    string numInput = Console.ReadLine();
    int num;

    if (int.TryParse(numInput, out num))
    {
        switch (num)
        {
            // new game
            case 1:
                {
                    Console.WriteLine("Start a new game");
                    Console.WriteLine("Choose the difficulty: easy,medium,hard");
                    PlayCell[,] playMap = Map.GeneratePlayingMap(GetDifficuiltyInput());
  
                    int score = PlayGame(playMap);

                    Player myPlayer = existingPlayers.Find(p => p.Name == player.Name)!;
                    myPlayer.AddScore(score);

                    using StreamWriter writer = new StreamWriter(path);
                    serializer.Serialize(writer, existingPlayers);
                    break;
                }
            // show scores
            case 2:
                ShowTopScores(player);
                break;
            // exit
            case 3:
                Environment.Exit(0);
                break;
            //wrong input
            default:
                Console.WriteLine("The number you imputed is not in the list");
                break;
        }
    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a valid integer.");
    }

}




int PlayGame(PlayCell[,] playMap)
{

    int score = Map.NonVisibleCount(playMap);

    while (Map.NonVisibleCount(playMap) != 0)
    {
        Console.WriteLine();
        Console.WriteLine("You have to guess the cells marked with X");
        PrintPlayMap(playMap);
        int[] nextStep = GetNextStep();

        if (playMap[nextStep[0], nextStep[1]].Value == nextStep[2])
        {
            playMap[nextStep[0], nextStep[1]].IsVisible = true;
            Console.WriteLine("Good guess ^^");
        }
        else
        {
            Console.WriteLine("Wrong guess :p");
            score--;
        }
    }

    Console.WriteLine("Congratulations! You have finished the game.");
    Console.WriteLine("Your score was: " + score);
    return score;

}




int[] GetNextStep()
{
    int[] nextStep = new int[3];
    Console.WriteLine("Enter your next step:");
    Console.Write("X coordinate (1-9)(sor): ");

    int x;
    while (true)
    {
        string xInput = Console.ReadLine();
        if (int.TryParse(xInput, out x) && x is >= 1 and <= 9)
        {
            nextStep[0] = x - 1;
            break;
        }
        Console.WriteLine("Invalid input. Please enter a valid integer for X coordinate.");

    }

    Console.Write("Y coordinate (1-9)(oszlop): ");

    int y;
    while (true)
    {
        string yInput = Console.ReadLine();
        if (int.TryParse(yInput, out y) && y is >= 1 and <= 9)
        {
            nextStep[1] = y - 1;
            break;
        }
        Console.WriteLine("Invalid input. Please enter a valid integer for Y coordinate.");
    }

    Console.Write("Value (1-9): ");

    int value;
    while (true)
    {
        string valueInput = Console.ReadLine();
        if (int.TryParse(valueInput, out value) && value is >= 1 and <= 9)
        {
            nextStep[2] = value;
            break;
        }
        Console.WriteLine("Invalid input. Please enter a valid integer between 1 and 9 for the value.");


    }

    return nextStep;
}


string GetDifficuiltyInput()
{
    string difficulty;
    while (true)
    {
        difficulty = Console.ReadLine();
        if (difficulty is "easy" or "medium" or "hard")
        {
            break;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid difficulty level: easy, medium, or hard.");
        }
    }

    return difficulty;
}







void PrintGenerateMap(GenerateCell[,] myArray)
{
    for (int i = 0; i < myArray.GetLength(0); i++)
    {
        for (int j = 0; j < myArray.GetLength(1); j++)
        {
            Console.Write(myArray[i, j].Value + " ");
        }
        Console.WriteLine();
    }
}

void PrintPlayMap(PlayCell[,] myArray)
{
    for (int i = 0; i < myArray.GetLength(0); i++)
    {
        for (int j = 0; j < myArray.GetLength(1); j++)
        {
            if (myArray[i, j].IsVisible)
            {
                Console.Write(myArray[i, j].Value + " ");
            }
            else
            {
                Console.Write("X" + " ");
            }

        }
        Console.WriteLine();
    }
}





void ShowTopScores(Player player)
{
    Console.WriteLine("Your best scores:");

    bool noScores = true;

    for (var index = 0; index < player.TopScores.Count; index++)
    {
        var score = player.TopScores[index];
        Console.WriteLine(index + 1 + ". " + score + " points");
        noScores = false;
    }

    if (noScores)
    {
        Console.WriteLine("You don't have saved scores yet");
    }
}


