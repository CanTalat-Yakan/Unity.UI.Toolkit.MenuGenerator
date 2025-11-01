# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# UI Menu Generator

> Quick overview: Visual editor for building data‑driven UI Toolkit menus. Create a menu GameObject, open the Menu Builder, arrange items in a TreeView, and click Apply to generate a clean ScriptableObject asset tree that your runtime generator can consume.

A focused editor workflow for authoring UI menus as ScriptableObjects. You build a hierarchy with a left‑pane tree and edit each item’s properties on the right; on Apply, the system writes a stable folder + asset structure, updates references, and prunes any unused files.

![screenshot](Documentation/Screenshot.png)

## Features
- One‑click setup
  - GameObject → UI Toolkit → Add Menu creates a ready‑to‑edit menu with required components
- Menu Builder window
  - Left: hierarchical TreeView with rename, breadcrumbs, context menu, and Add Item button
  - Right: dynamic inspector for the selected ScriptableObject (arrays/references drawn smartly)
  - Toolbar: Revert (reload from data), Apply (save to disk and update runtime data)
- Deterministic asset writing
  - Saves a root asset `UIMenuData_<Name>.asset` and a child directory with ordered, uniquely named assets per node
  - Automatically moves/renames folders when you rename the root, and deletes files no longer present in the tree
- Clean integration
  - Data object model (`MenuData`, categories/items) is populated from the tree on Apply
  - Runtime components (Menu/MenuGenerator/UIMenuProfileProvider) sit on the same GameObject for use in‑game

## Requirements
- Unity Editor 6000.0+ (Editor‑only workflow; generates assets for runtime)
- UI Toolkit (UIDocument/UXML to render your menus at runtime)
- UnityEssentials editor support libraries (used by the builder UI):
  - EditorWindowDrawer, SimpleTreeView, CustomScriptableObjectDrawer, EditorIconUtilities

## Usage

### 1) Create a Menu object
- Menu: GameObject → UI Toolkit → Add Menu
  - Creates a GameObject named “UI Menu” with components:
    - `Menu` (menu data holder; calls `Initialize()`)
    - `UIMenuProfileProvider` (profiles/settings)
    - `MenuGenerator` (consumes data to render at runtime)

### 2) Open the Menu Builder
- Select the “UI Menu” object; use the button provided by the Menu component in the Inspector to open the builder
  - Internally the builder is opened through a `Menu.ShowEditor` hook

### 3) Build your hierarchy
- Use the left TreeView to add/rename/reorder items and categories (context menu or Add Item button)
- The right pane exposes the selected node’s ScriptableObject fields; edit values directly
- Breadcrumbs help navigate back to ancestors

### 4) Save (“Apply”) and wire runtime
- Click Apply to open a Save dialog
  - Root saved as `UIMenuData_<Name>.asset`
  - A sibling folder `UIMenuData_<Name>/` holds child assets organized by depth/index with stable names
  - References are set:
    - `MenuData.Root` is repopulated with top‑level assets
    - Category assets’ `Data[]` arrays are filled with ordered child references (recursively)
  - Unused files/directories in that folder are deleted automatically
- The Menu component’s data is updated and ready for the `MenuGenerator` to consume at runtime

### 5) Revert
- Click Revert to reload the TreeView from the current data without writing to disk

## How It Works
- Editor window (`MenuEditor`)
  - Composes the builder UI with EditorWindowDrawer
  - Builds a SimpleTreeView from `MenuData` and provides rename/context/breadcrumbs
  - On Apply: `MenuEditorAssetSerializer.Save` persists layout and repopulates the data model
- Asset serialization (`MenuEditorAssetSerializer`)
  - Save dialog selects/creates the root `.asset` and a child directory
  - Writes each node to disk using an ordered unique filename (index + name + ID)
  - Sets root/category references, refreshes AssetDatabase, and cleans unused assets/folders
- Prefab spawner (`MenuPrefabSpawner`)
  - Adds the `Menu`, `UIMenuProfileProvider`, and `MenuGenerator` in one step and calls `menu.Initialize()`

## Notes and Limitations
- Editor‑only tooling: you still need runtime code (e.g., `MenuGenerator`) to render the menu in UI Toolkit
- Renaming root moves underlying files/folders; commit or refresh your VCS accordingly
- “Apply” overwrites/moves assets under the chosen folder; avoid editing those files manually while the builder is open
- Some nodes may hide array/reference fields depending on type (e.g., categories vs selectable items)
- The builder relies on UnityEssentials editor utilities; ensure those modules are installed

## Files in This Package
- `Editor/MenuEditor.cs` – Menu Builder window (tree + dynamic inspector, Apply/Revert)
- `Editor/MenuEditorAssetSerializer.cs` – Save/move/clean asset graph; repopulate references
- `Editor/MenuPrefabSpawner.cs` – GameObject → UI Toolkit → Add Menu
- `Editor/UnityEssentials.UIToolkitMenuGenerator.Editor.asmdef` – Editor assembly definition

## Tags
unity, ui toolkit, menu, data‑driven, scriptableobject, generator, tree view, editor, uxml
