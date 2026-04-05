# Class Responsibilities

This file explains the most important classes in the repository.

For each class, the structure is:

- what it is for
- what problem it solves
- who uses it
- key members
- dependencies

## BaseFramework Core

### `ObservableObject`

What it is for:

- the base state container for inspectable models

What problem it solves:

- gives the framework one shared way to store values
- supports change propagation
- supports layout invalidation
- supports dependency tracking and rejection-based visibility

Who uses it:

- almost every sample model
- runtime forms
- nested child objects
- the WPF inspector

Key members:

- `Get<T>()`
- `Set(...)`
- `GetRaw(...)`
- `ApplyExternalValue(...)`
- `AddDependency(...)`
- `AddRejection(...)`
- `RemoveRejection(...)`
- `LayoutInvalidated`

Depends on:

- its own internal state dictionaries
- event-based invalidation

### `ParameterBridgeObject`

What it is for:

- a framework model that can export and import inspectable values

What problem it solves:

- turns inspectable members into a key-value payload
- uses stable business keys instead of relying on property names

Who uses it:

- most example models in `BaseFramework.WpfHost`
- parity tests in `BaseFramework.Core.Tests`

Key members:

- `GetParameters(...)`
- `GetParametersByClrName()`
- `SetParameters(...)`

Depends on:

- `ObservableObject`
- reflection on `[InspectableMember]`

### `InspectableMemberAttribute`

What it is for:

- marks a property or method as part of the generated inspector surface

What problem it solves:

- tells the framework which members matter
- defines the stable business key and display label

Who uses it:

- reflected models
- source-generated models

Key properties:

- `Key`
- `DisplayName`
- `ReadOnly`
- `Order`
- `ValueSourcePropertyName`

Depends on:

- nothing runtime-heavy by itself

### `InspectableMemberMetadata`

What it is for:

- the runtime description of one inspectable member

What problem it solves:

- gives the framework one uniform metadata shape for:
  - reflected members
  - generated members
  - runtime-built members

Who uses it:

- metadata provider
- access evaluator
- WPF inspector
- editor registry

Key members:

- `Key`
- `ClrName`
- `DisplayName`
- `Kind`
- `EditorHint`
- `PersistenceKey`
- `DatabaseKey`
- `AccessRules`
- `ValidationHints`
- `Getter`
- `Setter`
- `Invoker`
- `ValueSourceAccessor`

Depends on:

- `MemberKind`
- access and validation metadata
- optional reflection members

### `ReflectionObjectMetadataProvider`

What it is for:

- the main metadata resolver

What problem it solves:

- chooses whether metadata comes from:
  - runtime source
  - generated source
  - reflection

Who uses it:

- WPF host
- product app
- tests

Key methods:

- `GetMetadata(object target)`
- `GetMetadata(Type targetType)`

Depends on:

- `GeneratedMetadataRegistry`
- reflection
- `IRuntimeInspectableMetadataSource`

### `DefaultMemberAccessEvaluator`

What it is for:

- the default visibility, editability, and invocation rules engine

What problem it solves:

- turns raw role/permission requirements into yes/no UI decisions

Who uses it:

- `ObjectInspectorControl`
- framework tests
- product dynamic form inspector

Key method:

- `Evaluate(...)`

Depends on:

- `InspectableAccessContext`
- `InspectableAccessRules`

## BaseFramework WPF

### `ObjectInspectorControl`

What it is for:

- the reusable dynamic inspector surface

What problem it solves:

- renders a model without a handwritten form
- groups members into a readable layout
- respects access rules and rejections

Who uses it:

- `BaseFramework.WpfHost`
- `DocumentAutomation.App`

Key methods:

- `Bind(...)`
- `Clear()`
- internal `Rebuild()`

Depends on:

- `IObjectMetadataProvider`
- `IMemberAccessEvaluator`
- `IInspectorEditorRegistry`
- `ObservableObject`

### `DefaultInspectorEditorRegistry`

What it is for:

- the default editor lookup table

What problem it solves:

- keeps editor selection out of one giant switch in the inspector
- supports hint-based overrides

Who uses it:

- `ObjectInspectorControl`
- WPF host pages
- product document generation page

Key methods:

- `CreateEditor(...)`
- `Register(MemberKind, ...)`
- `Register(string editorHint, ...)`

Depends on:

- editor control classes
- `InspectorEditorContext`

### `MemberAccess`

What it is for:

- small helper layer for reading, writing, invoking, and matching metadata-driven members

What problem it solves:

- keeps editor controls from duplicating metadata access rules

Who uses it:

- many WPF editor controls
- `DefaultInspectorEditorRegistry`

Key methods:

- `CanWrite(...)`
- `CanInvoke(...)`
- `GetValue(...)`
- `SetValue(...)`
- `Invoke(...)`
- `GetValueSource(...)`

Depends on:

- `InspectableMemberMetadata`

## BaseFramework Example Host

### `BasicDocumentExampleModel`

What it is for:

- the beginner example

What problem it solves:

- shows the smallest useful end-to-end inspectable model

Who uses it:

- `BaseFramework.WpfHost`
- learners

What it demonstrates:

- scalar fields
- dropdown value source
- multiline text
- note editor
- computed field
- method invocation

### `NestedWorkflowExampleModel`

What it is for:

- the nested object and collection example

What problem it solves:

- shows that the inspector can recurse into object graphs and collections

Who uses it:

- `BaseFramework.WpfHost`

What it demonstrates:

- rejection-based conditional visibility
- nested object editing
- collection editing
- collection mutation through commands
- computed summary

### `RoleAwareExampleModel`

What it is for:

- the permission example

What problem it solves:

- makes field-level and action-level authorization visible in the UI

Who uses it:

- `RoleAwarePage`

What it demonstrates:

- visible-to-all field
- role-visible field
- permission-editable field
- permission-invokable action

### `TemplateDrivenExampleForm`

What it is for:

- the runtime metadata example

What problem it solves:

- shows that template fields do not need CLR properties to become editable UI

Who uses it:

- `BaseFramework.WpfHost`
- learners moving toward document automation

Key method:

- `GetRuntimeMetadata()`

Depends on:

- `IRuntimeInspectableMetadataSource`
- runtime field specifications

## EF Core Learning Project

### `ConnectionStringResolver`

What it is for:

- finding the connection string for the EF demo

What problem it solves:

- keeps connection discovery simple and readable for learners

Who uses it:

- `Program`
- `StudyRunner`
- design-time tooling paths

Key methods:

- `Resolve()`
- `Describe(...)`

Depends on:

- environment variables
- JSON config files

### `AppDbContext`

What it is for:

- the EF Core model for the learning project

What problem it solves:

- turns entity classes into PostgreSQL tables and JSON mappings

Who uses it:

- `StudyRunner`
- EF CLI tools

Key parts:

- `DbSet<Student>`
- `DbSet<Course>`
- `DbSet<Enrollment>`
- `DbSet<AuditLog>`
- `OnModelCreating(...)`

Depends on:

- Npgsql EF Core provider

### `StudyRunner`

What it is for:

- the command dispatcher and learning script for the EF demo

What problem it solves:

- gives each EF topic a runnable entry point

Who uses it:

- `Program.cs`
- the learner via console commands

Key methods:

- `PrintHelp()`
- `ApplyMigrations()`
- `SeedSampleData()`
- `RunBasicCrud()`
- `RunQueryLearningExamples()`
- `RunRelationshipExamples()`
- `RunJsonExamples()`
- `RunRawSqlExamples()`
- `RunTransactionExample()`

Depends on:

- `AppDbContext`
- models
- PostgreSQL connection

## Product Foundation

### `MainWindow`

What it is for:

- the shell of the desktop app

What problem it solves:

- shows user context
- shows runtime status
- builds role-aware navigation

Who uses it:

- the main WPF app

Key responsibilities:

- ensure the template file exists
- resolve current user
- register pages based on permissions

Depends on:

- `ICurrentUserContext`
- `IAuthorizationService`
- `ITemplateStorage`
- the three page classes

### `ApplicationAuthorizationService`

What it is for:

- product-level authorization checks

What problem it solves:

- decides whether a user may see or edit a template field

Who uses it:

- `DocumentPreparationService`
- the shell for page visibility
- tests

Key methods:

- `HasPermission(...)`
- `CanView(...)`
- `CanEdit(...)`

Depends on:

- `CurrentUserSession`
- `TemplateFieldDefinition`

### `DocumentPreparationService`

What it is for:

- the application service that prepares a template for editing and generation

What problem it solves:

- combines user, project, template, autofill data, defaults, computed values, and permissions into one editable runtime form

Who uses it:

- `DocumentGenerationPage`
- application tests

Key methods:

- `PrepareAsync(...)`
- `CreateGenerationRequestAsync(...)`

Depends on:

- `ICurrentUserContext`
- `IAuthorizationService`
- `IProjectDataService`
- `ITemplateCatalogService`
- `ITemplateStorage`
- `IOutputStorage`

### `DynamicDocumentFormObject`

What it is for:

- a runtime-built inspectable form for template fields

What problem it solves:

- lets the BaseFramework inspector edit template-driven data that does not exist as CLR properties

Who uses it:

- `DocumentPreparationService`
- `DocumentGenerationPage`

Key methods:

- constructor that loads definitions and initial values
- `GetRuntimeMetadata()`
- `SnapshotValues()`

Depends on:

- `ObservableObject`
- `IRuntimeInspectableMetadataSource`
- `TemplateFieldDefinition`
- `DocumentFieldValue`

### `DocumentAutomationConnectionResolver`

What it is for:

- runtime choice of database mode and storage roots

What problem it solves:

- keeps startup readable and centralizes environment/config inspection

Who uses it:

- infrastructure DI setup

Key methods:

- `Resolve(...)`
- `ResolveStorage(...)`

Depends on:

- configuration
- Npgsql connection test

### `DemoDataStore`

What it is for:

- in-memory seed-backed data when PostgreSQL is unavailable

What problem it solves:

- keeps the main app explorable even without database setup

Who uses it:

- demo services

Key contents:

- users
- roles
- permissions
- templates
- template fields
- projects

Depends on:

- `DocumentAutomationSeedData`

### `DocumentAutomationDbContext`

What it is for:

- the product EF Core schema

What problem it solves:

- maps users, roles, permissions, templates, projects, and generated documents into PostgreSQL

Who uses it:

- database-backed infrastructure services
- persistence tests

Key responsibilities:

- configure table names
- configure JSONB columns
- configure composite keys
- seed initial records

Depends on:

- Npgsql EF Core provider
- `JsonValueConverterFactory`
- `DocumentAutomationSeedData`

### `OpenXmlTemplateScanner`

What it is for:

- reads `.docx` files and extracts placeholder-based field definitions

What problem it solves:

- turns a raw template file into a structured field list

Who uses it:

- designer page
- word tests

Key methods:

- `ScanAsync(...)`
- `ExtractFieldsAsync(...)`

Depends on:

- OpenXML SDK
- regex placeholder matching

### `OpenXmlDocumentGenerator`

What it is for:

- creates a generated document from a request

What problem it solves:

- copies a template and replaces text placeholders with final values

Who uses it:

- `DocumentGenerationPage`
- word tests

Key method:

- `GenerateAsync(...)`

Depends on:

- OpenXML SDK
- `DocumentGenerationRequest`

### `DemoTemplateDocumentWriter`

What it is for:

- creates the seed template file used by the demo app and tests

What problem it solves:

- makes the repository runnable without needing a human-made starter Word file first

Who uses it:

- `MainWindow`
- word tests

Key method:

- `EnsureSeedTemplateAsync(...)`

Depends on:

- OpenXML SDK

## Best Classes To Read In Order

If you want a compact understanding, read these classes in this order:

1. `BaseFramework.Core/Attributes/InspectableMemberAttribute.cs`
2. `BaseFramework.Core/Metadata/InspectableMemberMetadata.cs`
3. `BaseFramework.Core/Services/ReflectionObjectMetadataProvider.cs`
4. `BaseFramework.Wpf/Controls/ObjectInspectorControl.xaml.cs`
5. `BaseFramework.Wpf/Controls/DefaultInspectorEditorRegistry.cs`
6. `BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`
7. `src/DocumentAutomation.Application/Models/DynamicDocumentFormObject.cs`
8. `src/DocumentAutomation.Application/Services/DocumentPreparationService.cs`
9. `src/DocumentAutomation.Word/OpenXml/OpenXmlTemplateScanner.cs`
10. `src/DocumentAutomation.Word/OpenXml/OpenXmlDocumentGenerator.cs`
