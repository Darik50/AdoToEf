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

    class SplitTree
    {
        static string pathSolution = @"D:\Darik\Диплом\AdoTest\AdoTest.sln";
        static string pathProject = @"D:\Darik\Диплом\AdoTest\AdoTest\AdoTest.csproj";
        static string pathFile = @"D:\Darik\Диплом\EM6Test\EM6Test\Program.cs";
        //static IEnumerable<Microsoft.CodeAnalysis.Project> projects;
        public SplitTree(object[] arrPath)
        {
            pathSolution = arrPath[0].ToString();
            if (arrPath.Length == 2)
            {
                pathProject = arrPath[1].ToString();
            }
            if (arrPath.Length == 3)
            {
                pathFile = arrPath[2].ToString();
            }
        }
        public void SplitSolution()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(pathSolution).Result;

            var projects = solution.Projects;

            SplitProject(projects);
        }
        public void SplitProject()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var project = msWorkspace.OpenProjectAsync(pathProject).Result;

            var files = project.Documents;
        }
        public void SplitProject(IEnumerable<Microsoft.CodeAnalysis.Project> projects)
        { 
            foreach(var project in projects)
            {
                var files = project.Documents;
                SplitFiles(files);
            }
        }
        public void SplitFiles()
        {
            string programText = File.ReadAllText(pathFile);

            var tree = CSharpSyntaxTree.ParseText(programText);
            
            IEnumerable<MethodDeclarationSyntax> methods = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>();  
        }
        public void SplitFiles(IEnumerable<Microsoft.CodeAnalysis.Document> files)
        {
            foreach (var file in files)
            {
                string programText = File.ReadAllText(file.FilePath);
                Console.WriteLine(file.FilePath);

                var tree = CSharpSyntaxTree.ParseText(programText);

                IEnumerable<MethodDeclarationSyntax> methods = tree
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();
            }
        }

    }
}
