using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Console.WriteLine("������� ���� � ����� ��� �������");
            string typeFile = "";
            string path = Console.ReadLine();
            if (path.Contains(".sln"))
            {
                typeFile = "Solution";
            }
            else
            {
                if (path.Contains(".csproj"))
                {
                    typeFile = "Project";
                }
                else
                {
                    if (path.Contains(".cs"))
                    {
                        typeFile = "File";
                    }
                    else
                    {
                        Console.WriteLine("������ �������� ������ �����");
                        return;
                    }
                }
            }
            ChangeFile a = new ChangeFile(typeFile, path);
            Dictionary<MethodStruct, List<SqlToEfStruct>> arr = a.StartChange();
            Console.WriteLine("������� 1 ���� ���������� ������������ �� �������������� ���� � ��������� �����");
            //Console.WriteLine("������� 2 ���� ���������� ������������ � ���� ������������ � ����� ����� ");
            string take = Console.ReadLine();
            if (take == "1")
            {
                string text = "";
                foreach (var i in arr)
                {
                    text += "���� � ������ ��� �������������� ����:\n" + i.Key.project.Name + "->" + i.Key.document.Name + i.Key.documentPath + "->" + i.Key.method.Identifier + "\n".Replace("->->", "->");
                    foreach (var j in i.Value)
                    {
                        text += j.sqlQuery + "\n" + "��� �������������� ������� SQL-�������, ���������� ������������ ���������� ���:\n" + j.EfBlock + "\n";
                    }
                    text += "\n";
                }
                StreamWriter sw = new StreamWriter(@"C:\Users\������\Desktop\�����.txt");
                sw.WriteLine(text);
                sw.Close();
            }
            //else
            //{
            //    foreach (var i in arr)
            //    {
            //        string code = "";
            //        string pathFile = "";
            //        if (i.Key.documentPath is null) 
            //        {
            //            pathFile = i.Key.document.FilePath;
            //            code = File.ReadAllText(i.Key.document.FilePath);
            //        }
            //        else
            //        {
            //            pathFile = i.Key.documentPath;
            //            code = File.ReadAllText(i.Key.documentPath);
            //        }
            //        foreach(var j in i.Value)
            //        {
            //            string text = "//��� �������������� ������� SQL-�������, ���������� ������������ ���������� ���\n" + j.EfBlock + "\n" + j.replaceLine;
            //            string pattern = @"^(\W)*";
            //            string target = "";
            //            Regex regex = new Regex(pattern);
            //            string line = regex.Replace(j.replaceLine, target);
            //            code.Replace(line, text);
            //        }
            //        File.Create(pathFile.Replace(".cs", "1.cs")).Close();
            //        File.WriteAllText(pathFile.Replace(".cs", "1.cs"), code);
            //    }
            //}
        }
        
    }
}
