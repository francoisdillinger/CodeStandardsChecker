
System.String strFilePath = @"C:\Users\jj\Desktop\appPermits80\Pages\PermitHeader.cshtml.cs";
System.String strSourceCode = System.IO.File.ReadAllText(strFilePath);

Microsoft.CodeAnalysis.SyntaxTree tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(strSourceCode, path: strFilePath);
Microsoft.CodeAnalysis.SyntaxNode root = tree.GetRoot();
Microsoft.CodeAnalysis.MetadataReference[] references = new Microsoft.CodeAnalysis.MetadataReference[]
{
    Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location),
    Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location)
};
// Create compilation
Microsoft.CodeAnalysis.CSharp.CSharpCompilation compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("MyCompilation")
    .AddReferences(references)
    .AddSyntaxTrees(tree);

// Get semantic model
Microsoft.CodeAnalysis.SemanticModel model = compilation.GetSemanticModel(tree);

// Run your linter
CodeStandardsChecker.Linters.FrameworkLinter linter = new CodeStandardsChecker.Linters.FrameworkLinter(model, root);
linter.Visit(root);

List<CodeStandardsChecker.Models.Violation> alViolations = linter.fncGetViolations();

// Create an instance
var objReportGenerator = new CodeStandardsChecker.Reporters.HTMLReportGenerator();

// Generate the HTML
System.String strHtml = objReportGenerator.fncGenerateReport(alViolations);

// Save to temp file
System.String strTempFile = System.IO.Path.GetTempFileName() + ".html";
System.IO.File.WriteAllText(strTempFile, strHtml);

// Open in default browser
System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = strTempFile,
    UseShellExecute = true
});