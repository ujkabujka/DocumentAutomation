# Workflows

This file explains how the main parts of the repository work step by step.

The goal is plain understanding, not just naming classes.

## 1. How BaseFramework Generates UI From A Model

### Short version

A model exposes inspectable metadata, the framework reads that metadata, the WPF inspector chooses controls, and the controls read and write values through delegates.

### Step by step

1. A model class is created.

   Example:

   - `BasicDocumentExampleModel`
   - `NestedWorkflowExampleModel`
   - `RoleAwareExampleModel`

2. The model marks members with `[InspectableMember]`.

   These members become part of the editable surface.

3. Optional companion attributes add extra meaning.

   Examples:

   - display grouping
   - help text
   - permissions
   - validation hints
   - persistence keys
   - editor hints

4. `ReflectionObjectMetadataProvider` is asked for metadata.

5. The provider chooses one metadata source:

   - runtime metadata from `IRuntimeInspectableMetadataSource`
   - generated metadata from `GeneratedMetadataRegistry`
   - reflection fallback

6. `ObjectInspectorControl` binds to the target model and metadata provider.

7. The inspector evaluates access for each member through `IMemberAccessEvaluator`.

8. The inspector removes members that:

   - are rejected by `ObservableObject`
   - fail visibility rules

9. The inspector sorts the remaining members by:

   - section
   - category
   - order
   - display name

10. The inspector asks `IInspectorEditorRegistry` for an editor control.

11. The editor reads and writes values through:

   - `Getter`
   - `Setter`
   - `Invoker`
   - `ValueSourceAccessor`

12. When the model invalidates layout, the inspector rebuilds itself.

### Small example

```csharp
[InspectableMember("example.basic.project_code", "Project Code", Order = 1)]
public string ProjectCode
{
    get => Get<string>() ?? string.Empty;
    set => Set(value);
}
```

This one property is enough for the inspector to know:

- the stable key
- the label
- the order
- the value type

## 2. How Metadata Is Discovered

The framework uses a clear priority order.

### Step by step

1. `GetMetadata(target)` is called.
2. If the object implements `IRuntimeInspectableMetadataSource`, runtime metadata is used immediately.
3. Otherwise, the provider checks whether the source generator has registered metadata for the type.
4. If generated metadata exists and contains members, it is used.
5. If not, reflection scans public properties and methods for `[InspectableMember]`.
6. Reflection metadata is cached per type.

### Why this matters

This design lets the same inspector support three very different scenarios:

- ordinary reflection-based models
- compile-time generated metadata
- dynamic forms built from a template or database

## 3. How Editor Controls Are Selected

`DefaultInspectorEditorRegistry` maps either:

- an explicit editor hint
- or a member kind

### Step by step

1. A member arrives at the registry.
2. If `EditorHint` is set, the registry tries the hint first.
3. If no hint match exists, the registry falls back to `MemberKind`.
4. The registry returns a WPF control or `null`.
5. The inspector adds that control to the layout.

### Examples

- `EditorHints.Image` -> `PathPickerEditorControl`
- `MemberKind.DateTime` -> `DateTimeEditorControl`
- `MemberKind.Method` -> `MethodEditorControl`
- `MemberKind.Collection` -> `CollectionEditorControl`

### Important test

`BaseFramework.Wpf.Tests/InspectorEditorRegistryTests.cs` proves that:

- editor hints win over generic kinds
- grouping is section-first, category-second

## 4. How Stable Keys Work

Stable keys are the business-facing names of fields.

Examples:

- `project.code`
- `approval.note`
- `image:test_setup`

### Why stable keys exist

CLR property names can change during refactoring.

Stable keys are safer for:

- database mapping
- template placeholders
- saved document field values
- external automation

### Step by step

1. A property is marked with `[InspectableMember("project.code", "Project Code")]`.
2. `ParameterBridgeObject.GetParameters()` exports values by stable key.
3. `SetParameters()` accepts both:

   - stable key
   - CLR property name

4. New integrations should use stable keys only.

## 5. How Role And Permission Rules Work

There are two similar but separate places where permissions matter.

### In BaseFramework

The framework-level metadata can say:

- who may see a field
- who may edit a field
- who may invoke an action

This is evaluated by `DefaultMemberAccessEvaluator`.

### In the product app

The product-level template field definitions can also carry:

- `VisibleRoles`
- `VisiblePermissions`
- `EditableRoles`
- `EditablePermissions`

This is checked by `ApplicationAuthorizationService`.

### Step by step in the product

1. The current user is resolved.
2. The current user gets a role list and permission list.
3. A template field is tested against those rules.
4. Hidden fields are not added to the runtime form.
5. Visible but restricted fields are shown but may be read-only.

## 6. How The PostgreSQL Demo Flow Works

The EF Core learning project is intentionally simple.

### Step by step

1. `Program.cs` reads the command line.
2. `ConnectionStringResolver` chooses a connection string.
3. `StudyRunner` receives the connection string.
4. The chosen command runs one study scenario:

   - help
   - info
   - canconnect
   - migrate
   - seed
   - basic
   - queries
   - relationships
   - json
   - rawsql
   - transaction
   - all

5. `StudyRunner` builds `AppDbContext`.
6. `AppDbContext` maps entities to PostgreSQL tables.
7. The scenario writes logs to the console so the learner can see what happened.

### Best beginner path

1. `help`
2. `info`
3. `canconnect`
4. `migrate`
5. `seed`
6. `basic`

## 7. How The Document Automation App Starts

### Step by step

1. `App.xaml.cs` builds a Generic Host.
2. Configuration is loaded from:

   - `appsettings.json`
   - `appsettings.local.json`
   - environment variables

3. `AddDocumentAutomationInfrastructure(...)` runs.
4. The infrastructure resolves:

   - database connection availability
   - template root path
   - output root path

5. If PostgreSQL is available:

   - EF-backed services are registered

6. If PostgreSQL is not available:

   - demo in-memory services are registered

7. Word services are added:

   - scanner
   - field extractor
   - generator

8. `MainWindow` is created.
9. On load, the app ensures:

   - template folder exists
   - seed `.docx` exists
   - current user session is resolved
   - pages are added based on permissions

## 8. How The Current Document Preparation Workflow Works

This is the most important product workflow today.

### Step by step

1. The user selects:

   - a project
   - a template

2. `DocumentGenerationPage` calls `IDocumentPreparationService.PrepareAsync(...)`.

3. `DocumentPreparationService` loads:

   - current user
   - project
   - template

4. It asks `IProjectDataService` for autofill values.

5. For each template field, it tries to resolve a value in priority order.

   Typical priority:

   - database
   - computed
   - default

6. It creates a `DocumentFieldValue` for each field.

7. It filters out fields the user is not allowed to see.

8. It creates `DynamicDocumentFormObject`.

9. `DynamicDocumentFormObject` turns template definitions into runtime metadata.

10. The page binds that form to `ObjectInspectorControl`.

11. The user edits missing or editable values.

12. The page can save persistable values back to the project store.

13. The page can request final generation.

## 9. How Template Reading Works

### Current implementation

`OpenXmlTemplateScanner` reads `.docx` text nodes and looks for placeholders that match:

```text
$some_key$
```

The regex is:

```text
\$(?<field>[A-Za-z0-9_:\.-]+)\$
```

### Step by step

1. A `.docx` file path is given to the scanner.
2. The scanner opens the Word document with OpenXML.
3. It reads text nodes from the main document body.
4. It finds placeholders inside those text nodes.
5. It removes duplicates.
6. It infers field types from prefixes or names.

Type inference examples:

- `image:test_setup` -> `Image`
- `table:test_results` -> `Table`
- `test_date` -> `Date`
- everything else -> `Text`

7. It returns `TemplateFieldDefinition` objects.

### Important limitation

This first pass looks at whole text nodes.

That is enough for the demo template, but real Word documents can split visible text across multiple runs. A future scanner will need to handle that more robustly.

## 10. How The App Knows Which Keys Are Required

There are two ways a field becomes required.

### Seeded template definition

The database or demo seed already defines:

- field key
- display name
- type
- required flag
- section
- category
- persistence hints

### Scanned template

The scanner discovers placeholder keys from the `.docx` file and creates field definitions.

### Combined mental model

The template says:

- which keys exist
- how they should be grouped
- what type they look like

The application says:

- which of those keys can be filled from the database
- which need user input
- which are restricted by permission

## 11. How Database Autofill, User Input, And Generation Fit Together

### Step by step

1. The template defines the field key:

   example: `project_name`

2. The template field definition may also define:

   - `DatabaseKey`
   - `PersistenceKey`

3. `DocumentPreparationService` checks the project autofill dictionary using those keys.

4. If a value is found, the field source becomes `Database`.

5. If no database value is found, computed rules are tried.

   Examples:

   - `system.current_user`
   - `system.generated_on`

6. If still missing, a default value is tried.

7. If still missing and the field is required, it is marked missing.

8. The user fills the missing data in the dynamic form inspector.

9. `CreateGenerationRequestAsync(...)` snapshots the form values.

10. `OpenXmlDocumentGenerator` copies the template to an output path and replaces text placeholders.

11. Unsupported field kinds such as image and table are reported as warnings.

## 12. Future Document Editor / Template Reader Workflow

The current repo already contains the foundation for the future document editor flow.

The likely future workflow is:

1. A designer drops a new Word template into the template storage folder.
2. The app scans it.
3. The app discovers placeholders.
4. The app shows a designer screen to review field definitions.
5. The designer adds business metadata:

   - labels
   - sections
   - persistence rules
   - permissions
   - save-back rules

6. The system engineer chooses a project and template.
7. The app fills what it can from PostgreSQL.
8. The app asks the user for what is still missing.
9. The app generates a final document.

For the concrete example, see:

- `docs/TEMPLATE_READER_EXAMPLE.md`
