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
    public static Token[] TokenizeFromFile(string path) {
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

        return TokenizeFromMemory(newFileContents);
    }

    public static Token[] TokenizeFromMemory(string fileContents)
    {
        fileContents = "{" + fileContents + "}";

        List<Token> stack = new List<Token>();
        
        string currentWord = "";
        bool isString = false;
        
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

    static dynamic[] ParseDict(Token[] tokens)
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

            var tmp = Parse(tokens.ToList().Skip(1).ToArray());
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

    static dynamic[] ParseArray(Token[] tokens)
    {
        List<dynamic> json_array = new List<dynamic>();

        var t = tokens[0];
        if(t.GetType() == typeof(TokenArrayEnd)) {
            return new dynamic[] {json_array, tokens.ToList().Skip(1).ToArray()};
        }

        while(true) {
            var tmp = Parse(tokens);
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

    static dynamic[] Parse(Token[] tokens) {
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

    static string SerializeObjectToJson(dynamic obj) {
        string output = "";

        int i = -1;
        if(obj.GetType() == typeof(Dictionary<string, dynamic>)) {
            output += "{";
            foreach(KeyValuePair<string, dynamic> kvp in obj) {
                i++;
                if(kvp.Value.GetType() == typeof(List<dynamic>) || kvp.Value.GetType() == typeof(Dictionary<string, dynamic>)) {
                    output += $"\"{kvp.Key}\": {SerializeObjectToJson(kvp.Value)}";
                } else {
                    if(kvp.Value.GetType() == typeof(string)) {
                        output += $"\"{kvp.Key}\": \"{kvp.Value}\"";
                    } else if(kvp.Value.GetType() == typeof(bool) ){
                        var boolval = kvp.Value ? "true" : "false";
                        output += $"\"{kvp.Key}\": {boolval}";
                    } else {
                        output += $"\"{kvp.Key}\": {kvp.Value}";
                    }
                }
                if(i != obj.Count - 1) {
                    output += ", ";
                }
            }
            output += "}";
        } else if(obj.GetType() == typeof(List<dynamic>)) {
            output += "[";
            foreach(dynamic val in obj) {
                i++;
                if(val.GetType() == typeof(List<dynamic>) || val.GetType() == typeof(Dictionary<string, dynamic>)) {
                    output += $"{SerializeObjectToJson(val)}";
                } else {
                    if(val.GetType() == typeof(string)) {
                        output += $"\"{val}\"";
                    } else if(val.GetType() == typeof(bool) ){
                        output += val ? "true" : "false";
                    } else {
                        output += val;
                    }
                }
                if(i != obj.Count - 1) {
                    output += ", ";
                }
            }
            output += "]";
        }

        return output;
    } 

    static string SerializeObject(dynamic obj) {
        string output = "";

        int i = -1;
        if(obj.GetType() == typeof(Dictionary<string, dynamic>)) {
            output += "{";
            foreach(KeyValuePair<string, dynamic> kvp in obj) {
                i++;
                if(kvp.Value.GetType() == typeof(List<dynamic>) || kvp.Value.GetType() == typeof(Dictionary<string, dynamic>)) {
                    output += $"{kvp.Key} = {SerializeObject(kvp.Value)}";
                } else {
                    if(kvp.Value.GetType() == typeof(string)) {
                        output += $"{kvp.Key} = \"{kvp.Value}\"";
                    } else if(kvp.Value.GetType() == typeof(bool) ){
                        var boolval = kvp.Value ? "true" : "false";
                        output += $"{kvp.Key} = {boolval}";
                    } else {
                        output += $"{kvp.Key} = {kvp.Value}";
                    }
                }
                if(i != obj.Count - 1) {
                    output += ", ";
                }
            }
            output += "}";
        } else if(obj.GetType() == typeof(List<dynamic>)) {
            output += "[";
            foreach(dynamic val in obj) {
                i++;
                if(val.GetType() == typeof(List<dynamic>) || val.GetType() == typeof(Dictionary<string, dynamic>)) {
                    output += $"{SerializeObject(val)}";
                } else {
                    if(val.GetType() == typeof(string)) {
                        output += $"\"{val}\"";
                    } else if(val.GetType() == typeof(bool) ){
                        output += val ? "true" : "false";
                    } else {
                        output += val;
                    }
                }
                if(i != obj.Count - 1) {
                    output += ", ";
                }
            }
            output += "]";
        }

        return output;
    } 

    static dynamic DeserializeObject(Token[] tokens) {
        return Parse(tokens)[0];
    }

    static dynamic DeserializeObjectFromFile(string filePath) {
        var stack = TokenizeFromFile(filePath);
        return DeserializeObject(stack);
    }

    static dynamic DeserializeObjectFromMemory(string memory) {
        var stack = TokenizeFromMemory(memory);
        return DeserializeObject(stack);
    }

    static void Main(string[] args)
    {
        var DeserializedObject = DeserializeObjectFromFile("../../../examples/fullconfig.fc");
        Console.WriteLine("\nFCF:");
        Console.WriteLine(SerializeObject(DeserializedObject));
        Console.WriteLine("\nJSON:");
        Console.WriteLine(SerializeObjectToJson(DeserializedObject));
    }
}
