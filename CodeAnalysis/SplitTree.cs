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

    public class SplitTree
    {
        static string pathSolution = @"D:\Darik\Диплом\AdoTest\AdoTest.sln";
        static string pathProject = @"D:\Darik\Диплом\AdoTest\AdoTest\AdoTest.csproj";
        static string pathFile = @"D:\Darik\Диплом\EM6Test\EM6Test\Program.cs";
        static string type = "Solution";
        public List<MethodStruct> methodStructs = new List<MethodStruct>();
        //static IEnumerable<Microsoft.CodeAnalysis.Project> projects;
        public SplitTree(string _type, string _path)
        {
            type = _type;
            if (type == "Solution")
            {
                pathSolution = _path;
            }
            else
            {
                if (type == "Project")
                {
                    pathProject = _path;
                }
                else
                {

                    if (type == "File")
                    {
                        pathFile = _path;
                    }
                }
            }
        }

        public void StartSplit()
        {
            if (type == "Solution")
            {
                SplitSolution();
            }
            else
            {
                if (type == "Project")
                {
                    SplitProject();
                }
                else
                {

                    if (type == "File")
                    {
                        SplitFile();
                    }
                }
            }
        }
        //Метод который разбивает заданное решение на проекты
        void SplitSolution()
        {
            MethodStruct methodStruct = new MethodStruct();

            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(pathSolution).Result;

            var projects = solution.Projects;

            methodStruct.solution = solution;

            SplitProject(projects, methodStruct);
        }
        //Метод который разбивает заданный проект на файлы
        void SplitProject()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var project = msWorkspace.OpenProjectAsync(pathProject).Result;

            var files = project.Documents;

            MethodStruct methodStruct = new MethodStruct();
            methodStruct.project = project;

            SplitFile(files, methodStruct);
        }
        //Метод который разбивает заданные проекты на файлы
        void SplitProject(IEnumerable<Microsoft.CodeAnalysis.Project> projects, MethodStruct methodStruct)
        { 
            foreach(var project in projects)
            {
                var files = project.Documents;
                methodStruct.project = project;
                SplitFile(files, methodStruct);
            }
        }
        //Метод который разбивает заданный файл на методы
        void SplitFile()
        {
            string programText = File.ReadAllText(pathFile);

            var tree = CSharpSyntaxTree.ParseText(programText);

            MethodStruct methodStruct = new MethodStruct();
            methodStruct.documentPath = pathFile;

            IEnumerable<MethodDeclarationSyntax> methods = tree
            .GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                methodStruct.method = method;
                MethodStruct met = new MethodStruct();
                met.solution = methodStruct.solution;
                met.project = methodStruct.project;
                met.document = methodStruct.document;
                met.documentPath = methodStruct.documentPath;
                met.method = methodStruct.method;
                methodStructs.Add(methodStruct);
            }
        }
        //Метод который разбивает заданные файлы на методы
        void SplitFile(IEnumerable<Microsoft.CodeAnalysis.Document> files, MethodStruct methodStruct)
        {
            foreach (var file in files)
            {
                string programText = File.ReadAllText(file.FilePath);

                var tree = CSharpSyntaxTree.ParseText(programText);

                methodStruct.document = file;

                IEnumerable<MethodDeclarationSyntax> methods = tree
                .GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

                foreach (var method in methods) 
                {
                    methodStruct.method = method;
                    MethodStruct met = new MethodStruct();
                    met.solution = methodStruct.solution;
                    met.project = methodStruct.project;
                    met.document = methodStruct.document;
                    met.documentPath = methodStruct.documentPath;
                    met.method = methodStruct.method;
                    methodStructs.Add(met);
                }
                
            }
        }

    }
}
