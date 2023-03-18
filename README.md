# Fjord Config Format

## How to use

### Deserialize from file

```c#
using fcflib;

Dictionary<string, dynamic> deserializedObject = FCF.DeserializeObjectFromFile($filePath);
```

### Deserialize from memory

```c#
using fcflib;

string memory = """
version = 1.4,
name = FCF,
buildOptions = [
    "Release",
    "Beta",
    "Alpha"
]
""";

Dictionary<string, dynamic> deserializedObject = FCF.DeserializeObjectFromMemory(memory);
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

```
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
- Implied top level object eg. no need to surround top level object in curly braces "{}"
- Names are identifiers not strings to have a clear distinction
- Usage of equals "=" instead of colon ":" to more clearly designate an assign operation