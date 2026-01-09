
namespace CodeStandardsChecker.Reporters
{
    public class HTMLReportGenerator
    {
        public System.String fncGenerateReport(System.Collections.Generic.List<CodeStandardsChecker.Models.Violation> palViolations)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Calculate statistics
            int totalViolations = palViolations.Count;
            int uniqueFiles = fncGetUniqueFileCount(palViolations);
            int changeFromLastWeek = 3; // You can make this dynamic

            DateTime reportDate = DateTime.Now;

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { ");
            sb.AppendLine("            font-family: 'Segoe UI', Arial, sans-serif; ");
            sb.AppendLine("            padding: 20px; ");
            sb.AppendLine("            background-color: #f0f2f5; ");
            sb.AppendLine("            margin: 0;");
            sb.AppendLine("        }");
            sb.AppendLine("        .container { ");
            sb.AppendLine("            background-color: white; ");
            sb.AppendLine("            padding: 30px; ");
            sb.AppendLine("            border-radius: 8px; ");
            sb.AppendLine("            max-width: 800px; ");
            sb.AppendLine("            margin: 0 auto;");
            sb.AppendLine("            box-shadow: 0 2px 8px rgba(0,0,0,0.1);");
            sb.AppendLine("        }");
            sb.AppendLine("        .header { ");
            sb.AppendLine("            background: linear-gradient(135deg, #0078d4 0%, #005a9e 100%);");
            sb.AppendLine("            color: white; ");
            sb.AppendLine("            padding: 25px; ");
            sb.AppendLine("            border-radius: 8px; ");
            sb.AppendLine("            margin-bottom: 25px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .header h1 {");
            sb.AppendLine("            margin: 0 0 10px 0;");
            sb.AppendLine("            font-size: 24px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .header p {");
            sb.AppendLine("            margin: 0;");
            sb.AppendLine("            opacity: 0.9;");
            sb.AppendLine("            font-size: 14px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .summary {");
            sb.AppendLine("            background-color: #e3f2fd;");
            sb.AppendLine("            padding: 20px;");
            sb.AppendLine("            border-radius: 8px;");
            sb.AppendLine("            margin-bottom: 25px;");
            sb.AppendLine("            border-left: 4px solid #2196f3;");
            sb.AppendLine("        }");
            sb.AppendLine("        .stat-box {");
            sb.AppendLine("            display: inline-block;");
            sb.AppendLine("            background-color: white;");
            sb.AppendLine("            padding: 15px 20px;");
            sb.AppendLine("            margin: 8px 8px 8px 0;");
            sb.AppendLine("            border-radius: 6px;");
            sb.AppendLine("            box-shadow: 0 1px 3px rgba(0,0,0,0.1);");
            sb.AppendLine("        }");
            sb.AppendLine("        .stat-box strong {");
            sb.AppendLine("            color: #0078d4;");
            sb.AppendLine("            font-size: 24px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .violation { ");
            sb.AppendLine("            padding: 15px; ");
            sb.AppendLine("            margin: 10px 0; ");
            sb.AppendLine("            background-color: #fff9e6; ");
            sb.AppendLine("            border-left: 4px solid #ffa726;");
            sb.AppendLine("            border-radius: 4px;");
            sb.AppendLine("            transition: all 0.2s;");
            sb.AppendLine("        }");
            sb.AppendLine("        .violation:hover {");
            sb.AppendLine("            background-color: #fff3cd;");
            sb.AppendLine("            box-shadow: 0 2px 4px rgba(0,0,0,0.1);");
            sb.AppendLine("        }");
            sb.AppendLine("        input[type=\"checkbox\"] { ");
            sb.AppendLine("            margin-right: 12px; ");
            sb.AppendLine("            width: 18px; ");
            sb.AppendLine("            height: 18px; ");
            sb.AppendLine("            cursor: pointer;");
            sb.AppendLine("            vertical-align: middle;");
            sb.AppendLine("        }");
            sb.AppendLine("        label { ");
            sb.AppendLine("            cursor: pointer;");
            sb.AppendLine("            display: inline-block;");
            sb.AppendLine("            width: calc(100% - 30px);");
            sb.AppendLine("            vertical-align: middle;");
            sb.AppendLine("        }");
            sb.AppendLine("        .file-name { ");
            sb.AppendLine("            font-weight: 600; ");
            sb.AppendLine("            color: #0078d4; ");
            sb.AppendLine("            font-size: 15px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .line-number { ");
            sb.AppendLine("            color: #666; ");
            sb.AppendLine("            font-family: 'Consolas', 'Courier New', monospace;");
            sb.AppendLine("            background-color: #f5f5f5;");
            sb.AppendLine("            padding: 2px 6px;");
            sb.AppendLine("            border-radius: 3px;");
            sb.AppendLine("            font-size: 13px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .message {");
            sb.AppendLine("            color: #333;");
            sb.AppendLine("            margin-top: 5px;");
            sb.AppendLine("            font-size: 14px;");
            sb.AppendLine("        }");
            sb.AppendLine("        .footer {");
            sb.AppendLine("            margin-top: 30px;");
            sb.AppendLine("            padding-top: 20px;");
            sb.AppendLine("            border-top: 2px solid #e0e0e0;");
            sb.AppendLine("            text-align: center;");
            sb.AppendLine("            color: #666;");
            sb.AppendLine("            font-size: 13px;");
            sb.AppendLine("        }");
            sb.AppendLine("        h2 {");
            sb.AppendLine("            color: #333;");
            sb.AppendLine("            margin: 25px 0 15px 0;");
            sb.AppendLine("            font-size: 20px;");
            sb.AppendLine("        }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"container\">");
            sb.AppendLine("        <div class=\"header\">");
            sb.AppendLine("            <h1>📋 Code Standards Report</h1>");
            sb.AppendLine($"            <p>Week of {reportDate:MMMM d, yyyy} • Check off items as you complete them</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        ");
            sb.AppendLine("        <div class=\"summary\">");
            sb.AppendLine("            <h2 style=\"margin-top: 0; color: #1976d2;\">📊 Summary</h2>");
            sb.AppendLine("            <div class=\"stat-box\">");
            sb.AppendLine($"                📝 <strong>{totalViolations}</strong> violations");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"stat-box\">");
            sb.AppendLine($"                📁 <strong>{uniqueFiles}</strong> files");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"stat-box\">");
            sb.AppendLine($"                📈 <strong>+{changeFromLastWeek}</strong> from last week");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        ");
            sb.AppendLine("        <h2>🔍 Violations Found</h2>");
            sb.AppendLine("        ");

            // Iterate through violations
            for (int i = 0; i < palViolations.Count; i++)
            {
                Models.Violation vViolation = palViolations[i];
                string checkboxId = $"v{i + 1}";

                sb.AppendLine("        <div class=\"violation\">");
                sb.AppendLine($"            <input type=\"checkbox\" id=\"{checkboxId}\">");
                sb.AppendLine($"            <label for=\"{checkboxId}\">");
                sb.AppendLine($"                <div>");
                sb.AppendLine($"                    <span class=\"file-name\">{vViolation.prpFilePath}:</span>");
                sb.AppendLine($"                    <span class=\"line-number\">Line {vViolation.prpLineNumber}</span>");
                sb.AppendLine($"                </div>");
                sb.AppendLine($"                <div class=\"message\">{vViolation.prpMessage}</div>");
                sb.AppendLine("            </label>");
                sb.AppendLine("        </div>");
                sb.AppendLine("        ");
            }

            sb.AppendLine("        <div class=\"footer\">");
            sb.AppendLine("            <p><strong>Questions?</strong> Contact your development team lead</p>");
            sb.AppendLine($"            <p>Generated by Code Standards Checker v1.0 • {DateTime.Now:dddd, MMMM d, yyyy}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
        private int fncGetUniqueFileCount(List<CodeStandardsChecker.Models.Violation> palViolations)
        {
            HashSet<System.String> alstrUniqueFiles = new HashSet<string>();
            foreach (Models.Violation vViolation in palViolations)
            {
                alstrUniqueFiles.Add(vViolation.prpFilePath!);
            }
            return alstrUniqueFiles.Count;
        }
    }
}
