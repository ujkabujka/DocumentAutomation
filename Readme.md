# DocumentAutomation

`DocumentAutomation` is a learning-focused repository that now contains three connected parts:

1. `BaseFrameWork/`
   A reusable metadata-driven UI framework for C# objects.
2. `BaseFrameWorkAndPosgreSQL/`
   A PostgreSQL + EF Core study project.
3. `src/` and `tests/`
   The first serious foundation of the document automation product.

The main idea of the repository is simple:

- describe data with metadata
- let the UI build itself from that metadata
- connect that flow to roles, permissions, templates, PostgreSQL, and document generation

## Start Here

If you are new to the repository, read these files in order:

1. [`docs/REPOSITORY_MAP.md`](docs/REPOSITORY_MAP.md)
2. [`docs/ARCHITECTURE_TREE.md`](docs/ARCHITECTURE_TREE.md)
3. [`docs/WORKFLOWS.md`](docs/WORKFLOWS.md)
4. [`docs/DIAGRAMS.md`](docs/DIAGRAMS.md)
5. [`docs/CLASS_RESPONSIBILITIES.md`](docs/CLASS_RESPONSIBILITIES.md)
6. [`docs/LEARNING_PATH.md`](docs/LEARNING_PATH.md)
7. [`docs/TEMPLATE_READER_EXAMPLE.md`](docs/TEMPLATE_READER_EXAMPLE.md)

There is also an earlier product note in:

- [`docs/architecture/role-based-document-automation-foundation.md`](docs/architecture/role-based-document-automation-foundation.md)

## Repository Shape

```text
DocumentAutomation/
|- BaseFrameWork/                 Reusable dynamic UI framework
|- BaseFrameWorkAndPosgreSQL/     EF Core + PostgreSQL learning project
|- docs/                          Repository atlas and architecture notes
|- src/                           Main document automation product
|- tests/                         Product tests
`- DocumentAutomation.slnx        Root product solution
```

## What To Run

### BaseFramework example gallery

```powershell
dotnet run --project BaseFrameWork\BaseFramework.WpfHost\BaseFramework.WpfHost.csproj
```

This is the easiest place to see:

- scalar fields
- dropdowns
- nested objects
- collections
- command invocation
- role-aware metadata
- runtime template-driven forms

### BaseFramework tests

```powershell
dotnet test BaseFrameWork\BaseFramework.sln
```

### PostgreSQL learning project

```powershell
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- help
```

### Main document automation app

```powershell
dotnet run --project src\DocumentAutomation.App\DocumentAutomation.App.csproj
```

If PostgreSQL is not configured, the app falls back to demo mode with seeded users, roles, projects, templates, and a sample `.docx` template.

## Demo Template

A sample document template is committed at:

- `src/DocumentAutomation.App/data/templates/system-acceptance-report.docx`

It contains placeholders such as:

- `$project_name$`
- `$test_date$`
- `$image:test_setup$`
- `$table:test_results$`

See [`docs/TEMPLATE_READER_EXAMPLE.md`](docs/TEMPLATE_READER_EXAMPLE.md) for the full walkthrough.

## Main Questions This Repo Answers

- How can a C# object describe its own editable UI?
- How can metadata be discovered by reflection, source generation, or runtime construction?
- How can one inspector support both ordinary models and template-driven dynamic forms?
- How can PostgreSQL patterns move from a learning console app into a real desktop architecture?
- How can a `.docx` template become a dynamic input form and then a generated output document?

## Important Note About Trees In The Docs

The documentation trees intentionally hide build output folders such as:

- `bin/`
- `obj/`
- `TestResults/`

Those folders are generated artifacts, not source design.
