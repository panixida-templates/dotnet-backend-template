# Architecture tests

The suite checks module and layer dependencies, project references, host composition,
and conventions for domain types, application handlers, persistence, endpoints, and test code.

The starter service contains one module. Dependency checks cover every module discovered
under ``src/Modules/``. Rules for repositories, endpoints, and other optional features apply
when the matching code is present.

Run from the solution directory.

```shell
dotnet test --project tests/Organization.Product.ArchitectureTests/Organization.Product.ArchitectureTests.csproj --configuration Release
```

The conventions are based on the
[Tactical Heroes API architecture tests](https://github.com/tactical-heroes/api/tree/63b7492e/tests/PANiXiDA.TacticalHeroes.ArchitectureTests).
