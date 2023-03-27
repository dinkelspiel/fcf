using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using libfcf;

namespace FCF;

class Program {
    static void PrintError(InvalidTokenTypeException e, string sourceFilePath) {
        string[] filearr = File.ReadAllLines(sourceFilePath);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("error");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($": {e.Message}");
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
            case "test":
                string subPath = "tests/";
                string categoryPath = "serializer/";
                List<string> serializeTests = new List<string>();

                serializeTests.Add("simple");
                serializeTests.Add("simplecondensed");

                bool verbose = false;
                if(args.Length >= 2) {
                    if(args[1] == "verbose") {
                        verbose = true;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Serializer Tests");

                int i = 0;
                foreach(string test in serializeTests)
                {
                    i++;

                    var d = Parser.DeserializeObjectFromFile(subPath + categoryPath + test + ".fc");
                    string resultJSON = File.ReadAllText(subPath + categoryPath + test + "_result.json");
                    string resultFCF = File.ReadAllText(subPath + categoryPath + test + "_result.fc");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Test {i} '{test}': ");
                    if (Parser.SerializeObject(d) != resultFCF)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"FCF Failed");
                        if(verbose) {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            Console.WriteLine("Got:");
                            Console.WriteLine(Parser.SerializeObject(d));
                            Console.WriteLine("Expected:");
                            Console.WriteLine(resultFCF);
                        }
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"FCF Succeeded");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(", ");

                    if (Parser.SerializeObjectToJson(d) != resultJSON)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"JSON Failed");
                        if(verbose) {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            Console.WriteLine("Got:");
                            Console.WriteLine(Parser.SerializeObjectToJson(d));
                            Console.WriteLine("Expected:");
                            Console.WriteLine(resultJSON);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"JSON Succeeded");
                    }
                    Console.WriteLine();
                }

                categoryPath = "tokenizer/";
                List<string> tokenizerTest = new List<string>();

                tokenizerTest.Add("fullconfig");
                tokenizerTest.Add("doubleid");
                tokenizerTest.Add("idstring");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nTokenizer Tests");

                i = 0;
                foreach(string test in tokenizerTest) {
                    i++;
                    Token[] tokens = Tokenizer.TokenizeFromFile(subPath + categoryPath + test + ".fc");
                    string[] expected = File.ReadAllLines(subPath + categoryPath + test + ".txt");

                    List<KeyValuePair<string, string>> problems = new List<KeyValuePair<string, string>>();

                    int tokenIdx = -1;
                    foreach(var token in tokens) {
                        tokenIdx++;

                        string output = "";
                        if(token.GetType() == typeof(TokenString)) {
                            TokenString t = (TokenString)token; 
                            output += $"{tokenIdx} {token} \"{t.value}\"";
                        } else {
                            dynamic t = (dynamic)token;
                            output += $"{tokenIdx} {token} '{t.value}'";
                        }

                        bool problem = false;
                        if(tokenIdx < expected.Length) {
                            if(output != expected[tokenIdx]) {
                                problems.Add(new KeyValuePair<string, string>(output, expected[tokenIdx]));
                                problem = true;
                            }
                        }

                        if(verbose) {
                            if(problem) {
                                Console.ForegroundColor = ConsoleColor.Red;
                            } else {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.WriteLine(output);
                        }
                    }
                    
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Test {i} '{test}': ");
                    if(problems.Count > 0) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed");
                        foreach(KeyValuePair<string, string> p in problems) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"  Got:      {p.Key}");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"  Expected: {p.Value}");
                        }
                    } else {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Succeeded");
                    }
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
                    Parser.DeserializeObjectFromFile(args[1]);
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
                    var d = Parser.DeserializeObjectFromFile(args[1]);
                    Console.WriteLine(Parser.SerializeObjectToJson(d));
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
                    var d = Parser.DeserializeObjectFromFile(args[1]);

                    Console.WriteLine(Parser.SerializeObject(d));
                }
                catch (InvalidTokenTypeException e)
                {
                    PrintError(e, args[1]);
                }
                break;
            case "tokenize":
                if (args.Length < 2)
                {
                    Console.WriteLine("'tokenize' requries a filepath attached");
                    return;
                }

                Token[] ts = Tokenizer.TokenizeFromFile(args[1]);

                int tIdx = -1;
                foreach(Token token in ts) {
                    tIdx ++;
                    string output = "";
                    if(token.GetType() == typeof(TokenString)) {
                        TokenString t = (TokenString)token; 
                        output += $"{tIdx} {token} \"{t.value}\"";
                    } else {
                        dynamic t = (dynamic)token;
                        output += $"{tIdx} {token} '{t.value}'";
                    }
                    Console.WriteLine(output);
                }
                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }
    }
}
