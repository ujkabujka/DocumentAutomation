# Role-Based Document Automation Foundation

## Purpose

This repository now has three distinct responsibilities:

1. `BaseFrameWork/`
   Reusable dynamic metadata, inspector, and host samples.
2. `BaseFrameWorkAndPosgreSQL/`
   Educational PostgreSQL and EF Core learning project.
3. Product foundation under `src/` and `tests/`
   The first serious architecture for the role-based document automation application.

The product architecture must evolve the existing `BaseFrameWork` instead of replacing it. The dynamic inspector remains the main UI composition mechanism for editable business objects and runtime document fields.

## Product Layers

- Presentation
  WPF shell, navigation, view models, and role-aware screen composition.
- Application
  Use cases, authorization checks, document preparation workflow, and app-facing service contracts.
- Domain
  Templates, document fields, security model, projects, and generated document records.
- Infrastructure
  PostgreSQL persistence, file storage, current user resolution, and demo-mode fallbacks.
- Word
  Template scanning and minimal document generation based on OpenXML.

## BaseFramework Evolution

- Keep reflection metadata as a working fallback path.
- Introduce richer metadata with stable keys, grouping, descriptions, editor hints, validation hints, persistence hints, and permission requirements.
- Move WPF inspector/navigation/editor controls into a reusable `BaseFramework.Wpf` project.
- Use metadata access delegates so the inspector can work with both reflected objects and runtime-generated document forms.
- Add an editor registry instead of hardcoding all editor selection inside one switch.
- Preserve existing samples and expand them into a guided example gallery.

## Product Foundation Scope

This first implementation pass must include:

- A new root product solution.
- Domain and application contracts for security, templates, document preparation, and generation.
- PostgreSQL persistence with first migrations and sample seed data.
- WPF app shell with Admin, Designer, and Document Generation areas.
- Hybrid startup mode:
  real PostgreSQL when available, seeded demo services when not.
- Minimal real OpenXML support for scanning and replacing simple text placeholders in `.docx` files.

This pass intentionally does not complete full image/table generation. Those field types are modeled, scanned, and surfaced in the workflow, but unsupported generation is reported explicitly.

## Educational Outcome

The repository should remain useful for learning:

- `BaseFrameWork` teaches dynamic inspector concepts from simple scalar fields to role-aware and template-driven forms.
- `BaseFrameWorkAndPosgreSQL/EFCoreDemo` remains a study project for EF Core and PostgreSQL patterns.
- The product projects show how those ideas become a maintainable desktop application architecture.
