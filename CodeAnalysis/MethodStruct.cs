using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class MethodStruct
    {
        public Solution solution = null;
        public Project project = null;
        public Document document = null;
        public string documentPath = null;
        public MethodDeclarationSyntax method = null;
    }
}
