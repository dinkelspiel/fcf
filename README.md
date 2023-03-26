# FCF CLI

## Usage

### Validate file

```
fcf validate ./examples/fullconfig.fc
```

### .fcfcf file to JSON

```
fcf jsonify ./examples/fullconfig.fc
```

### .fc file to FCF

*Mostly for testing purposes*

```
fcf parse ./examples/fullconfig.fc
```

### Parse tokens for FCF file

```
fcf tokenize ./examples/fullconfig.fc
```

### Run tests

This command parses the files in ./tests and compares them to the _result files to see if they match.

```
fcf test
```

## Build 

```
dotnet build
```