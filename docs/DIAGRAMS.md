# Diagrams

This file collects Mermaid diagrams for the main architectures and workflows.

You can copy these blocks into GitHub Markdown preview or other Mermaid viewers.

## 1. Repository Architecture Tree

```mermaid
flowchart TB
    Repo["DocumentAutomation Repository"]
    Repo --> BF["BaseFrameWork"]
    Repo --> PG["BaseFrameWorkAndPosgreSQL"]
    Repo --> Docs["docs"]
    Repo --> Product["src + tests"]

    BF --> Core["BaseFramework.Core"]
    BF --> Gen["BaseFramework.Generators"]
    BF --> Wpf["BaseFramework.Wpf"]
    BF --> WpfHost["BaseFramework.WpfHost"]
    BF --> WebHost["BaseFramework.WebHost"]
    BF --> BfTests["Core + WPF Tests"]

    PG --> EF["EFCoreDemo"]
    PG --> Scripts["scripts"]

    Product --> App["DocumentAutomation.App"]
    Product --> Application["DocumentAutomation.Application"]
    Product --> Domain["DocumentAutomation.Domain"]
    Product --> Infra["DocumentAutomation.Infrastructure"]
    Product --> Persistence["DocumentAutomation.Persistence"]
    Product --> Word["DocumentAutomation.Word"]
    Product --> ProductTests["Application / Persistence / Word Tests"]
```

## 2. BaseFramework Metadata Flow

```mermaid
flowchart LR
    Target["Target Object"] --> Provider["ReflectionObjectMetadataProvider"]
    Provider --> Runtime{"Implements IRuntimeInspectableMetadataSource?"}
    Runtime -- Yes --> RuntimeMeta["Runtime Metadata"]
    Runtime -- No --> Generated{"Generated Metadata Registered?"}
    Generated -- Yes --> GeneratedMeta["GeneratedMetadataRegistry"]
    Generated -- No --> Reflection["Reflection Scan"]
    RuntimeMeta --> Members["InspectableTypeMetadata"]
    GeneratedMeta --> Members
    Reflection --> Members
```

## 3. Dynamic UI Generation Flow

```mermaid
sequenceDiagram
    participant Page as WPF Page
    participant Inspector as ObjectInspectorControl
    participant Provider as Metadata Provider
    participant Evaluator as Access Evaluator
    participant Registry as Editor Registry
    participant Editor as Editor Control
    participant Model as ObservableObject

    Page->>Inspector: Bind(model, provider, evaluator, accessContext, registry)
    Inspector->>Provider: GetMetadata(model)
    Provider-->>Inspector: InspectableTypeMetadata
    Inspector->>Evaluator: Evaluate each member
    Evaluator-->>Inspector: view/edit/invoke decisions
    Inspector->>Registry: CreateEditor(member)
    Registry-->>Inspector: WPF editor
    Inspector->>Editor: place editor in layout
    Editor->>Model: read/write through getter/setter delegates
    Model-->>Inspector: LayoutInvalidated
    Inspector->>Inspector: Rebuild
```

## 4. BaseFramework Core Class Diagram

```mermaid
classDiagram
    class ObservableObject {
      +GetRaw(key)
      +ApplyExternalValue(key, value)
      +AddRejection(key)
      +RemoveRejection(key)
      +LayoutInvalidated
    }

    class ParameterBridgeObject {
      +GetParameters()
      +GetParametersByClrName()
      +SetParameters(values)
    }

    class InspectableMemberAttribute {
      +Key
      +DisplayName
      +ReadOnly
      +Order
    }

    class InspectableMemberMetadata {
      +Key
      +ClrName
      +DisplayName
      +Kind
      +Getter
      +Setter
      +Invoker
      +AccessRules
    }

    class ReflectionObjectMetadataProvider {
      +GetMetadata(object)
      +GetMetadata(Type)
    }

    class DefaultMemberAccessEvaluator {
      +Evaluate(member, target, context)
    }

    class ObjectInspectorControl {
      +Bind(...)
      +Clear()
    }

    class DefaultInspectorEditorRegistry {
      +CreateEditor(context)
      +Register(kind, factory)
      +Register(hint, factory)
    }

    ObservableObject <|-- ParameterBridgeObject
    ReflectionObjectMetadataProvider --> InspectableMemberMetadata
    ObjectInspectorControl --> ReflectionObjectMetadataProvider
    ObjectInspectorControl --> DefaultMemberAccessEvaluator
    ObjectInspectorControl --> DefaultInspectorEditorRegistry
```

## 5. Role / Permission Flow

```mermaid
flowchart TD
    Session["CurrentUserSession"] --> Roles["Roles"]
    Session --> Permissions["Permissions"]

    Field["TemplateFieldDefinition or InspectableMemberMetadata"] --> ViewRules["VisibleRoles / VisiblePermissions"]
    Field --> EditRules["EditableRoles / EditablePermissions"]
    Field --> InvokeRules["InvokeRoles / InvokePermissions"]

    Roles --> Decision["Authorization Decision"]
    Permissions --> Decision
    ViewRules --> Decision
    EditRules --> Decision
    InvokeRules --> Decision

    Decision --> Hidden["Hide member"]
    Decision --> ReadOnly["Show read-only member"]
    Decision --> Editable["Show editable member"]
    Decision --> Invokable["Enable command"]
```

## 6. Document Generation Workflow

```mermaid
sequenceDiagram
    participant User
    participant Page as DocumentGenerationPage
    participant Prep as DocumentPreparationService
    participant Project as IProjectDataService
    participant Template as ITemplateCatalogService
    participant Form as DynamicDocumentFormObject
    participant Generator as IDocumentGenerator

    User->>Page: Select project + template
    Page->>Prep: PrepareAsync(projectId, templateId)
    Prep->>Project: GetProjectAsync + GetAutofillValuesAsync
    Prep->>Template: GetTemplateAsync
    Prep->>Prep: Resolve database/computed/default values
    Prep->>Form: Build runtime form metadata
    Form-->>Page: Dynamic form + initial field values
    User->>Page: Fill missing fields
    Page->>Prep: CreateGenerationRequestAsync(preparation)
    Prep-->>Page: DocumentGenerationRequest
    Page->>Generator: GenerateAsync(request)
    Generator-->>Page: Output path + warnings + final values
```

## 7. Template Reader Workflow

```mermaid
flowchart TD
    Docx[".docx Template"] --> Scanner["OpenXmlTemplateScanner"]
    Scanner --> TextNodes["Read Wordprocessing Text Nodes"]
    TextNodes --> Regex["Find $placeholder$ keys"]
    Regex --> Unique["Remove duplicates"]
    Unique --> Infer["Infer field type / section / category"]
    Infer --> Fields["TemplateFieldDefinition[]"]
    Fields --> Designer["Designer page can review field model"]
    Fields --> RuntimeForm["DynamicDocumentFormObject can render runtime form"]
```

## 8. Database -> App -> Document Pipeline

```mermaid
flowchart LR
    DB["PostgreSQL / Demo Data"] --> ProjectService["IProjectDataService"]
    TemplateStore["Template Storage"] --> TemplateService["ITemplateCatalogService"]
    TemplateFile[".docx File"] --> Scanner["ITemplateScanner"]

    ProjectService --> Prep["DocumentPreparationService"]
    TemplateService --> Prep
    Scanner --> Designer["Designer Workflow"]
    Prep --> Form["DynamicDocumentFormObject"]
    Form --> Inspector["BaseFramework WPF Inspector"]
    Inspector --> Request["DocumentGenerationRequest"]
    Request --> Generator["OpenXmlDocumentGenerator"]
    Generator --> Output["Generated .docx Output"]
```

## 9. Learning Flow Through The Repository

```mermaid
flowchart TD
    Start["Start"] --> Map["Read Repository Map"]
    Map --> Tree["Read Architecture Tree"]
    Tree --> Workflows["Read Workflows"]
    Workflows --> FrameworkRun["Run BaseFramework.WpfHost"]
    FrameworkRun --> FrameworkTests["Read BaseFramework Tests"]
    FrameworkTests --> EfRead["Read EFCoreDemo README"]
    EfRead --> EfRun["Run EFCoreDemo commands"]
    EfRun --> ProductRead["Read src/ product projects"]
    ProductRead --> ProductRun["Run DocumentAutomation.App"]
    ProductRun --> TemplateExample["Read Template Reader Example"]
```
