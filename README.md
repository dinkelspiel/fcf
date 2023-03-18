# FCF CLI

## Usage

### Validate file

```
fcf validate ./examples/fullconfig.fc
```

### .fcf file to JSON

```
fcf jsonify ./examples/fullconfig.fc
```

### .fcf file to FCF

*Mostly for testing purposes*

```
fcf parse ./examples/fullconfig.fc
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