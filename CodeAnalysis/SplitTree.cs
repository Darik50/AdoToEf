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
        //Метод который разбивает заданное решение на проекты
        public void SplitSolution()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(pathSolution).Result;

            var projects = solution.Projects;

            SplitProject(projects);
        }
        //Метод который разбивает заданный проект на файлы
        public void SplitProject()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var project = msWorkspace.OpenProjectAsync(pathProject).Result;

            var files = project.Documents;

            SplitFiles(files);
        }
        //Метод который разбивает заданные проекты на файлы
        public void SplitProject(IEnumerable<Microsoft.CodeAnalysis.Project> projects)
        { 
            foreach(var project in projects)
            {
                var files = project.Documents;
                SplitFiles(files);
            }
        }
        //Метод который разбивает заданный файл на методы
        public void SplitFiles()
        {
            string programText = File.ReadAllText(pathFile);

            var tree = CSharpSyntaxTree.ParseText(programText);
            
            IEnumerable<MethodDeclarationSyntax> methods = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>();  
        }
        //Метод который разбивает заданные файлы на методы
        public void SplitFiles(IEnumerable<Microsoft.CodeAnalysis.Document> files)
        {
            foreach (var file in files)
            {
                string programText = File.ReadAllText(file.FilePath);

                var tree = CSharpSyntaxTree.ParseText(programText);

                IEnumerable<MethodDeclarationSyntax> methods = tree
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();
                foreach (var method in methods) 
                {
                    string pattern = @"(^[^{]+{)|(}$)";
                    string target = " ";
                    Regex regex = new Regex(pattern);
                    string result = regex.Replace(method.ToString(), target);
                    SearchSQL a = new SearchSQL();
                    a.SearchInto(result);
                }
            }
        }

    }
}
