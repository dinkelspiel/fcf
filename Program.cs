using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static FjordConfigFormat.FCF;
using Newtonsoft.Json;
using FjordConfigFormat;

namespace Config;

class Program {
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("No argument provided");
            return;
        }

        switch(args[0].ToLower())
        {
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
                    if (SerializeObject(d) != resultFCF)
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

                    if (SerializeObjectToJson(d) != resultJSON)
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
                    Console.WriteLine("File is invalid!");
                    Console.WriteLine("Error: InvalidTokenTypeException");
                    Console.WriteLine("Message: " + e.Message);
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
                    Console.WriteLine(SerializeObjectToJson(d));
                }
                catch (InvalidTokenTypeException e)
                {
                    Console.WriteLine("File is invalid!");
                    Console.WriteLine("Error: InvalidTokenTypeException");
                    Console.WriteLine("Message: " + e.Message);
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
                    var d = DeserializeObjectFromFile(args[1]);
                    Console.WriteLine(SerializeObject(d));
                }
                catch (InvalidTokenTypeException e)
                {
                    Console.WriteLine("File is invalid!");
                    Console.WriteLine("Error: InvalidTokenTypeException");
                    Console.WriteLine("Message: " + e.Message);
                }
                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }
    }
}
