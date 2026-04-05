# Learning Path

This file suggests a reading and running order for a new developer.

There are two good paths:

- the short path for orientation
- the full path for deep understanding

## Short Path: 30 To 45 Minutes

### Goal

Understand what the repo is, what each area does, and where to go next.

### Order

1. Read the root `Readme.md`.
2. Read `docs/REPOSITORY_MAP.md`.
3. Read `docs/ARCHITECTURE_TREE.md`.
4. Read `docs/WORKFLOWS.md`.
5. Skim `docs/DIAGRAMS.md`.
6. Open `BaseFrameWork/README.md`.
7. Open `BaseFrameWork/BaseFramework.WpfHost/MainWindow.xaml.cs`.
8. Open `src/DocumentAutomation.App/MainWindow.xaml.cs`.

By the end of this path you should know:

- why the repo has more than one solution
- what the framework does
- what the EF demo does
- what the product foundation does

## Full Path: Recommended Order

## Stage 1. Learn The Repository Shape

Read:

1. `Readme.md`
2. `docs/REPOSITORY_MAP.md`
3. `docs/ARCHITECTURE_TREE.md`
4. `docs/WORKFLOWS.md`
5. `docs/DIAGRAMS.md`
6. `docs/CLASS_RESPONSIBILITIES.md`

Reason:

- this prevents random file jumping
- you get the whole mental map before reading code

## Stage 2. Learn BaseFramework From Easy To Hard

### Read first

1. `BaseFrameWork/README.md`
2. `BaseFramework.Core/Attributes/InspectableMemberAttribute.cs`
3. `BaseFramework.Core/Metadata/InspectableMemberMetadata.cs`
4. `BaseFramework.Core/Services/ReflectionObjectMetadataProvider.cs`
5. `BaseFramework.Wpf/Controls/ObjectInspectorControl.xaml.cs`
6. `BaseFramework.Wpf/Controls/DefaultInspectorEditorRegistry.cs`

### Then run

```powershell
dotnet run --project BaseFrameWork\BaseFramework.WpfHost\BaseFramework.WpfHost.csproj
```

### Then inspect examples in this order

1. `BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`

   Read these classes in order:

   - `BasicDocumentExampleModel`
   - `NestedWorkflowExampleModel`
   - `ApprovalSettingsExampleModel`
   - `WorkflowStepExample`
   - `RoleAwareExampleModel`

2. `BaseFramework.WpfHost/Models/TemplateDrivenExampleForm.cs`
3. `BaseFramework.WpfHost/Models/InspectorTestHierarchy.cs`

### What to look for

- stable keys
- companion attributes
- runtime metadata
- rejection-based visibility
- nested object reuse
- collection editing
- command invocation

## Stage 3. Learn From The Framework Tests

Run:

```powershell
dotnet test BaseFrameWork\BaseFramework.sln
```

Read:

1. `BaseFramework.Core.Tests/InspectableMetadataTests.cs`
2. `BaseFramework.Wpf.Tests/InspectorEditorRegistryTests.cs`

Why these matter:

- they explain behavior in small examples
- they prove generator parity, access rules, and grouping rules

## Stage 4. Learn The PostgreSQL Study Project

Read:

1. `BaseFrameWorkAndPosgreSQL/README.md`
2. `BaseFrameWorkAndPosgreSQL/EFCoreDemo/README.md`
3. `EFCoreDemo/Configuration/ConnectionStringResolver.cs`
4. `EFCoreDemo/Data/AppDbContext.cs`
5. `EFCoreDemo/Examples/StudyRunner.cs`

Run:

```powershell
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- help
```

Then this order:

1. `info`
2. `canconnect`
3. `migrate`
4. `seed`
5. `basic`
6. `queries`
7. `relationships`
8. `json`
9. `rawsql`
10. `transaction`

What to learn here:

- how EF Core maps objects to PostgreSQL
- how JSONB is used
- how a learning runner can teach each concept separately

## Stage 5. Learn The Product Domain

Read:

1. `src/DocumentAutomation.Domain/Security/SecurityModels.cs`
2. `src/DocumentAutomation.Domain/Templates/TemplateModels.cs`
3. `src/DocumentAutomation.Domain/Documents/DocumentModels.cs`

Goal:

- learn the vocabulary of the product before reading services

## Stage 6. Learn The Product Application Layer

Read:

1. `src/DocumentAutomation.Application/Contracts/ApplicationContracts.cs`
2. `src/DocumentAutomation.Application/Models/ApplicationModels.cs`
3. `src/DocumentAutomation.Application/Services/ApplicationAuthorizationService.cs`
4. `src/DocumentAutomation.Application/Services/DocumentPreparationService.cs`
5. `src/DocumentAutomation.Application/DynamicForms/DynamicDocumentFormObject.cs`

Goal:

- understand how the app prepares editable document fields

## Stage 7. Learn The Product Infrastructure And Persistence

Read:

1. `src/DocumentAutomation.Infrastructure/Configuration/DocumentAutomationConnectionResolver.cs`
2. `src/DocumentAutomation.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
3. `src/DocumentAutomation.Infrastructure/Demo/DemoServices.cs`
4. `src/DocumentAutomation.Persistence/DocumentAutomationDbContext.cs`
5. `src/DocumentAutomation.Persistence/Seed/DocumentAutomationSeedData.cs`

Goal:

- understand how the app switches between demo mode and database mode

## Stage 8. Learn The Word Template Path

Read:

1. `docs/TEMPLATE_READER_EXAMPLE.md`
2. `src/DocumentAutomation.Word/OpenXml/DemoTemplateDocumentWriter.cs`
3. `src/DocumentAutomation.Word/OpenXml/OpenXmlTemplateScanner.cs`
4. `src/DocumentAutomation.Word/OpenXml/OpenXmlDocumentGenerator.cs`

Look at the real sample template:

- `src/DocumentAutomation.App/data/templates/system-acceptance-report.docx`

Goal:

- connect placeholders to runtime fields and generated documents

## Stage 9. Learn The Product UI

Read:

1. `src/DocumentAutomation.App/App.xaml.cs`
2. `src/DocumentAutomation.App/MainWindow.xaml`
3. `src/DocumentAutomation.App/MainWindow.xaml.cs`
4. `src/DocumentAutomation.App/Views/AdminPage.xaml.cs`
5. `src/DocumentAutomation.App/Views/DesignerPage.xaml.cs`
6. `src/DocumentAutomation.App/Views/DocumentGenerationPage.xaml.cs`

Run:

```powershell
dotnet run --project src\DocumentAutomation.App\DocumentAutomation.App.csproj
```

Suggested demo sequence:

1. start in demo mode
2. inspect the current user and role banner
3. open the Designer page and rescan the sample template
4. open the Generate page
5. prepare the document
6. inspect missing and autofilled fields
7. generate output

## Stage 10. Learn The Product Tests

Run:

```powershell
dotnet test DocumentAutomation.slnx
```

Read:

1. `tests/DocumentAutomation.Application.Tests/AuthorizationAndPreparationTests.cs`
2. `tests/DocumentAutomation.Persistence.Tests/PersistenceModelTests.cs`
3. `tests/DocumentAutomation.Word.Tests/OpenXmlWorkflowTests.cs`

Why:

- they show intended behavior without UI noise

## Best Example Set By Topic

### Simple scalar model

- `BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`
- class: `BasicDocumentExampleModel`

### Nested object model

- same file
- class: `NestedWorkflowExampleModel`

### Collection model

- same file
- class: `NestedWorkflowExampleModel`
- also `InspectorTestHierarchy.cs` for larger structures

### Method invocation model

- `BasicDocumentExampleModel.RecalculateScore()`
- `RoleAwareExampleModel.GeneratePreview()`
- `Test_Class_3.SumTwoDoubles(...)`

### Role-aware field visibility example

- `BaseFramework.WpfHost/Views/RoleAwarePage.xaml.cs`
- `RoleAwareExampleModel`

### PostgreSQL example flow

- `EFCoreDemo/Examples/StudyRunner.cs`

### Document template example

- `src/DocumentAutomation.App/data/templates/system-acceptance-report.docx`
- `docs/TEMPLATE_READER_EXAMPLE.md`

### Document field extraction example

- `src/DocumentAutomation.Word/OpenXml/OpenXmlTemplateScanner.cs`
- `tests/DocumentAutomation.Word.Tests/OpenXmlWorkflowTests.cs`

## Suggested Exercises

After the first read-through, try these small exercises:

1. Add a new inspectable property to `BasicDocumentExampleModel`.
2. Add a new editor hint and register a custom WPF editor.
3. Add a new placeholder to the sample `.docx` template.
4. Extend `OpenXmlTemplateScanner` to infer one more type.
5. Add a new permission and make one field visible only to that permission.
6. Add a new JSONB-mapped property to the product persistence layer.
