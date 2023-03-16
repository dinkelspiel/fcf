using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;


class Token
{
    
}

class TokenIdentifier : Token
{
    public string value;

    public TokenIdentifier(string value)
    {
        this.value = value;
    }
}

class TokenString : Token
{
    public string value;

    public TokenString(string value)
    {
        this.value = value;
    }
}

class TokenNumber : Token
{
    public float value;

    public TokenNumber(float value) {
        this.value = value;
    }
}

class TokenBoolean : Token {
    public bool value;
    public TokenBoolean(bool value) {
        this.value = value;
    }
}

class TokenAssign : Token
{
    
}

class TokenComma : Token
{
    
}

class TokenArrayStart : Token
{
    
}

class TokenArrayEnd : Token
{
    
}

class TokenDictStart : Token
{
    
}

class TokenDictEnd : Token
{
    
}

class TokenEnumStart : Token
{

}

class TokenEnumEnd : Token 
{

}

class Program {
    public static Token[] TokenizeFile(string path)
    {
        string[] fileContentsArray = File.ReadAllLines(path);
        string fileContents = "";
        foreach(string line in fileContentsArray)
        {
            string line2 = line.Split("//").First();
            line2 = line2 + "\n";
            
            fileContents += line2.TrimStart().TrimEnd();
        }

        string newFileContents = "";
        bool isString = false;
        foreach (char c in fileContents)
        {
            if (c == '"')
            {
                isString = !isString;
            }

            if (!isString && c == ' ')
            {
                continue;
            }

            newFileContents += c;
        }

        fileContents = "{" +  newFileContents  + "}";

        List<Token> stack = new List<Token>();
        
        string currentWord = "";
        isString = false;
        
        var boolvalues = new String[2]{"true", "false"};

        foreach (char c in fileContents)
        {
            switch (c)
            {
                case '=':
                    if (boolvalues.Contains(currentWord))
                    {
                        stack.Add(new TokenBoolean(currentWord == "true"));    
                    }
                    else
                    {
                        stack.Add(new TokenIdentifier(currentWord));
                    }

                    currentWord = "";
                    stack.Add(new TokenAssign());
                    break;
                case ',':
                    if (currentWord != "")
                    {
                        float value = 0f;
                        if(float.TryParse(currentWord, out value)) {
                            stack.Add(new TokenNumber(value));
                            currentWord = "";
                        } else {
                            if (boolvalues.Contains(currentWord))
                            {
                                stack.Add(new TokenBoolean(currentWord == "true"));    
                            }
                            else
                            {
                                stack.Add(new TokenIdentifier(currentWord));
                            }

                            currentWord = "";
                        }
                    }

                    stack.Add(new TokenComma());
                    break;
                case '[':
                    stack.Add(new TokenArrayStart());
                    break;
                case ']':
                    if (currentWord != "")
                    {
                        float value = 0f;
                        if(float.TryParse(currentWord, out value)) {
                            stack.Add(new TokenNumber(value));
                            currentWord = "";
                        } else {
                            if (boolvalues.Contains(currentWord))
                            {
                                stack.Add(new TokenBoolean(currentWord == "true"));    
                            }
                            else
                            {
                                stack.Add(new TokenIdentifier(currentWord));
                            }

                            currentWord = "";
                        }
                    }

                    stack.Add(new TokenArrayEnd());
                    break;
                case '{':
                    stack.Add(new TokenDictStart());
                    break;
                case '}':
                    if (currentWord != "")
                    {
                        float value = 0f;
                        if(float.TryParse(currentWord, out value)) {
                            stack.Add(new TokenNumber(value));
                            currentWord = "";
                        } else {
                            if (boolvalues.Contains(currentWord))
                            {
                                stack.Add(new TokenBoolean(currentWord == "true"));    
                            }
                            else
                            {
                                stack.Add(new TokenIdentifier(currentWord));
                            }

                            currentWord = "";
                        }
                    }

                    stack.Add(new TokenDictEnd());
                    break;
                case '(':
                    stack.Add(new TokenEnumStart());
                    break;
                case ')':
                    if (currentWord != "")
                    {
                        if (boolvalues.Contains(currentWord))
                        {
                            stack.Add(new TokenBoolean(currentWord == "true"));    
                        }
                        else
                        {
                            stack.Add(new TokenIdentifier(currentWord));
                        }

                        currentWord = "";
                    }

                    stack.Add(new TokenEnumEnd());
                    break;

                default:
                    if (c != '"')
                    {
                        currentWord += c;
                    }
                    
                    if (c == '"')
                    {
                        if (isString)
                        {
                            stack.Add(new TokenString(currentWord));
                            currentWord = "";
                        }
                        
                        isString = !isString;
                    }

                    break;
            }
        }

        return stack.ToArray();
    }

    static dynamic ParseDict(Token[] tokens)
    {
        Dictionary<string, dynamic> json_object = new Dictionary<string, dynamic>();
        var t = tokens[0];
        if(t.GetType() == typeof(TokenDictEnd)) {
            return new dynamic[] {json_object, tokens.ToList().Skip(1).ToArray()};
        }

        while(true) {
            var json_key = tokens[0];
            if(json_key.GetType() == typeof(TokenIdentifier)) {
                tokens = tokens.ToList().Skip(1).ToArray();
            } else {
                Console.WriteLine("Expected key got: " + json_key);
                Environment.Exit(3);
            }

            if(tokens[0].GetType() != typeof(TokenAssign)) {
                Console.WriteLine("Excpected assing after key in dict got: " + t);
                Environment.Exit(4);
            }

            var tmp = ParseInternal(tokens.ToList().Skip(1).ToArray());
            var json_value = tmp[0];
            tokens = tmp[1];

            if(json_value.GetType() == typeof(TokenIdentifier)) {
                Console.WriteLine("Excpected string | number | bool got identifier: " + ((TokenIdentifier)json_value).value);
                Environment.Exit(4);
            }

            // json_object.Add((string)(((TokenIdentifier)json_key).value), json_value);

            if(json_value.GetType() == typeof(TokenString) || json_value.GetType() == typeof(TokenNumber) || json_value.GetType() == typeof(TokenBoolean) || json_value.GetType() == typeof(TokenIdentifier)) {
                json_object.Add((string)(((TokenIdentifier)json_key).value), ((dynamic)json_value).value);
            } else {
                json_object.Add((string)(((TokenIdentifier)json_key).value), json_value);
            }
            
            t = tokens[0];
            if(t.GetType() == typeof(TokenDictEnd)) {
                return new dynamic[] {json_object, tokens.ToList().Skip(1).ToArray()};
            } else if(t.GetType() != typeof(TokenComma)) {
                Console.WriteLine("Expected assign after pair in object got: " + t);
                Environment.Exit(5);
            }

            tokens = tokens.ToList().Skip(1).ToArray();
        }
    }

    static dynamic ParseArray(Token[] tokens)
    {
        List<dynamic> json_array = new List<dynamic>();

        var t = tokens[0];
        if(t.GetType() == typeof(TokenArrayEnd)) {
            return new dynamic[] {json_array, tokens.ToList().Skip(1).ToArray()};
        }

        while(true) {
            var tmp = ParseInternal(tokens);
            var json = tmp[0];
            tokens = tmp[1];

            if(json.GetType() == typeof(TokenString) || json.GetType() == typeof(TokenNumber) || json.GetType() == typeof(TokenBoolean) || json.GetType() == typeof(TokenIdentifier)) {
                json_array.Add(((dynamic)json).value);
            } else {
                json_array.Add(json);
            }

            t = tokens[0];
            if(t.GetType() == typeof(TokenArrayEnd)) {
                return new dynamic[] {json_array, tokens.ToList().Skip(1).ToArray()};
            } else if(t.GetType() != typeof(TokenComma)) {
                Console.WriteLine("Expected comma after object in array");
                Environment.Exit(1);
            } else {
                tokens = tokens.ToList().Skip(1).ToArray();
            }
        }
    }

    static dynamic ParseInternal(Token[] tokens) {
        var t = tokens[0];

        // Console.WriteLine("Parsing Array");
        if (t.GetType() == typeof(TokenArrayStart))
        {
            // Console.WriteLine("Parsing Array: " + t);
            return ParseArray(tokens.ToList().Skip(1).ToArray());
        } else if (t.GetType() == typeof(TokenDictStart))
        {
            // Console.WriteLine("Parsing Dict: " + t);
            return ParseDict(tokens.ToList().Skip(1).ToArray());
        }
        else
        {
            return new dynamic[] {t, tokens.ToList().Skip(1).ToArray()};
        }
    }

    static string SerializeObject(dynamic tokens) {
        string output = "";
        foreach(dynamic i in tokens) {
            if(i.GetType() == typeof(Dictionary<string, dynamic>)) {
                output += ("{ ");
                foreach(KeyValuePair<string, dynamic> j in (Dictionary<string, dynamic>)i) {
                    if(j.Value.GetType() == typeof(TokenIdentifier) || j.Value.GetType() == typeof(TokenString) || j.Value.GetType() == typeof(TokenNumber) || j.Value.GetType() == typeof(TokenBoolean)) { 
                        if(j.Value.GetType() == typeof(TokenString)) {
                            output += (j.Key + ": \"" + (dynamic)j.Value.value + "\", ");
                        } else {
                            output += (j.Key + ": " + j.Value.value + ", ");
                        }
                    } else {
                        if(j.Value.GetType() == typeof(List<dynamic>)) {
                            output += (j.Key + ": [" + SerializeObject(j.Value) + "], ");
                        } else {
                            output += (j.Key + ": {" + SerializeObject(j.Value) + "}, ");
                        }
                    }
                    // output += "\n";
                }
                output += (" } ");
            } else if(i.GetType() == typeof(List<dynamic>)) {
                foreach(dynamic j in i) {
                    if(j.GetType() == typeof(List<dynamic>)) {
                        if(j.GetType() == typeof(List<dynamic>)) {
                            output += "[" + SerializeObject(j) + "], "; 
                        } else {
                            output += "{" + SerializeObject(j) + "}, ";
                        }                    
                        // output += ", ";
                    } else {
                        if(j.GetType() == typeof(TokenString)) {
                            output += "\"" + j + "\", ";
                        } else {
                            output += j + ", ";
                        }
                    }
                }
            }
        }
        return output;
    }

    static dynamic Parse(Token[] tokens) {
        return ParseInternal(tokens)[0];
    }

    static void Main(string[] args)
    {
        var stack = TokenizeFile("./examples/fullconfig.fcf");

        var d = Parse(stack);
        // // Console.WriteLine();        
        // Console.WriteLine(SerializeObject(d));
        // // Console.WriteLine();
        // // foreach(var i in d[0]) {
        // //     Console.WriteLine(i);
        // // }
        // Console.WriteLine(d[0]["arr"][1]);
        Console.WriteLine(d["user"][0]["name"]);

        // foreach(var d in stack) {
        //     if(d.GetType() == typeof(TokenString) || d.GetType() == typeof(TokenNumber) || d.GetType() == typeof(TokenBoolean) || d.GetType() == typeof(TokenIdentifier)) {
        //         Console.WriteLine(d + ": " + ((dynamic)d).value);
        //     } else {
        //         Console.WriteLine(d);
        //     }
        // }
        
    }
}
