# First Template Use

Use this checklist to turn `dotnet-backend-template` into a real service.
Run through it right after creating a repository from the template, before adding production features.

In the final service, this file can be deleted or moved to `docs/` if the team wants to keep the checklist.
Delete it after the first service setup PR if the checklist is no longer useful.

## 0. Prefer `dotnet new`

The preferred way to create a service is the .NET template:

```bash
dotnet new install PANiXiDA.Templates.DotnetBackend

dotnet new panixida-backend \
  -n Company.Product.Service \
  --organization Company \
  --product Product \
  --module Module \
  --service-slug service \
  --repository-name service-repository \
  --registry-owner company \
  -o Company.Product.Service
```

When the service is created through `dotnet new`, template repository files are
already excluded and `README_TEMPLATE.md` is generated as `README.md`.

If the service is created by cloning or copying this repository manually, remove
template-only files from the final service repository:

- `.template.config/`;
- template-only `publish-template` job from `.github/workflows/ci.yml`;
- `template/`;
- `icon.png`;
- `version.json`, unless the service intentionally uses Nerdbank.GitVersioning;
- template repository `README.md`;
- `artifacts/`, if it was created by local build or package commands;
- generated package files, if any;
- `.tmp/`, if it was used for template testing.

For manual usage, keep `README_TEMPLATE.md` only long enough to rename it to
`README.md` for the service, then delete the `README_TEMPLATE.md` file.

This repository also contains repository-only examples for template maintainers.
They live under `RepositoryExamples` folders and are excluded when the service is
generated through `dotnet new`. If the service is created by cloning or copying
this repository manually, remove every `RepositoryExamples` folder.

## 1. Choose Target Names

Define these values before renaming anything:

| Item                  | Example                     | Used in                                       |
| --------------------- | --------------------------- | --------------------------------------------- |
| Service name          | `Billing`                   | README, API title, Helm release, Docker image |
| Root namespace        | `Company.Product`           | all `.cs`, `.csproj`, architecture tests      |
| Module name           | `Billing` or `Payments`     | `*.Module.*` projects, `src/Module` folder    |
| Solution name         | `Company.Product.Billing`   | `.slnx`, `dotnet restore/build/test` commands |
| Service slug          | `billing`                   | Helm, image names, gateway host               |
| Database name         | `billing`                   | appsettings, migrator, deployment config      |
| Repository owner/name | `company/billing-service`   | badges, CI, image repository                  |
| Registry URL          | `ghcr.io`                   | CI/CD, Helm images                            |
| Environments          | `development`, `production` | Helm values, CI/CD, OpenBao keys              |

Do not do a partial rename. Replace all template names first, then start implementing domain features.

If the service was generated through `dotnet new`, the basic replacements from
`.template.config/template.json` have already been applied. Still run the checks
below because service-specific values still must be reviewed manually.

## 2. Rename The Solution, Projects, And Folders

Current template names:

- `PANiXiDA.DotnetTemplate.slnx`;
- `src/Organization.Product.Host`;
- `src/Organization.Product.Common`;
- `src/Modules/Module/Organization.Product.Module.Contracts`;
- `src/Modules/Module/Organization.Product.Module.Domain`;
- `src/Modules/Module/Organization.Product.Module.Application`;
- `src/Modules/Module/Organization.Product.Module.Infrastructure`;
- `src/Modules/Module/Organization.Product.Module.Presentation`;
- `tests/Organization.Product.ArchitectureTests`;
- `tests/Organization.Product.Testing`;
- `tests/Modules/Module/Organization.Product.Module.UnitTests`;
- `tests/Modules/Module/Organization.Product.Module.IntegrationTests`;
- `tests/Modules/Module/Organization.Product.Module.FunctionalTests`;
- `tools/Organization.Product.Ef.Migrator`;
- `deploy/helm/dotnet-template`.

Actions:

1. If generated through `dotnet new`, verify the `.slnx`, project folders and `.csproj` files were renamed as expected.
2. If copied manually, rename the `.slnx` file to the final solution name.
3. Rename project folders and their `.csproj` files.
4. Update project paths inside the `.slnx` file.
5. Update all `ProjectReference` paths in `.csproj` files.
6. Update API and migrator Dockerfiles: `COPY`, `WORKDIR`, `RUN dotnet restore/build/publish`, `ENTRYPOINT`.
7. Update test project names and `InternalsVisibleTo`.
8. Rename Helm directory `deploy/helm/dotnet-template` to `deploy/helm/<service-slug>`.

Check:

```bash
rg "PANiXiDA\.DotnetTemplate|Organization\.Product|dotnet-template|dotnet-backend-template|panixida-templates"
```

After the rename, these values should not remain in code, configuration, CI/CD, Helm, or README unless intentionally kept in historical notes.

## 3. Update Namespaces And Assembly Names

Replace `Organization.Product` with the final root namespace in:

- `namespace ...`;
- `using Organization.Product...`;
- `<Using Include="Organization.Product...">`;
- `<InternalsVisibleTo Include="Organization.Product...">`;
- migration snapshot and migration designer files after real migrations are generated;
- functional, integration, unit, and architecture tests.

Keep the layer suffixes:

- `src/Organization.Product.Common`;
- `src/Modules/Module/Organization.Product.Module.Contracts`;
- `.Domain`;
- `.Application`;
- `.Infrastructure`;
- `.Presentation`;
- `.Host`.

Architecture tests discover modules by these suffixes. If you intentionally change them, update `tests/.../ArchitectureDefinition.cs`.

## 4. Create The First Real Feature

The generated template starts without sample business features or pre-generated migrations.
Create the first real feature slice only after final names and namespaces are set.

### Generate a CRUD slice

Define a public `AggregateRoot<TId>` in the module's Domain project under
`<ModuleNamespace>.Domain.<PluralFeature>`. The generator expects a Guid-backed
ID in the aggregate's namespace with `Value` and `Create(Guid)`, a public static
`Create(...)` returning `Result<TAggregate>`, and a public instance
`Update(...)` returning `Result`. Parameters must match writable aggregate
properties by name. Use scalar parameters for scalar-backed properties and the
value object's type for compound properties. A private setter is supported.

The generator supports scalar properties and value objects that expose a scalar
`Value` or `Name` and a corresponding `Create` factory. For a compound value
object, it matches each public static `Create` parameter to a readable property
by name, scalar type, and nullability. The factory returns `Result<TValueObject>`
and the aggregate method accepts the value object itself. For example,
`Address.Create(string city, string street)` binds to `City` and `Street`.

Compound value objects become nested API objects and are stored in one
PostgreSQL `jsonb` column per aggregate property. The generator calls the
factory when constructing commands' domain values and when reading them into
the aggregate. To expose a compound value in a response, include its aggregate
property in the projection. List sorting uses top-level scalar fields.
Entity references and entity collections are excluded from the root CRUD
fields. Generate child collection operations explicitly with `children`.

In `tools/<Root>.CodeGenerator/appsettings.json`, configure one or more entries
in `jobs`. Each job uses a single Domain project and can contain several models.
Use a separate job for each additional Domain project. `pathToModels` accepts
the project directory or its `.Domain.csproj`, relative to the repository root.
Each model uses its full C# name and specifies its own route and read
projections. Both projections include `Id`.

```json
{
  "jobs": [
    {
      "pathToModels": "src/Modules/Catalog/Acme.Store.Catalog.Domain",
      "models": [
        {
          "aggregate": "Acme.Store.Catalog.Domain.Products.Product",
          "route": "products",
          "detailsFields": ["Id", "Name", "Description"],
          "listFields": ["Id", "Name"]
        },
        {
          "aggregate": "Acme.Store.Catalog.Domain.Categories.Category",
          "route": "categories",
          "detailsFields": ["Id", "Name"],
          "listFields": ["Id", "Name"]
        }
      ],
      "generatedFiles": ["CreateCommand", "UpdateCommand"],
      "templates": {
        "CreateCommand": "Templates/Application/Write/Create/Create__Resource__Command.cs.sbn"
      },
      "outputDirectory": {
        "CreateCommand": "src/Modules/{ModuleName}/{Module}.Application/{Feature}/Write/Create"
      },
      "overwriteExisting": false,
      "deleteGenerated": false,
      "needMigrate": false
    }
  ]
}
```

The example selects two artifacts to illustrate selective generation. The
commented job in `appsettings.json` lists all available artifacts for a full
CRUD. Omitted artifacts may still be required by the generated code.

- `templates` overrides the Scriban template for an artifact. Relative paths are
  resolved from the generator project. Keep custom templates in
  `Templates/Custom` or another directory outside the four bundled layers.
- `outputDirectory` overrides the destination directory for an artifact.
  Relative paths are resolved from the repository root. Use `{Module}` for the
  full module namespace, `{ModuleName}` for its last segment, and `{Feature}`
  or `{Resource}` for the selected model's plural or singular name. The generator
  keeps its conventional file name. Configure output directories inside the
  repository.
- `overwriteExisting` allows replacing existing files for the selected
  artifacts. Its default is `false`.
- `deleteGenerated` removes only the selected files whose first line still
  identifies the scaffolded aggregate. It works without building Domain and
  cannot be combined with `overwriteExisting`.
- `needMigrate` is reserved for future use. Run the EF migrator separately.

Run the project from the solution directory, using the generated project name.

```bash
dotnet run --project tools/Acme.Store.CodeGenerator
```

For a full CRUD, the tool creates C# files for Create, GetDetails, GetList,
Update, and Delete.
GetList includes pagination and sorting. Repository, EF model, and endpoint
discovery use the template's existing module registrations. The generator
checks the output paths for all jobs before applying any changes.
Review the HTTP and persistence contracts, add feature tests, and use the EF
migrator once the model is ready. Commit the resulting C# files with the feature.

### Generate CRUD for a child entity collection

Add a `children` entry to an aggregate model. The commented job in
`appsettings.json` includes the complete `Child*` artifact list and an example
for `Product.Variants`. Use a separate job when different collections need
different artifact selections. Each child requires:

- `collection`, a public collection property whose elements derive from
  `Entity<TId>`. Use a typed Guid ID with `Value` and `Create(Guid)`.
- `resource`, the singular name for generated types and files, such as
  `ProductVariant`. Choose a unique name for each endpoint resource.
- `route`, the lowercase child route segment, such as `variants`.
- `createMethod`, `updateMethod`, and `deleteMethod` for the write artifacts
  you select. Define these public instance methods on the aggregate. Create
  returns `Result<TChildId>`. Update and delete take the typed child ID first
  and return `Result`. Any remaining parameters follow the same field
  conventions as aggregate CRUD.
- `detailsFields` and `listFields`, each including `Id`.

Write handlers use the parent repository and call the aggregate's methods.
The EF configuration maps the collection with `OwnsMany` and a separate table.
Read handlers use the parent's read model to check that the aggregate exists.
Generate the parent repository and read database model in the same job, or
reuse them from an existing root CRUD. Use a mutable backing collection, such as `_variants`
for the `Variants` property, so EF can track additions and removals.

For a route such as `/products/{productId}/variants`, the generator provides
POST and paged GET on the collection, and GET, PUT, and DELETE on an item.
Item access is scoped to the parent. A missing parent returns 404, and an
existing parent without children returns an empty page. Run the EF migrator
after generating the collection configuration.

### Feature layout

Use this structure for a feature:

```text
src/Modules/<Module>/<Root>.<Module>.Domain/<Feature>/
src/Modules/<Module>/<Root>.<Module>.Application/<Feature>/
src/Modules/<Module>/<Root>.<Module>.Infrastructure/Persistence/Features/<Feature>/
src/Modules/<Module>/<Root>.<Module>.Presentation/Features/<Feature>/
tests/Modules/<Module>/<Root>.<Module>.UnitTests/...
tests/Modules/<Module>/<Root>.<Module>.IntegrationTests/...
tests/Modules/<Module>/<Root>.<Module>.FunctionalTests/...
```

For the first feature, add only the files the service actually needs:

- domain model, value objects, policies and specifications;
- application commands, queries, handlers and validators;
- infrastructure repositories, EF configurations, read models and mappers;
- presentation endpoints, request/response contracts and mappers;
- unit, integration and functional tests for the touched behavior.

Do not add placeholder entities, endpoints, repositories or tests that are not part of the real domain.

## 5. Rename DbContext And Create Migrations

Current template classes:

- `ModuleWriteDbContext`;
- `ModuleReadDbContext`.

Actions:

1. Rename DbContext classes to final service names, for example `<ServiceName>WriteDbContext` and `<ServiceName>ReadDbContext`.
2. Update registrations in:
   - `src/...Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`;
   - `src/...Infrastructure/DependencyInjection/HostBuilderExtensions.cs`;
   - `tools/...Ef.Migrator/Program.cs`;
   - integration and functional test fixtures.
3. Verify that no template migrations exist in `src/...Infrastructure/Persistence/Core/Migrations`.
4. Generate the first migration only after the real EF model is added.
5. Check `tools/...Ef.Migrator/appsettings.json`:
   - `Ef:ProjectPath`;
   - `Ef:MigrationsDirectory`;
   - `GenerateMigrations`;
   - `ApplyMigrations`;
   - `ConnectionStrings:PostgreSqlConnectionString`.

If the service does not use PostgreSQL or EF Core, remove the migrator project, the migration job in Helm, and related package references.

## 6. Configure Appsettings

Files to check:

- `src/...Host/appsettings.json`;
- `src/...Host/appsettings.Development.json`;
- `tools/...Ef.Migrator/appsettings.json`;
- `src/...Host/Properties/launchSettings.json`;
- `tools/...Ef.Migrator/Properties/launchSettings.json`.

Required actions:

1. Replace `dotnet-template` in connection strings and `OTEL_SERVICE_NAME`.
2. Keep local development passwords in `appsettings.json` if that is the team convention.
3. Update `ScalarConfiguration:Title`.
4. Remove `MongoDbConnectionString` if MongoDB is not used.
5. Remove gRPC endpoint `Kestrel:Endpoints:Grpc` if the service does not expose gRPC.
6. Check ports `8080`, `8081`, `launchSettings.json`, and Helm `service.containerPort`.
7. Keep or regenerate the host project local settings identifier only if Visual Studio tooling requires it.

Check:

```bash
rg "dotnet-template|MongoDbConnectionString" src tools deploy
```

Keep MongoDB matches only when the final service uses MongoDB.

## 7. Check Dockerfiles

Files:

- `src/...Host/Dockerfile`;
- `tools/...Ef.Migrator/Dockerfile`.

Check and update:

- `COPY` paths for all `.csproj` files;
- `RUN dotnet restore`;
- `WORKDIR`;
- `RUN dotnet build`;
- `RUN dotnet publish`;
- `ENTRYPOINT` with the final `.dll` name;
- `EXPOSE 8081`, if gRPC is no longer needed;
- `codegen write`, if code generation is no longer used.

The CI/CD pipeline prepares package feed configuration for Docker builds. Keep Dockerfiles aligned with that pipeline behavior.

Check:

```bash
docker build -f src/<HostProject>/Dockerfile -t <service-slug>-api:local .
docker build -f tools/<MigratorProject>/Dockerfile -t <service-slug>-ef-migrator:local .
```

## 8. Update CI/CD

File:

- `.github/workflows/ci.yml`.

Check:

- target branches `main`, `development`;
- reusable workflows `PANiXiDA-Infrastructure/ci-cd`;
- `paths-ignore` for image values;
- `format`, `tests`, and `helm` jobs;
- API build job;
- migrator build job, if the migrator remains;
- Telegram notification job, if notifications are needed.

Configure GitHub repository variables:

- `HELM_VALUES_PATH`;
- `HELM_COMMON_VALUES_FILE`;
- `HELM_DEVELOPMENT_RELEASE_NAME`;
- `HELM_PRODUCTION_RELEASE_NAME`;
- `SERVICE_FOLDER`;
- `SERVICE_NAME`;
- `MIGRATOR_FOLDER`, if the migrator remains;
- `MIGRATOR_NAME`, if the migrator remains;
- `REGISTRY_URL`;
- `SONAR_PROJECT_KEY`;
- `TELEGRAM_CHAT_ID`, if notifications are needed.

Configure protected CI/CD values used by the workflow:

- `REGISTRY_USER`;
- `REGISTRY_TOKEN`;
- `SONAR_TOKEN`;
- `TELEGRAM_BOT_TOKEN`, if notifications are needed.

Register the repository in `inventory/sonarqube/repositories.json` in
`PANiXiDA-Infrastructure/core-platform`. After that change is merged, the
`SonarQube Repositories Sync` workflow creates the SonarQube project and sets
`SONAR_PROJECT_KEY` and `SONAR_TOKEN` in this repository. Both values are
required because the SonarQube Quality Gate blocks image builds and publishing.

If the service does not use the migrator, remove `build-migrator`, its dependencies, and migrator image updates.

## 9. Update Helm Deployment

Files:

- `deploy/helm/<service-slug>/values.yaml`;
- `deploy/helm/<service-slug>/values-development.yaml`;
- `deploy/helm/<service-slug>/values-production.yaml`;
- `deploy/helm/<service-slug>/images-development.yaml`;
- `deploy/helm/<service-slug>/images-production.yaml`.

Check and replace:

- API image repository;
- migrator image repository;
- registry pull configuration;
- application environment name;
- OpenBao role;
- OpenBao application key;
- OpenBao registry key;
- gateway host;
- gateway HTTPS section name;
- `OTEL_SERVICE_NAME`;
- `OTEL_EXPORTER_OTLP_ENDPOINT`;
- `replicaCount`;
- `resources`;
- `probes`;
- migration job settings.

If MongoDB, a message bus, or external APIs are used in production, add the required environment values and provider mappings.

Reset image tags for the new service until CI/CD updates them:

```yaml
tag: "0"
```

Check:

```bash
rg "dotnet-template|panixida-templates/dotnet-backend-template" deploy .github
```

## 10. Update README

File:

- `README.md`.

The generated `README.md` comes from `README_TEMPLATE.md` in the template
repository. Treat it as a starting point for the real service documentation.

Fill or remove all placeholders:

- `<ServiceName>`;
- `<OWNER>`;
- `<REPOSITORY>`;
- `<REPOSITORY_URL>`;
- `<SolutionName>`;
- `<short purpose>`;
- `<target audience / consumers>`;
- `<main value / main scenario>`;
- `<database-name>`;
- `<ExternalService*>`;
- `<API_DOCS_URL>`;
- `<LicenseName>`;
- maintainer contacts.

Update:

- overview;
- feature list;
- architecture description;
- project structure after renaming;
- quick start commands;
- local configuration;
- local infrastructure;
- configuration table;
- endpoint tables;
- authentication and authorization;
- domain model;
- database section;
- Docker commands;
- observability;
- Helm deployment;
- CI/CD variables and protected values;
- environments;
- troubleshooting;
- template replacement checklist.

Remove README sections that do not apply to the service:

- MongoDB;
- gRPC;
- code generation;
- metrics/tracing;
- migrator.

Check:

```bash
rg "<ServiceName>|<OWNER>|<REPOSITORY>|<REPOSITORY_URL>|<SolutionName>|<LicenseName>|Organization\.Product|dotnet-template" README.md
```

## 11. Check Package Management And SDK

Files:

- `global.json`;
- `Directory.Build.props`;
- `Directory.Build.targets`;
- `Directory.Packages.props`;
- `.editorconfig`;
- `.gitattributes`;
- `.gitignore`;
- `.dockerignore`.

Check:

- SDK version in `global.json` is available locally and in CI;
- `TargetFramework` matches the chosen .NET version;
- `TreatWarningsAsErrors` remains enabled;
- package versions are relevant for the service;
- unused package versions are removed;
- package feeds are available in CI and Docker build;
- `.dockerignore` does not exclude files required by Docker build;
- `.gitattributes` keeps the expected line endings.

## 12. Create Missing Service Files

Create only what the final service needs:

- new `.slnx` with correct project paths;
- renamed `.csproj` files;
- first real feature slice;
- first real EF migration;
- `.env.example`, if the team uses local Docker runs through an env file;
- additional Helm values, if there are more environments;
- documentation in `docs/`, if README becomes too large.

Keep these out of the final service changes:

- real `.env` files;
- database dumps;
- local logs;
- `bin/`;
- `obj/`;
- `TestResults/`;
- temporary JSON/TXT/MD files.

## 13. Final Check Before The First PR

Check that template values are gone:

```bash
rg "PANiXiDA\.DotnetTemplate|Organization\.Product|dotnet-template|dotnet-backend-template|panixida-templates|<ServiceName>|<OWNER>|<REPOSITORY>|<SolutionName>|<LicenseName>"
```

Check that no accidental sample feature names remain:

```bash
rg "Users|User" src tests
```

There should be no matches unless the real service domain intentionally uses those words.

Check diff format:

```bash
git diff --check
git status --short
```

Check .NET:

```bash
dotnet restore <SolutionName>.slnx
dotnet format <SolutionName>.slnx --severity warn --no-restore
dotnet format <SolutionName>.slnx --verify-no-changes --severity warn --no-restore
dotnet build <SolutionName>.slnx --configuration Release
dotnet test --solution <SolutionName>.slnx --configuration Release
```

Check Docker:

```bash
docker build -f src/<HostProject>/Dockerfile -t <service-slug>-api:local .
docker build -f tools/<MigratorProject>/Dockerfile -t <service-slug>-ef-migrator:local .
```

Check Helm:

```bash
helm lint <chart-source> --values deploy/helm/<service-slug>/values.yaml
```

If the shared chart lives in another repository, use the same chart source that `.github/workflows/ci.yml` uses.

## 14. Recommended Work Order

1. Choose names and environments.
2. Generate the service with `dotnet new` or remove template-only files after manual clone.
3. Verify solution, projects, folders, and namespaces.
4. Update `.slnx`, `.csproj`, and Dockerfiles where needed.
5. Create the first real feature slice.
6. Rename DbContext and generate the first real migration when the EF model is ready.
7. Configure appsettings and deployment values.
8. Update Helm values.
9. Update GitHub Actions variables and protected values.
10. Rewrite README for the service.
11. Run restore, format, build, test, Docker build, and Helm validation.
12. Check with `rg` for leftover template values.
13. Remove local temporary artifacts and open the first PR.
