# DocumentAutomation.Word

This project is the document-template learning slice of the repository.

It does four small jobs:

1. read template text
2. find placeholders such as `$project_name$`
3. turn those placeholders into structured field definitions
4. render a new `.docx` by replacing simple text placeholders

## Recommended Reading Order

1. `Examples/Templates/example-01-simple-text.txt`
2. `Examples/Templates/example-02-mixed-placeholders.txt`
3. `Parsing/PlaceholderParser.cs`
4. `Parsing/TemplateFieldDefinitionFactory.cs`
5. `Parsing/TextTemplateScanner.cs`
6. `OpenXml/OpenXmlTemplateTextReader.cs`
7. `OpenXml/OpenXmlTemplateScanner.cs`
8. `OpenXml/OpenXmlDocumentGenerator.cs`

## Folder Guide

- `Abstractions`
  - small interfaces that separate file reading from placeholder interpretation
- `Models`
  - small internal models used while scanning
- `Parsing`
  - the simplest part of the subsystem
- `OpenXml`
  - `.docx` specific adapters
- `Examples`
  - small templates and readable demo helpers
- `DependencyInjection`
  - service registration

## First Example

Input:

```text
Project Name: $project_name$
Project Code: $project_code$
```

Output:

- `project_name -> Text`
- `project_code -> Text`

## Mixed Example

Input:

```text
Project Name: $project_name$
Test Date: $test_date$
Test Setup Image: $image:test_setup$
Test Results Table: $table:test_results$
```

Output:

- `project_name -> Text`
- `test_date -> Date`
- `image:test_setup -> Image`
- `table:test_results -> Table`

## Where To Look For Printed Output

`Examples/TemplateInspectionReporter.cs` formats a readable report that lists:

- key
- field type
- suggested label
- likely source
- required flag
