# Fjord Config Format

## How to use

```c#
Token[] tokens = TokenizeFile($filePath);
Dictionary<string, dynamic> SerializedObject = SerializeObject(tokens);
```

## JSON Comparison

```json
{
    "persons": [
        {
            "name": "keii",
            "age": 16,
            "alive": true
        },
        {
            "name": "keii2",
            "age": 17,
            "alive": false
        }
    ]
}
```

```json
persons = [
    {
        name = "keii",
        age = 16,
        alive = true
    },
    {
        name = "keii2",
        age = 17,
        alive = false
    }
]
```

Differences:
- Implied top level object eg. no need to surround object in curly braces "{}"
- Names are identifiers not strings to have a clear distinction
- Usage of equals "=" instead of colon ":" to more clearly designate a assign operation