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
        string pathSolution = @"D:\Darik\Диплом\AdoToEf\AdoToEf.sln";
        string pathProject = @"D:\Darik\Диплом\AdoTest\AdoTest\AdoTest.csproj";
        string pathFile = @"D:\Darik\Диплом\AdoTest\AdoTest\Program.cs";
        static async Task Main(string[] args)
        {
            object[] o = { @"D:\Darik\Диплом\EM6Test\EM6Test.sln"};
            SplitTree a = new SplitTree(o);
            a.SplitSolution();
            Console.WriteLine("----------------");
            a.SplitProject();
            Console.WriteLine("----------------");
            a.SplitFiles();
        }
        
    }
}
