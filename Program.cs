using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using libfcf;

namespace Config;

class Program {
    static void PrintError(InvalidTokenTypeException e, string sourceFilePath) {
        string[] filearr = FCF.SterilizeStringArray(File.ReadAllLines(sourceFilePath));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("error");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(": " + e.Message);
        Console.WriteLine();



        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(e.token.lineStart);
        Console.Write(" | ");

        foreach(var j in filearr[e.token.lineStart - 1]) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(j);
        }
        Console.WriteLine();



        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(e.token.lineStart + 1);
        Console.Write(" | ");

        int jIndex = -1;
        foreach(var j in filearr[e.token.lineStart]) {
            jIndex ++;
            if(jIndex >= e.token.charStart && jIndex < e.token.charEnd) {
                Console.ForegroundColor = ConsoleColor.Red;
            } else {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write(j);
        }
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Blue;
        for(var j = 0; j < e.token.lineStart.ToString().Length; j++) {
            Console.Write(" ");
        }
        Console.Write(" | ");

        for(int j = 0; j < e.token.charStart; j++) {
            Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        for(var j = 0; j < e.token.charEnd - e.token.charStart; j++) {
            Console.Write("^");
        }
        Console.WriteLine();



        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(e.token.lineStart + 2);
        Console.Write(" | ");

        foreach(var j in filearr[e.token.lineStart + 1]) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(j);
        }
        Console.WriteLine();
    }

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("No argument provided");
            return;
        }

        switch(args[0].ToLower())
        {
            case "color":
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Test");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("asd");
                Console.WriteLine();
                

                break;
            case "test":
                string subPath = "tests/";
                List<string> tests = new List<string>();

                tests.Add("simple");
                tests.Add("simplecondensed");

                int i = 0;
                foreach(string test in tests)
                {
                    i++;

                    var d = FCF.DeserializeObjectFromFile(subPath + test + ".fcf");
                    string resultJSON = File.ReadAllText(subPath + test + "_result.json");
                    string resultFCF = File.ReadAllText(subPath + test + "_result.fcf");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Test {i} '{test}': ");
                    if (FCF.SerializeObject(d) != resultFCF)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"FCF Failed");
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"FCF Succeeded");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(", ");

                    if (FCF.SerializeObjectToJson(d) != resultJSON)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"JSON Failed");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"JSON Succeeded");
                    }
                    Console.WriteLine();
                }


                break;
            case "validate":
                if(args.Length < 2)
                {
                    Console.WriteLine("'Validate' requries a filepath attached");
                    return;
                }

                try
                {
                    FCF.DeserializeObjectFromFile(args[1]);
                    Console.WriteLine("File is valid!");
                }
                catch(InvalidTokenTypeException e)
                {
                    PrintError(e, args[1]);
                }
                break;
            case "jsonify":
                if (args.Length < 2)
                {
                    Console.WriteLine("'jsonify' requries a filepath attached");
                    return;
                }

                try
                {
                    var d = FCF.DeserializeObjectFromFile(args[1]);
                    Console.WriteLine(FCF.SerializeObjectToJson(d));
                }
                catch (InvalidTokenTypeException e)
                {
                    PrintError(e, args[1]);
                }
                break;
            case "parse":
                if (args.Length < 2)
                {
                    Console.WriteLine("'parse' requries a filepath attached");
                    return;
                }

                try
                {
                    var d = FCF.DeserializeObjectFromFile(args[1]);

                    Console.WriteLine(FCF.SerializeObject(d));
                }
                catch (InvalidTokenTypeException e)
                {
                    PrintError(e, args[1]);
                }
                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }
    }
}
