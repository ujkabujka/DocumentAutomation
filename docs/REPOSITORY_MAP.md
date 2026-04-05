# Repository Map

This file explains the repository at the highest level.

The repository is easier to understand if you think of it as three connected learning layers:

1. the reusable framework
2. the PostgreSQL learning project
3. the real product foundation

## Big Picture

```text
DocumentAutomation/
|- BaseFrameWork/
|  |- BaseFramework.Core
|  |- BaseFramework.Generators
|  |- BaseFramework.Wpf
|  |- BaseFramework.WpfHost
|  |- BaseFramework.WebHost
|  |- BaseFramework.Core.Tests
|  `- BaseFramework.Wpf.Tests
|- BaseFrameWorkAndPosgreSQL/
|  `- EFCoreDemo
|- docs/
|  |- architecture/
|  `- *.md atlas files
|- src/
|  |- DocumentAutomation.App
|  |- DocumentAutomation.Application
|  |- DocumentAutomation.Domain
|  |- DocumentAutomation.Infrastructure
|  |- DocumentAutomation.Persistence
|  `- DocumentAutomation.Word
|- tests/
|  |- DocumentAutomation.Application.Tests
|  |- DocumentAutomation.Persistence.Tests
|  `- DocumentAutomation.Word.Tests
`- DocumentAutomation.slnx
```

## What Each Top-Level Folder Means

### `BaseFrameWork/`

This is the reusable dynamic UI framework.

Use this area when you want to learn:

- how inspectable metadata is defined
- how metadata is discovered
- how dynamic UI editors are selected
- how a runtime-built form can reuse the same inspector

This is the best first stop for framework learners.

### `BaseFrameWorkAndPosgreSQL/`

This is the EF Core and PostgreSQL study area.

Use this area when you want to learn:

- connection string resolution
- `DbContext` setup
- migrations
- CRUD
- relationships
- JSON/JSONB
- transactions

It is intentionally console-first and educational.

### `docs/`

This folder is the written guide to the repository.

It now contains:

- a repository map
- architecture trees
- workflow explanations
- grouped example guides
- Mermaid diagrams
- class responsibility notes
- a learning path
- a document template reader example

### `src/`

This is the real product foundation.

Use this area when you want to understand:

- the role-based desktop app
- the application/domain/infrastructure split
- how the framework is used in a real app
- how template scanning and document generation work

### `tests/`

This folder shows what the product promises to do.

These tests are especially useful for learners because they show the intended behavior in small focused pieces.

## Quick Mental Model

### If you want to learn the framework first

Open:

1. `BaseFrameWork/README.md`
2. `BaseFramework.Core`
3. `BaseFramework.WpfHost`
4. `BaseFramework.Core.Tests`
5. `BaseFramework.Wpf.Tests`

### If you want to learn PostgreSQL and EF Core first

Open:

1. `BaseFrameWorkAndPosgreSQL/README.md`
2. `BaseFrameWorkAndPosgreSQL/EFCoreDemo/README.md`
3. `EFCoreDemo/Program.cs`
4. `EFCoreDemo/Examples/StudyRunner.cs`
5. `EFCoreDemo/Data/AppDbContext.cs`

### If you want to understand the product app first

Open:

1. `src/DocumentAutomation.App`
2. `src/DocumentAutomation.Application`
3. `src/DocumentAutomation.Domain`
4. `src/DocumentAutomation.Infrastructure`
5. `src/DocumentAutomation.Persistence`
6. `src/DocumentAutomation.Word`

## Where The Main Ideas Live

### Dynamic UI metadata

- `BaseFrameWork/BaseFramework.Core/Attributes`
- `BaseFrameWork/BaseFramework.Core/Metadata`
- `BaseFrameWork/BaseFramework.Core/Services/ReflectionObjectMetadataProvider.cs`

### Stable key export/import

- `BaseFrameWork/BaseFramework.Core/Api/ParameterBridgeObject.cs`

### Permission-aware field visibility/editing

- `BaseFrameWork/BaseFramework.Core/Access`
- `src/DocumentAutomation.Application/Services/ApplicationAuthorizationService.cs`

### Reusable WPF inspector

- `BaseFrameWork/BaseFramework.Wpf/Controls`

### Example gallery

- `BaseFrameWork/BaseFramework.WpfHost`

### EF Core learning runner

- `BaseFrameWorkAndPosgreSQL/EFCoreDemo/Examples/StudyRunner.cs`

### Product domain

- `src/DocumentAutomation.Domain`

### Product workflow services

- `src/DocumentAutomation.Application`

### Product persistence

- `src/DocumentAutomation.Persistence`
- `src/DocumentAutomation.Infrastructure/Persistence`

### Word template scanning and generation

- `src/DocumentAutomation.Word/OpenXml`

## Source Versus Generated Noise

When reading this repo, ignore these generated folders:

- `bin/`
- `obj/`
- `TestResults/`

The real design lives in:

- `.cs`
- `.xaml`
- `.md`
- `.json`
- `.csproj`

## Best Entry Points For A New Reader

If you only have 15 minutes:

1. `Readme.md`
2. `docs/LEARNING_PATH.md`
3. `BaseFrameWork/README.md`
4. `BaseFrameWork/BaseFramework.WpfHost/MainWindow.xaml.cs`
5. `src/DocumentAutomation.App/MainWindow.xaml.cs`

If you want the whole story:

1. read the docs in `docs/`
2. run the WPF host
3. inspect the framework tests
4. run the EF Core demo
5. run the main product app
