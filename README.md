# CodeStandardsChecker

A custom C# code linter built on Microsoft's Roslyn compiler platform to enforce Hungarian notation and coding standards across legacy .NET applications.

## Overview

This tool was built to address inconsistent coding standards across 40+ legacy applications at my workplace. Rather than relying on manual code review, I created an automated linter that parses C# source files and detects violations of company-specific naming conventions and style rules.

**Note:** This is a proof-of-concept that achieved its technical learning objectives. The core functionality—syntax tree parsing, semantic type resolution, and violation detection—is complete and functional.

## Features

### Implemented

- **Hungarian Notation Validation**
  - Class-level fields: `m` + type prefix + name (e.g., `mstrFirstName`, `mobjApplicationFunctions`)
  - Local variables: type prefix + name (e.g., `strName`, `intCount`)
  - Parameters: `p` + type prefix + name (e.g., `pstrInput`)
  - Properties: `prp` + name (e.g., `prpCityID`)

- **Type Prefix Mapping**
  - `str` → System.String
  - `int` → System.Int32
  - `bln` → System.Boolean
  - `obj` → Custom classes
  - `sru` → Structs
  - `enm` → Enums
  - And more...

- **Fully Qualified Type Enforcement**
  - Detects `string` and flags it should be `System.String`
  - Detects `int` and flags it should be `System.Int32`

- **Additional Checks**
  - Multiple variable declarations per statement
  - Nullable type requirements
  - Scope-aware validation (distinguishes class fields from local variables)


### Planned (Not Implemented)

- Suppression comments (`LINTER:SUPPRESS` or `NOSCAN`)
- HTML email reports with checkboxes
- TFS integration for scheduled runs
- SQL formatting validation
- Method ordering checks

## Sample Output

![Sample linter output](./images/linter_output.png)

*Lines 15 and 374 demonstrate context-aware validation—the same variable name requires different prefixes depending on whether it's a class-level field (`mobj`) or a local variable (`obj`).*

## Technical Approach

### Roslyn Compiler Platform

This linter uses Microsoft's Roslyn APIs to parse and analyze C# code. Roslyn exposes the same compiler internals that Visual Studio uses, providing access to:

- **Syntax Trees** - The structural representation of source code
- **Semantic Model** - Type information and symbol resolution

### Key Implementation Details

**Syntax Tree Walking**

The linter extends `CSharpSyntaxWalker` to traverse the abstract syntax tree and visit each node:

```csharp
public class FrameworkLinter : CSharpSyntaxWalker
{
    public FrameworkLinter(SemanticModel semanticModel) 
        : base(SyntaxWalkerDepth.StructuredTrivia)
    {
        // StructuredTrivia depth allows visiting #region directives
    }
}
```

**Semantic Model for Type Resolution**

The semantic model resolves what types actually are, not just what's written:

```csharp
private string GetExpectedTypeName(TypeSyntax type)
{
    var typeInfo = semanticModel.GetTypeInfo(type);
    return typeInfo.Type?.ToDisplayString() ?? "Unknown";
}
```

This is necessary because:
- `string` and `System.String` are the same type semantically
- The linter needs to know the fully qualified type to determine the correct prefix

**Region Tracking**

The linter tracks `#region` directives to determine scope context:

```csharp
private Stack<string> currentRegions = new Stack<string>();

public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
{
    string regionName = node.EndOfDirectiveToken.ToFullString().Trim();
    currentRegions.Push(regionName);
}
```

Variables inside `#region Class Declarations` require the `m` prefix; variables elsewhere do not.

## What I Learned

- **Roslyn is the actual C# compiler** exposed as an API—not a separate parsing library
- **Syntax vs Semantics**: The syntax tree shows what's written; the semantic model shows what it means
- **Visitor Pattern**: Walking a tree structure by overriding `Visit*` methods for specific node types
- **Structured Trivia**: Comments and preprocessor directives like `#region` are "trivia" attached to tokens, requiring special handling to access

## Project Status

This project was built as a proof of concept to explore Roslyn's compiler APIs. The core functionality—parsing syntax trees, resolving types via semantic analysis, and detecting Hungarian notation violations—is complete.

The project was presented to my team lead, who appreciated the initiative. However, the team's priority is a multi-year legacy application upgrade effort, so adoption was deferred. The remaining features (email reports, TFS integration) are infrastructure work rather than technical challenges.

**Future Development:** I may continue expanding this tool to add multi-file scanning, HTML reports, and additional rule checks.

## Tech Stack

- C# / .NET 8
- Microsoft.CodeAnalysis.CSharp (Roslyn)
- Console Application

## Building

```bash
dotnet restore
dotnet build
```

## Usage

```bash
dotnet run -- "path/to/file.cs"
```

## License

MIT