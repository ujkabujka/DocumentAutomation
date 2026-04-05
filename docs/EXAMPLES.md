# Examples

This file groups the repository examples from beginner to advanced.

Each section explains:

- what it demonstrates
- where to start reading in code
- how to run or inspect it
- what behavior to expect

## 1. Basic

### What it demonstrates

- scalar text fields
- numeric fields
- dropdown/value source
- multiline editor hint
- note editor
- computed read-only field
- method invocation

### Start reading here

- `BaseFrameWork/BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`
- class: `BasicDocumentExampleModel`

### How to run

```powershell
dotnet run --project BaseFrameWork\BaseFramework.WpfHost\BaseFramework.WpfHost.csproj
```

Open the `Basic` page.

### Expected behavior

- the inspector shows a small editable form
- changing numeric or selection values updates the computed readiness score
- the command button recomputes the score

## 2. Intermediate

### What it demonstrates

- nested inspectable objects
- collection editing
- conditional field visibility
- collection-manipulating commands
- computed summaries over nested state

### Start reading here

- `BaseFrameWork/BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`
- class: `NestedWorkflowExampleModel`

### How to run

Run the WPF host and open the `Intermediate` page.

### Expected behavior

- toggling the boolean field hides or shows approval settings
- collection items are visible and editable
- the computed overview updates as nested state changes

## 3. Advanced

### What it demonstrates

- larger object graphs
- nested collections
- scheduling-oriented state
- richer command-driven changes

### Start reading here

- `BaseFrameWork/BaseFramework.WpfHost/Models/InspectorTestHierarchy.cs`
- class: `Test_Class_3`

### How to run

Run the WPF host and open the `Advanced` page.

### Expected behavior

- the inspector shows a deeper object graph
- schedule items can be added dynamically
- derived fields update when dependent values change

## 4. Document Automation

### What it demonstrates

- runtime-generated metadata
- template-like field definitions without CLR properties
- image/table/file-oriented editor preparation

### Start reading here

- `BaseFrameWork/BaseFramework.WpfHost/Models/TemplateDrivenExampleForm.cs`
- `src/DocumentAutomation.Application/DynamicForms/DynamicDocumentFormObject.cs`

### How to run

1. Run the WPF host and open the `Document Automation` page.
2. Run the product app and open the `Designer` and `Generate` pages.

### Expected behavior

- the framework host shows a runtime-only form
- the product app can rescan a `.docx` and show discovered template fields
- the generation page builds a form from those field definitions

## 5. Security / Roles

### What it demonstrates

- screen-level permission filtering
- field-level visibility
- field-level editability
- action-level authorization

### Start reading here

- `BaseFrameWork/BaseFramework.WpfHost/Views/RoleAwarePage.xaml.cs`
- `BaseFrameWork/BaseFramework.WpfHost/Models/ExampleGalleryModels.cs`
- class: `RoleAwareExampleModel`
- `src/DocumentAutomation.Application/Services/ApplicationAuthorizationService.cs`

### How to run

1. Run the WPF host and open the `Security` page.
2. Switch between the simulated sessions.
3. Run the product app with `Identity:OverrideUserName` set to `engineer`, `designer`, or `admin`.

### Expected behavior

- some fields disappear entirely for the wrong session
- some fields stay visible but become read-only
- some actions can be invoked only by privileged users

## 6. Persistence / DB

### What it demonstrates

- connection string resolution
- EF Core migrations
- CRUD
- relationships
- JSONB
- transaction handling
- the product app's DB-backed services

### Start reading here

- `BaseFrameWorkAndPosgreSQL/EFCoreDemo/Program.cs`
- `BaseFrameWorkAndPosgreSQL/EFCoreDemo/Examples/StudyRunner.cs`
- `BaseFrameWorkAndPosgreSQL/EFCoreDemo/Data/AppDbContext.cs`
- `src/DocumentAutomation.Persistence/DocumentAutomationDbContext.cs`
- `src/DocumentAutomation.Infrastructure/Persistence/DbServices.cs`

### How to run

```powershell
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- help
```

Then:

```powershell
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- basic
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- relationships
dotnet run --project BaseFrameWorkAndPosgreSQL\EFCoreDemo -- json
```

### Expected behavior

- the console app prints each study step clearly
- SQL-backed behaviors become visible in small commands
- the product app uses the same ideas in a layered, non-console architecture

## 7. Template Reader Example

### What it demonstrates

- reading a real `.docx`
- extracting placeholder keys
- inferring field types
- suggesting likely sources such as database, computed, or user input

### Start reading here

- `src/DocumentAutomation.Word/OpenXml/DemoTemplateDocumentWriter.cs`
- `src/DocumentAutomation.Word/OpenXml/OpenXmlTemplateScanner.cs`
- `src/DocumentAutomation.App/Views/DesignerPage.xaml.cs`
- `docs/TEMPLATE_READER_EXAMPLE.md`

### How to run

1. Run the product app.
2. Open the `Designer` page.
3. Select `System Acceptance Report`.
4. Click `Rescan Selected Template`.

### Expected behavior

The page shows a readable extracted structure for fields such as:

- `project_name -> Text`
- `test_date -> Date`
- `image:test_setup -> Image`
- `table:test_results -> Table`

It also shows:

- whether the field is required
- the suggested source order
- the inferred DB key when available

## 8. Generation Workflow Example

### What it demonstrates

- project selection
- template selection
- DB/demo autofill
- missing field completion
- output document creation

### Start reading here

- `src/DocumentAutomation.App/Views/DocumentGenerationPage.xaml.cs`
- `src/DocumentAutomation.Application/Services/DocumentPreparationService.cs`
- `src/DocumentAutomation.Word/OpenXml/OpenXmlDocumentGenerator.cs`

### How to run

1. Run the product app.
2. Open the `Generate` page.
3. Select a project and template.
4. Click `Prepare`.
5. Fill missing fields in the inspector.
6. Click `Generate Document`.

### Expected behavior

- the right side shows prepared field values and their sources
- the left side shows the runtime-built input form
- a generated `.docx` is written to the output folder
- image and table placeholders produce explicit warnings rather than fake success
