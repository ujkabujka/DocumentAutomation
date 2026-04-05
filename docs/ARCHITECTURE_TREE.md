# Architecture Tree

This file gives a project-by-project tree.

The trees are simplified on purpose:

- they hide `bin/`
- they hide `obj/`
- they focus on source design, not build output

## 1. Whole Repository Tree

```text
DocumentAutomation/
|- Readme.md
|- DocumentAutomation.slnx
|- BaseFrameWork/
|  |- README.md
|  |- BaseFramework.sln
|  |- BaseFramework.Core/
|  |- BaseFramework.Generators/
|  |- BaseFramework.Wpf/
|  |- BaseFramework.WpfHost/
|  |- BaseFramework.WebHost/
|  |- BaseFramework.Core.Tests/
|  `- BaseFramework.Wpf.Tests/
|- BaseFrameWorkAndPosgreSQL/
|  |- README.md
|  |- BaseFrameWorkAndPosgreSQL.sln
|  |- EFCoreDemo/
|  `- scripts/
|- docs/
|  |- architecture/
|  |- REPOSITORY_MAP.md
|  |- ARCHITECTURE_TREE.md
|  |- WORKFLOWS.md
|  |- EXAMPLES.md
|  |- DIAGRAMS.md
|  |- CLASS_RESPONSIBILITIES.md
|  |- LEARNING_PATH.md
|  `- TEMPLATE_READER_EXAMPLE.md
|- src/
|  |- DocumentAutomation.App/
|  |- DocumentAutomation.Application/
|  |- DocumentAutomation.Domain/
|  |- DocumentAutomation.Infrastructure/
|  |- DocumentAutomation.Persistence/
|  `- DocumentAutomation.Word/
`- tests/
   |- DocumentAutomation.Application.Tests/
   |- DocumentAutomation.Persistence.Tests/
   `- DocumentAutomation.Word.Tests/
```

## 2. BaseFramework Projects

### `BaseFramework.Core`

```text
BaseFramework.Core/
|- ObservableObject.cs
|- Api/
|  |- ParameterBridgeObject.cs
|  `- ParameterExportMode.cs
|- Access/
|  |- DefaultMemberAccessEvaluator.cs
|  |- IMemberAccessEvaluator.cs
|  |- InspectableAccessContext.cs
|  |- InspectableAccessRules.cs
|  `- MemberAccessEvaluation.cs
|- Attributes/
|  |- GenerateInspectorMetadataAttribute.cs
|  |- InspectableMemberAttribute.cs
|  |- InspectablePresentationAttribute.cs
|  |- InspectableAccessAttribute.cs
|  |- InspectableValidationAttribute.cs
|  |- InspectablePersistenceAttribute.cs
|  `- InspectableEditorAttribute.cs
|- Generated/
|  |- GeneratedMetadataRegistry.cs
|  `- IGeneratedMetadataRegistration.cs
|- Metadata/
|  |- InspectableMemberMetadata.cs
|  |- InspectableTypeMetadata.cs
|  |- InspectableValidationHints.cs
|  |- MemberKind.cs
|  |- MemberKindResolver.cs
|  `- EditorHints.cs
|- Services/
|  |- IObjectMetadataProvider.cs
|  |- IRuntimeInspectableMetadataSource.cs
|  `- ReflectionObjectMetadataProvider.cs
|- Notes/
|- Collections/
|- Scheduling/
`- UndoRedo/
```

What this project does:

- defines the metadata language
- defines observable models
- resolves metadata by runtime source, generator, or reflection
- evaluates field and action access rules
- supports stable-key parameter import/export

Main classes to read first:

- `ObservableObject`
- `ParameterBridgeObject`
- `InspectableMemberAttribute`
- `InspectableMemberMetadata`
- `ReflectionObjectMetadataProvider`
- `DefaultMemberAccessEvaluator`

### `BaseFramework.Generators`

```text
BaseFramework.Generators/
`- InspectorMetadataGenerator.cs
```

What this project does:

- reads `[GenerateInspectorMetadata]`
- finds `[InspectableMember]` properties and methods
- emits metadata registration code
- lets the framework avoid reflection for those types

### `BaseFramework.Wpf`

```text
BaseFramework.Wpf/
|- Controls/
|  |- ObjectInspectorControl.xaml
|  |- ObjectInspectorControl.xaml.cs
|  |- DefaultInspectorEditorRegistry.cs
|  |- InspectorEditorContext.cs
|  |- IInspectorEditorRegistry.cs
|  |- MemberAccess.cs
|  |- StringEditorControl.cs
|  |- IntegerEditorControl.cs
|  |- DoubleEditorControl.cs
|  |- BooleanEditorControl.cs
|  |- EnumEditorControl.cs
|  |- DateTimeEditorControl.cs
|  |- NoteEditorControl.cs
|  |- ClassEditorControl.cs
|  |- CollectionEditorControl.cs
|  |- MethodEditorControl.cs
|  |- PathPickerEditorControl.cs
|  |- TableEditorControl.cs
|  `- Navigation/
|- Themes/
|  `- DarkTheme.xaml
`- GlobalUsings.cs
```

What this project does:

- turns metadata into WPF controls
- selects the right editor for each member kind
- groups members by section and category
- applies permission-aware visibility and editability

Read first:

- `ObjectInspectorControl.xaml.cs`
- `DefaultInspectorEditorRegistry.cs`
- `MemberAccess.cs`

### `BaseFramework.WpfHost`

```text
BaseFramework.WpfHost/
|- App.xaml
|- MainWindow.xaml
|- MainWindow.xaml.cs
|- Models/
|  |- ExampleGalleryModels.cs
|  |- TemplateDrivenExampleForm.cs
|  |- InspectorTestHierarchy.cs
|  |- DemoNode.cs
|  `- VisualTestNode.cs
|- Views/
|  |- InspectorExamplePage.xaml
|  |- InspectorExamplePage.xaml.cs
|  |- RoleAwarePage.xaml
|  |- RoleAwarePage.xaml.cs
|  |- DashboardPage.xaml
|  `- DashboardPage.xaml.cs
|- Themes/
`- Controls/   legacy local controls kept in source but excluded from build
```

What this project does:

- acts as the example gallery
- demonstrates beginner to advanced scenarios
- shows runtime template-driven metadata in a friendly host

Important note:

The host still contains older local `Controls/` files, but its project file excludes them from compilation. The active reusable controls come from `BaseFramework.Wpf`.

Best example files:

- `MainWindow.xaml.cs`
- `Models/ExampleGalleryModels.cs`
- `Models/TemplateDrivenExampleForm.cs`
- `Views/RoleAwarePage.xaml.cs`

### `BaseFramework.WebHost`

```text
BaseFramework.WebHost/
|- Program.cs
`- wwwroot/
   |- index.html
   |- app.js
   `- styles.css
```

What this project does:

- exposes a small web preview
- shows the idea of an inspector-like payload outside WPF
- keeps the framework concept explainable in a cross-platform way

### `BaseFramework.Core.Tests`

```text
BaseFramework.Core.Tests/
|- InspectableMetadataTests.cs
|- ObservableObjectTests.cs
|- CalendarTests.cs
`- NoteDocumentTests.cs
```

What this project proves:

- generated metadata matches reflection metadata
- access rules are respected
- core observable behavior works
- note and scheduling support still behave correctly

### `BaseFramework.Wpf.Tests`

```text
BaseFramework.Wpf.Tests/
`- InspectorEditorRegistryTests.cs
```

What this project proves:

- editor hints override generic member kinds
- the inspector groups by section and then category

## 3. PostgreSQL Learning Project

### `EFCoreDemo`

```text
EFCoreDemo/
|- Program.cs
|- README.md
|- appsettings.example.json
|- Configuration/
|  |- ConnectionStringResolver.cs
|  `- AppDbContextFactory.cs
|- Data/
|  `- AppDbContext.cs
|- Models/
|  |- Student.cs
|  |- Course.cs
|  |- Enrollment.cs
|  |- AuditLog.cs
|  `- supporting value objects and enums
|- Examples/
|  `- StudyRunner.cs
|- Migrations/
`- Scaffolding/
```

What this project does:

- teaches EF Core in a console-first way
- groups examples into explicit commands
- keeps PostgreSQL-specific learning visible

Read first:

- `README.md`
- `Program.cs`
- `Examples/StudyRunner.cs`
- `Data/AppDbContext.cs`

## 4. Product Projects Under `src/`

### `DocumentAutomation.Domain`

```text
DocumentAutomation.Domain/
|- Security/
|  `- SecurityModels.cs
|- Templates/
|  `- TemplateModels.cs
`- Documents/
   `- DocumentModels.cs
```

What this project does:

- defines the business language
- avoids UI, storage, and OpenXML concerns
- gives names to users, roles, fields, templates, requests, and results

### `DocumentAutomation.Application`

```text
DocumentAutomation.Application/
|- Contracts/
|  `- ApplicationContracts.cs
|- Models/
|  `- ApplicationModels.cs
|- DynamicForms/
|  `- DynamicDocumentFormObject.cs
`- Services/
   |- ApplicationAuthorizationService.cs
   `- DocumentPreparationService.cs
```

What this project does:

- defines what the app can ask the outside world to do
- prepares document input data
- creates runtime forms from template field definitions
- applies application-level authorization decisions

### `DocumentAutomation.Infrastructure`

```text
DocumentAutomation.Infrastructure/
|- Configuration/
|  |- DocumentAutomationConnectionResolver.cs
|  `- RuntimeSettings.cs
|- Demo/
|  |- DemoDataStore.cs
|  `- DemoServices.cs
|- DependencyInjection/
|  `- ServiceCollectionExtensions.cs
|- Persistence/
|  `- DbServices.cs
|- Runtime/
|  `- AppRuntimeContext.cs
`- Storage/
   `- FileSystemStorageServices.cs
```

What this project does:

- chooses demo mode or database-backed mode
- wires services into DI
- resolves storage roots and connection status
- provides file system storage
- adapts EF-backed services to application contracts

### `DocumentAutomation.Persistence`

```text
DocumentAutomation.Persistence/
|- DocumentAutomationDbContext.cs
|- Seed/
|  `- DocumentAutomationSeedData.cs
|- Infrastructure/
|  `- JsonValueConverterFactory.cs
|- Configuration/
`- Migrations/
```

What this project does:

- owns EF Core schema mapping
- stores security, template, project, and generated-document records
- uses JSONB for flexible lists and dictionaries
- seeds the first realistic roles, permissions, template, and project

### `DocumentAutomation.Word`

```text
DocumentAutomation.Word/
|- DependencyInjection/
|  `- ServiceCollectionExtensions.cs
`- OpenXml/
   |- DemoTemplateDocumentWriter.cs
   |- OpenXmlTemplateScanner.cs
   `- OpenXmlDocumentGenerator.cs
```

What this project does:

- scans `.docx` files for placeholders
- infers field definitions from placeholder keys
- generates new output files by replacing text placeholders
- seeds a starter `.docx` template

### `DocumentAutomation.App`

```text
DocumentAutomation.App/
|- App.xaml
|- App.xaml.cs
|- MainWindow.xaml
|- MainWindow.xaml.cs
|- appsettings.json
|- data/
|  `- templates/
|     `- system-acceptance-report.docx
`- Views/
   |- AdminPage.xaml
   |- AdminPage.xaml.cs
   |- DesignerPage.xaml
   |- DesignerPage.xaml.cs
   |- DocumentGenerationPage.xaml
   `- DocumentGenerationPage.xaml.cs
```

What this project does:

- starts the WPF app with Generic Host
- shows role-aware navigation
- displays admin, designer, and generation pages
- seeds the demo template if needed

## 5. Product Tests

### `DocumentAutomation.Application.Tests`

```text
DocumentAutomation.Application.Tests/
`- AuthorizationAndPreparationTests.cs
```

Focus:

- permission checks
- hidden field filtering
- default/computed/database value preparation
- generation request composition

### `DocumentAutomation.Persistence.Tests`

```text
DocumentAutomation.Persistence.Tests/
`- PersistenceModelTests.cs
```

Focus:

- table names
- JSONB mapping
- composite keys
- seeded records

### `DocumentAutomation.Word.Tests`

```text
DocumentAutomation.Word.Tests/
`- OpenXmlWorkflowTests.cs
```

Focus:

- placeholder scanning
- text replacement
- warnings for image and table placeholders

## 6. Project Dependency Summary

This section answers the practical question: "who depends on whom?"

### Framework side

- `BaseFramework.Core`
  Depends on:
  very little outside itself
  Used by:
  `BaseFramework.Wpf`, `BaseFramework.WpfHost`, `BaseFramework.WebHost`, product application code
- `BaseFramework.Generators`
  Depends on:
  Roslyn APIs and the framework metadata shape
  Used by:
  annotated model projects such as `BaseFramework.WpfHost`
- `BaseFramework.Wpf`
  Depends on:
  `BaseFramework.Core`
  Used by:
  `BaseFramework.WpfHost` and `DocumentAutomation.App`
- `BaseFramework.WpfHost`
  Depends on:
  `BaseFramework.Core`, `BaseFramework.Wpf`, `BaseFramework.Generators`
  Used by:
  learners and manual example runs
- `BaseFramework.WebHost`
  Depends on:
  basic preview payloads and web hosting
  Used by:
  lightweight preview/demo scenarios

### Learning project side

- `EFCoreDemo`
  Depends on:
  EF Core, Npgsql, PostgreSQL
  Used by:
  learners who want to study persistence concepts in isolation

### Product side

- `DocumentAutomation.Domain`
  Depends on:
  no app/UI/database concerns
  Used by:
  every product layer
- `DocumentAutomation.Application`
  Depends on:
  `DocumentAutomation.Domain`, selected framework concepts for dynamic forms
  Used by:
  `DocumentAutomation.App`, `DocumentAutomation.Infrastructure`, `DocumentAutomation.Word`
- `DocumentAutomation.Persistence`
  Depends on:
  `DocumentAutomation.Domain`, EF Core, Npgsql
  Used by:
  `DocumentAutomation.Infrastructure`
- `DocumentAutomation.Infrastructure`
  Depends on:
  `DocumentAutomation.Application`, `DocumentAutomation.Persistence`
  Used by:
  `DocumentAutomation.App`
- `DocumentAutomation.Word`
  Depends on:
  `DocumentAutomation.Application`, `DocumentAutomation.Domain`, OpenXML
  Used by:
  `DocumentAutomation.App`
- `DocumentAutomation.App`
  Depends on:
  infrastructure services, word services, reusable WPF inspector
  Used by:
  the end user

## 7. Typical Product Workflow Tree

```text
App starts
|- Resolve configuration
|- Resolve database/demo mode
|- Register services
|- Resolve current user
`- Show allowed pages

Designer flow
|- Select template
|- Rescan .docx
|- Extract placeholders
`- Save refreshed field model

Generation flow
|- Select project
|- Select template
|- Prepare field values
|- Show runtime form
|- Save persistable values
`- Generate output document
```
