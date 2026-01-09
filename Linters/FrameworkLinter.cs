namespace CodeStandardsChecker.Linters
{
    public class FrameworkLinter : Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker
    {
        #region Class Declarations
        private Microsoft.CodeAnalysis.SemanticModel msmSemantic;
        private System.Collections.Generic.Dictionary<System.Int32, System.String> malRegionsMap = new System.Collections.Generic.Dictionary<System.Int32, System.String>();
        private System.Collections.Generic.List<CodeStandardsChecker.Models.Violation> malViolations = new System.Collections.Generic.List<CodeStandardsChecker.Models.Violation>();
        #endregion

        public FrameworkLinter(Microsoft.CodeAnalysis.SemanticModel psmSemantic, Microsoft.CodeAnalysis.SyntaxNode psnRoot) : base(Microsoft.CodeAnalysis.SyntaxWalkerDepth.StructuredTrivia)
        {
            this.msmSemantic = psmSemantic;
            fncBuildRegionMap(malRegionsMap, psnRoot);
        }

        //#region VisitFieldDeclaration
        //public override void VisitFieldDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax pfdsNode)
        //{
        //    System.Console.WriteLine($"Visiting: {pfdsNode.GetType().Name}");
        //    System.Int32 intLineNumber = pfdsNode.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        //    System.String strFilePath = System.IO.Path.GetFileName(pfdsNode.SyntaxTree.FilePath);
        //    System.String strCurrentRegion = malRegionsMap.GetValueOrDefault(intLineNumber, "");
        //    System.String strActualTypeName = pfdsNode.Declaration.Type.ToString();
        //    System.String strExpectedTypeName = fncGetExpectedTypeName(pfdsNode.Declaration.Type);
        //    System.String strRegionPrefix = CodeStandardsChecker.Utilities.PrefixMapper.RegionPrefixes.GetValueOrDefault(strCurrentRegion, "");
        //    System.String strTypePrefix = CodeStandardsChecker.Utilities.PrefixMapper.TypePrefixes.GetValueOrDefault(strExpectedTypeName, "");
        //    System.String strBaseName = strActualTypeName.StartsWith("c") ? strActualTypeName.Substring(1) : strActualTypeName;
        //    System.String strExpectedName = System.String.Empty;
        //    System.String strActualName = System.String.Empty;
        //    System.String strcApplicationFunctions = "cApplicationFunctions";
        //    System.String strcApplicationFunctionsNullable = "cApplicationFunctions?";


        //    // 1. Check if VariableDeclaration IdentifierName is NOT a fully qualified name: (violation!)
        //    if (!strActualTypeName.Equals(strExpectedTypeName, StringComparison.Ordinal))
        //    {
        //        malViolations.Add(new CodeStandardsChecker.Models.Violation(
        //            strFilePath,
        //            intLineNumber,
        //            $"Variable of type'{strActualTypeName}' should be declared as '{strExpectedTypeName}'",
        //            pfdsNode
        //        ));
        //    }

        //    // 2. Check if VariableDeclaration has multiple VariableDeclarators declared together: (violation!)
        //    if (pfdsNode.Declaration.Variables.Count > 1)
        //    {
        //        malViolations.Add(new CodeStandardsChecker.Models.Violation(
        //            strFilePath,
        //            intLineNumber,
        //            "Do not declare multiple variables in one statement",
        //            pfdsNode
        //        ));
        //    }

        //    // Check cApplicationFuntions? != cApplicationFunctions <- Shouldn't this be nullable?: (violation!)
        //    if (!strActualTypeName.Equals(strcApplicationFunctions, StringComparison.Ordinal))
        //    {
        //        malViolations.Add(new CodeStandardsChecker.Models.Violation(
        //            strFilePath,
        //            intLineNumber,
        //            $"Type '{strActualTypeName}' needs to be nullable and should be named '{strcApplicationFunctionsNullable}'",
        //            pfdsNode
        //        ));
        //    }

        //    // If cApplicationFuntions? == cApplicationFuntions? then build the expected variable name.
        //    if (strActualTypeName.Equals(strcApplicationFunctionsNullable, StringComparison.Ordinal))
        //    {
        //        strExpectedName = strRegionPrefix + strTypePrefix + strBaseName;
        //    }

        //    foreach (Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax variable in pfdsNode.Declaration.Variables)
        //    {
        //        strActualName = variable.Identifier.Text;
        //    }

        //    // They MUST match exactly
        //    if (!strActualName.Equals(strExpectedName, StringComparison.Ordinal))
        //    {
        //        malViolations.Add(new CodeStandardsChecker.Models.Violation(
        //            strFilePath,
        //            intLineNumber,
        //            $"Variable '{strActualName}' should be named '{strExpectedName}'",
        //            pfdsNode
        //        ));
        //    }

        //    base.VisitFieldDeclaration(pfdsNode);
        //}
        //#endregion

        #region VisitPropertyDeclaration
        public override void VisitPropertyDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax pfdsNode)
        {
            System.Int32 intLineNumber = pfdsNode.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            System.String strFilePath = System.IO.Path.GetFileName(pfdsNode.SyntaxTree.FilePath);
            System.String strCurrentRegion = malRegionsMap.GetValueOrDefault(intLineNumber, "");
            System.String strActualTypeName = pfdsNode.Type.ToString();
            System.String strExpectedTypeName = fncGetExpectedTypeName(pfdsNode.Type);
            System.String strRegionPrefix = CodeStandardsChecker.Utilities.PrefixMapper.RegionPrefixes.GetValueOrDefault(strCurrentRegion, "");
            System.String strExpectedName = System.String.Empty;
            System.String strActualName = pfdsNode.Identifier.Text;
            System.String strActualNamePrefix = strActualName.Substring(0, (strRegionPrefix.Length));

            if (strExpectedTypeName == "System.String")
            { 
                // Strings need to be nullable: (violation!)
                if (pfdsNode.Type is not Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax)
                {
                    malViolations.Add(new CodeStandardsChecker.Models.Violation(
                        strFilePath,
                        intLineNumber,
                        $"Type '{strActualTypeName}' needs to be nullable.'",
                        pfdsNode
                    ));
                }
                strActualTypeName = strActualTypeName.Replace("?", "");
            }

            // Check if PropertyDeclaration IdentifierName is NOT a fully qualified name: (violation!)
            if (strActualTypeName != strExpectedTypeName)
            {
                malViolations.Add(new CodeStandardsChecker.Models.Violation(
                    strFilePath,
                    intLineNumber,
                    $"Property of type '{strActualTypeName}' should be declared fully qualified as '{strExpectedTypeName}'",
                    pfdsNode
                ));
            }



            // Check if prefixes match, if they don't: (violation!)
            if (strActualNamePrefix != strRegionPrefix)
            {
                malViolations.Add(new CodeStandardsChecker.Models.Violation(
                    strFilePath,
                    intLineNumber,
                    $"Property '{strActualName}' should have the prefix '{strRegionPrefix}'",
                    pfdsNode
                ));
            }

            base.VisitPropertyDeclaration(pfdsNode);
        }
        #endregion

        #region VisitVariableDeclaration
        public override void VisitVariableDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax pfdsNode)
        {
            //System.Console.WriteLine($"Visiting: {pfdsNode.GetType().Name}");
            System.Int32 intLineNumber = pfdsNode.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            System.String strFilePath = System.IO.Path.GetFileName(pfdsNode.SyntaxTree.FilePath);
            System.String strCurrentRegion = malRegionsMap.GetValueOrDefault(intLineNumber, "");
            System.String strActualTypeName = pfdsNode.Type.ToString();
            System.String strExpectedTypeName = fncGetExpectedTypeName(pfdsNode.Type);
            System.String strRegionPrefix = CodeStandardsChecker.Utilities.PrefixMapper.RegionPrefixes.GetValueOrDefault(strCurrentRegion, "");
            System.String strTypePrefix = CodeStandardsChecker.Utilities.PrefixMapper.TypePrefixes.GetValueOrDefault(strExpectedTypeName, "");
            System.String strBaseName = String.Empty;
            System.String strExpectedName = System.String.Empty;
            System.String strActualName = System.String.Empty;
            System.String strExpectedNamePrefix = System.String.Empty;
            System.String strActualNamePrefix = System.String.Empty;
            System.String strcApplicationFunctions = "cApplicationFunctions";
            System.String strcApplicationFunctionsNullable = "cApplicationFunctions?";

            foreach (Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax variable in pfdsNode.Variables)
            {
                strActualName = variable.Identifier.Text;
            }

            // Check if VariableDeclaration has multiple VariableDeclarators declared together: (violation!)
            if (pfdsNode.Variables.Count > 1)
            {
                malViolations.Add(new CodeStandardsChecker.Models.Violation(
                    strFilePath,
                    intLineNumber,
                    "Do not declare multiple variables in one statement",
                    pfdsNode
                ));
            }

            if (pfdsNode.Type is Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax)
            {
                // Special check for cApplicationFunctions Declarations
                if (strActualTypeName == strcApplicationFunctions || strActualTypeName == strcApplicationFunctionsNullable)
                {
                    strExpectedName = strRegionPrefix + strTypePrefix + "ApplicationFunctions";

                    // If type name is not 'cApplicationFuntions?' (Shouldn't this be nullable?): (violation!)
                    if (strActualTypeName != strcApplicationFunctionsNullable)
                    {
                        malViolations.Add(new CodeStandardsChecker.Models.Violation(
                            strFilePath,
                            intLineNumber,
                            $"Type '{strActualTypeName}' needs to be nullable and should be named '{strcApplicationFunctionsNullable}'",
                            pfdsNode
                        ));
                    }

                    // If not called mobjApplicationFunctions as a class or objApplicationFunctions if declared elsewhere : (violation!)
                    if (strActualName != strExpectedName)
                    {
                        malViolations.Add(new CodeStandardsChecker.Models.Violation(
                            strFilePath,
                            intLineNumber,
                            $"Variable '{strActualName}' should be named '{strExpectedName}'",
                            pfdsNode
                        ));
                    }
                }
            }
            else
            {
                strExpectedNamePrefix = strRegionPrefix + strTypePrefix;
                strActualNamePrefix = strActualName.Substring(0, (strExpectedNamePrefix.Length));

                // If VariableDeclaration IdentifierName is NOT a fully qualified name: (violation!)
                if (strActualTypeName != strExpectedTypeName)
                {
                    malViolations.Add(new CodeStandardsChecker.Models.Violation(
                        strFilePath,
                        intLineNumber,
                        $"Variable of type '{strActualTypeName}' should be declared fully qualified as '{strExpectedTypeName}'",
                        pfdsNode
                    ));
                }

                // If no prefix for this type exists in PrefixMapper.TypePrefixes: (violation!)
                if (System.String.IsNullOrEmpty(strTypePrefix))
                {
                    malViolations.Add(new CodeStandardsChecker.Models.Violation(
                        strFilePath,
                        intLineNumber,
                        $"LINTER CONFIG ERROR: No prefix mapping found for type '{strExpectedTypeName}'. Add this type to PrefixMapper.TypePrefixes.",
                        pfdsNode
                    ));
                }
                else
                {
                    // If prefixes don't match: (violation!)
                    if (strActualNamePrefix != strExpectedNamePrefix)
                    {
                        malViolations.Add(new CodeStandardsChecker.Models.Violation(
                            strFilePath,
                            intLineNumber,
                            $"Variable '{strActualName}' should have the prefix '{strExpectedNamePrefix}'",
                            pfdsNode
                        ));
                    }
                }
            }
            //if (intLineNumber == 903)
            //{
            //    System.String dummyCode = System.String.Empty;
            //}
            base.VisitVariableDeclaration(pfdsNode);
        }
        #endregion

        #region VisitFunctionDeclaration
        public override void VisitMethodDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax pfdsNode)
        {

            base.VisitMethodDeclaration(pfdsNode);
        }
        #endregion

        #region fncGetCurrentRegion
        private System.String fncGetCurrentRegion(System.Collections.Generic.Stack<System.String> pstralCurrentRegions)
        {
            if (pstralCurrentRegions.Count > 0)
                return pstralCurrentRegions.Peek();
            return "";
        }
        #endregion

        #region fncGetExpectedTypeName
        private System.String fncGetExpectedTypeName(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ptsType)
        {
            Microsoft.CodeAnalysis.TypeInfo typeInfo = Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetTypeInfo(msmSemantic, ptsType);
            Microsoft.CodeAnalysis.ITypeSymbol? typeSymbol = typeInfo.Type;


            if (typeSymbol == null)
                return "Unknown";

            Microsoft.CodeAnalysis.SymbolDisplayFormat format = new Microsoft.CodeAnalysis.SymbolDisplayFormat(
                globalNamespaceStyle: Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions:
                    Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            );

            return typeSymbol.ToDisplayString(format);
        }
        #endregion

        #region fncBuildRegionMap
        // This will work for non-nested regions. Check documentation to confirm that won't be an issue.
        private void fncBuildRegionMap(System.Collections.Generic.Dictionary<System.Int32, System.String> palRegionsMap, Microsoft.CodeAnalysis.SyntaxNode psnRoot)
        {
            System.String strCurrentRegionName = System.String.Empty;
            System.Int32 intCurrentRegionStartLine = 0;

            foreach (Microsoft.CodeAnalysis.SyntaxTrivia stTrivia in psnRoot.DescendantTrivia())
            {
                if (stTrivia.HasStructure &&  Microsoft.CodeAnalysis.CSharpExtensions.IsKind(stTrivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind.RegionDirectiveTrivia))
                {
                    Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax? tsChildRegionNode = stTrivia.GetStructure() as Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax;
                    strCurrentRegionName = tsChildRegionNode!.EndOfDirectiveToken.ToFullString().Trim().ToLower();
                    intCurrentRegionStartLine = stTrivia.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    palRegionsMap.Add(intCurrentRegionStartLine, strCurrentRegionName);
                    Console.WriteLine($"Line #{intCurrentRegionStartLine}: {stTrivia.ToString()}");
                }
                else if (Microsoft.CodeAnalysis.CSharpExtensions.IsKind(stTrivia, Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndRegionDirectiveTrivia))
                {
                    System.Int32 intEndRegionLine = stTrivia.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    for (System.Int32 i = intCurrentRegionStartLine + 1; i <= intEndRegionLine; i++)
                    {
                        palRegionsMap.Add(i, strCurrentRegionName);
                    }
                    Console.WriteLine($"Line #{intEndRegionLine}: {stTrivia.ToString()}");
                }
            }
        }
        #endregion

        #region fncGetViolations
        public System.Collections.Generic.List<CodeStandardsChecker.Models.Violation> fncGetViolations() => malViolations;
        #endregion




        // ----------------------------------------------------------
        // No longer needed
        // ----------------------------------------------------------
        #region VisitToken
        ////public override void VisitToken(Microsoft.CodeAnalysis.SyntaxToken psttoken)
        ////{
        ////    // Process leading trivia first (this includes #region)
        ////    foreach (Microsoft.CodeAnalysis.SyntaxTrivia trivia in psttoken.LeadingTrivia)
        ////    {
        ////        if (trivia.HasStructure)
        ////        {
        ////            Visit(trivia.GetStructure());  // GetStructure() returns the SyntaxNode
        ////        }
        ////    }

        ////    base.VisitToken(psttoken);
        ////}
        #endregion

        #region VisitRegionDirectiveTrivia
        //// Track when entering a region
        ////public override void VisitRegionDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax pdtsNode)
        ////{
        ////    System.Console.WriteLine($"Visiting: {pdtsNode.GetType().Name}");
        ////    System.String regionName = pdtsNode.EndOfDirectiveToken.ToFullString().Trim().ToLower();
        ////    mstralCurrentRegions.Push(regionName);
        ////    base.VisitRegionDirectiveTrivia(pdtsNode);
        ////}
        #endregion

        #region VisitEndRegionDirectiveTrivia
        //// Track when leaving a region
        ////public override void VisitEndRegionDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.EndRegionDirectiveTriviaSyntax pdtsNode)
        ////{
        ////    if (mstralCurrentRegions.Count > 0)
        ////        mstralCurrentRegions.Pop();
        ////    base.VisitEndRegionDirectiveTrivia(pdtsNode);
        ////}
        #endregion

        #region fncGetActualTypeName
        //private System.String fncGetActualTypeName(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax ptsType)
        //{
        //    // Handle simple identifier (cApplicationFunctions, List, etc.)
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax identifier)
        //    {
        //        return identifier.Identifier.Text;
        //    }

        //    // Handle qualified names (System.String, System.Collections.Generic.List)
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualified)
        //    {
        //        return qualified.Right.Identifier.Text;  // Get the last part
        //    }

        //    // Handle generic types (List<int>, Dictionary<string, int>)
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax generic)
        //    {
        //        return generic.Identifier.Text;  // Just "List", not "List<int>"
        //    }

        //    // Handle predefined types (string, int, bool)
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.PredefinedTypeSyntax predefined)
        //    {
        //        return predefined.Keyword.Text;
        //    }

        //    // Handle arrays (int[], string[])
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax arrayType)
        //    {
        //        return fncGetActualTypeName(arrayType.ElementType);  // Recursive call
        //    }

        //    // Handle nullable types (int?, string?)
        //    if (ptsType is Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax nullable)
        //    {
        //        return fncGetActualTypeName(nullable.ElementType);  // Recursive call
        //    }

        //    return "Unknown";  // Fallback
        //}
        #endregion


    }
}
