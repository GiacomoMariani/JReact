# **J_SO_LocalizationLibrary** Usage Guide

## Overview

The **J_SO_LocalizationLibrary**:

- Stores an array of **J_SO_LocalizationEntry** assets.
- Reads and parses localization data from a **TextAsset** (the “source” file).
- Dynamically creates or updates **J_SO_LocalizationEntry** assets based on that source file.

---

## Preparing the Source File

### Format

- **First Line**: Language list (matching `SystemLanguage` names). Example:
  English|Spanish|French

- **Subsequent Lines**: Localization entries. Example:
  GREETING|Hello|Hola|Bonjour BYE|Goodbye|Adiós|Au revoir

- **Delimiter**: Make sure each line uses `|`.

### Example

English|Spanish|French
GREETING|Hello|Hola|Bonjour
BYE|Goodbye|Adiós|Au revoir
WELCOME|Welcome|Bienvenido|Bienvenue

This file (`TextAsset`) is then assigned to the `_source` field of the library.

---

## Library Inspector Fields

**Buttons**
    - **LoadFromSource()**: Reads `_source`, updates `_languages`, and creates/updates `_entries`.
    - **CheckTextDuplicates()**: Scans `_source` for duplicate keys and logs them as warnings in the Console.
    - **HardClear()**: Deletes the `_entryFolder` and clears `_entries` (use carefully).

---

## Loading from Source

1. Assign your `_source` file in the library’s Inspector.
2. Click **LoadFromSource()**:
    - The first line sets `_languages` (e.g., `English`, `Spanish`, `French`).
    - Each subsequent line is used to:
        - Create or update a `J_SO_LocalizationEntry` asset in `_entryFolder`.
        - Reference it in `_entries`.
    - This folder and asset creation happens in the same directory as your library asset.

~~~~---

# **J_LocalizedText** Explanation

This **MonoBehaviour** is designed to tie a specific **TextMeshProUGUI** component (displayed text) to a single **J_SO_LocalizationEntry**, using a **J_SO_LocalizationLibrary** as the master database.

## Editor Buttons

### **GenerateNewEntry()**
- Creates a new localization entry in `_library` based on whatever is currently in `_text.text`.
- If `_text.text` is empty or `_entry` is already assigned, it logs an error instead of creating a duplicate.

### **UpdateEntry()**
- Calls `_library.UpdateEntry(_entry, _text.text)`, overwriting the **default** text of the existing entry with what is currently in `_text.text`.

### **TryCatchEntry()**
- Tries to find an existing entry with the key matching `_text.text` by calling `_library.TryCatch(_text.text)`.
- If no entry is found in the library or the source file, this returns `null`.
- If one does exist, `_entry` is assigned to that existing entry.

```csharp
public J_SO_LocalizationEntry TryCatch(string keyText)
```

Checks if an entry with keyText exists in the current dictionary.
If not found, scans _source for that key.
If present, creates the missing asset and populates it.
If not present, returns null.


