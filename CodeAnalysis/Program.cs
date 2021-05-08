using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalysis
{
    class Program
    {
        string pathSolution = @"D:\Darik\������\AdoTest\AdoTest.sln";
        string pathProject = @"D:\Darik\������\AdoTest\AdoTest\AdoTest.csproj";
        string pathFile = @"D:\Darik\������\AdoTest\AdoTest\Program.cs";
        static async Task Main(string[] args)
        {
            object[] o = { @"D:\Darik\������\AdoTest\AdoTest.sln" };
            SplitTree a = new SplitTree(o);
            a.SplitSolution();
            Console.WriteLine("----------------");
            a.SplitProject();
            Console.WriteLine("----------------");
            a.SplitFiles();
        }
        
    }
}
