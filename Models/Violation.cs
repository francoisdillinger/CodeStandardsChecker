namespace CodeStandardsChecker.Models
{
    public class Violation
    {
        public System.String? prpFilePath { get; set; }
        public System.Int32? prpLineNumber { get; set; }
        public System.String? prpMessage { get; set; }
        public Microsoft.CodeAnalysis.SyntaxNode? prpNode { get; set; }

        public Violation(System.String pstrFilePath, System.Int32 pintLineNumber, System.String pstrMessage, Microsoft.CodeAnalysis.SyntaxNode? psnNode)
        {
            prpFilePath = pstrFilePath;
            prpLineNumber = pintLineNumber;
            prpMessage = pstrMessage;
            prpNode = psnNode;
        }
    }
}
