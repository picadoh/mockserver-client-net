# Contributing

GitHub is the tool used for managing reviews and pull requests of MockServerClientNet (Mock-Server Client for .NET).

## Issues and Pull Requests

When opening an issue pull request, please make sure to address to the [maintainers of this repository](MAINTAINERS.md). The description must contain a clear explanation of what the change is about.

To submit a change, first look for an existing issue on the matter. If none is found, please open an issue explaining the goal and motivation.

## Development Guidelines

- Commit messages must be concise and clear, explaining the change
- The commit messages should reference the associated issue
- Each commit must have a well-defined scope, preferably with a small change
- Code must be kept as much clear and simple as possible
- The general spacing, alignment and naming conventions of the project must be respected
- Changes must be covered with tests
- Tests must succeed for each individual commit
- Binaries, packages or development auxiliary files (e.g. IDE metadata) must not be pushed to the repository
- If `.gitignore` is not considering files that must not be present in the repository, it should be edited before the commit
- Breaking changes shall be avoided
