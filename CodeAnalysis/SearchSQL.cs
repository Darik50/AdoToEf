﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    class SearchSQL
    {
        public void Search(string code)
        {
            List<SqlToEfStruct> arr = new List<SqlToEfStruct>();
            arr.AddRange(SearchInto(code));
            SearchUpdate(code);
            SearchDelete(code);
            SearchSelect(code);
            foreach (var i in arr)
            {
                Console.WriteLine(i.sqlQuery + "\n" + i.replaceLine + "\n" + i.EfBlock);                
            }
        }
        //Поиск запроса INSERT INTO
        public List<SqlToEfStruct> SearchInto(string code)
        {
            string pattern = @"[^;]+(?ixn)""INSERT[ ]+INTO[^;]+";
            Regex regex = new Regex(pattern);
            List<SqlToEfStruct> arr = new List<SqlToEfStruct>();
            foreach (Match match in Regex.Matches(code, pattern)) 
            {
                arr.Add(SplitInto(match.Value));
            }
            return arr;
        }

        //Разделяем на элементы запрос INSERT INTO
        public SqlToEfStruct SplitInto(string sqlQuery)
        {
            SqlToEfStruct intoStruct = new SqlToEfStruct();
            intoStruct.replaceLine = sqlQuery;
            Console.WriteLine("sql запрос");
            Console.WriteLine(sqlQuery);
            string tableName = "";
            string[] columns = new string[0];
            string[] values = new string[0];

            //убираем лишние проблемы
            string pattern = @"\s+";
            string target = " ";
            Regex regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"^(.)+(?ixn)INSERT[ ]INTO[ ]";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            intoStruct.sqlQuery = "INSERT INTO " + sqlQuery.Trim();

            //получаем имя таблицы
            pattern = @"(?ixn)^[ ]*[^ (]+([ ]|[(])";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                tableName = match.Value.Replace(" ", "").Replace("(", "");
            }
            sqlQuery = regex.Replace(sqlQuery, target);

            //получаем имена столбцов
            pattern = @"(?ixn)^[ ]*VALUES[ ]*[(]";
            regex = new Regex(pattern);
            if (!regex.IsMatch(sqlQuery))
            {
                pattern = @"^[ ]*[^)]+[)]";
                regex = new Regex(pattern);
                foreach (Match match in Regex.Matches(sqlQuery, pattern))
                {
                    columns = match.Value.Replace(" ", "").Replace(")", "").Replace("(", "").Split(',');
                }
                sqlQuery = regex.Replace(sqlQuery, target);
            }

            //получаем значения
            pattern = @"(?ixn)^[ ]*[^(]+[(]";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);            
            pattern = @"^[ ]*[^)]+[)]";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                values = match.Value.Replace(" ", "").Replace(")", "").Split(',');
            }
            sqlQuery = regex.Replace(sqlQuery, target);

            intoStruct = CreateInto(tableName, columns, values, intoStruct);
            return intoStruct;
        }

        //Создание аналога запроса INSERT INTO, с помощью EF
        SqlToEfStruct CreateInto(string tableName, string[] columns, string[] values, SqlToEfStruct intoStruct)
        {
            Console.WriteLine("EF6");
            string EFBlock = "using (ApplicationContext db = new ApplicationContext())\n{\n\t";

            EFBlock += tableName + " str = new " + tableName + @"{ ";
            if(columns.Length == 0)//генерируем имена столбцов если их не было в запросе
            {
                columns = new string[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    columns[i] = "<column" + i.ToString() + ">"; 
                }
            }
            for(int i = 0; i < values.Length; i++)
            {
                if (i < values.Length - 1)
                {
                    EFBlock += columns[i] + " = " + values[i].Replace(@"'", @"""") + ", ";
                }
                else
                {
                    EFBlock += columns[i] + " = " + values[i].Replace(@"'", @"""") + "}\n\t";
                }
            }

            EFBlock += "db.Test.Add(t1);\n\tdb.SaveChanges();\n}";
            Console.WriteLine(EFBlock);
            Console.WriteLine("----------------------");
            intoStruct.EfBlock = EFBlock;
            return intoStruct;
        }
        //update
        public void SearchUpdate(string code)
        {
            string pattern = @"[^;]+(?ixn)""UPDATE[^;]+";
            Regex regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(code, pattern))
            {
                SplitUpdate(match.Value);
            }
        }
        //Разделяем на элементы запрос UPDATE
        public void SplitUpdate(string sqlQuery)
        {
            Console.WriteLine("sql запрос");
            Console.WriteLine(sqlQuery);
            string tableName = "";
            List<string> columns = new List<string>();
            List<string> values = new List<string>();

            //убираем лишние проблемы
            string pattern = @"\s+";
            string target = " ";
            Regex regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"^(.)+(?ixn)UPDATE[ ]";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            //получаем имя таблицы
            pattern = @"(?ixn)^[ ]*[^ ]+";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                tableName = match.Value.Replace(" ", "");
            }
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"(?ixn)^[ ]*SET";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);
            bool flag = true;
            bool flagWhere = false;
            if (sqlQuery.ToUpper().Contains(" WHERE "))
            {
                pattern = @"^[^=]+[=](([^,]*[,])|(.*(?ixn)((['][ ]*WHERE)|([ ]+WHERE))))";
                flagWhere = true;
            }
            else
            {
                pattern = @"^[^=]+[=](([^,]*[,])|(.*$)|(.* ""))";
            }
            regex = new Regex(pattern);
            while (flag)
            {
                flag = false;
                foreach (Match match in Regex.Matches(sqlQuery, pattern))
                {
                    flag = true;
                    string line = match.Value;
                    string pat = @"[ ]*(((?ixn)WHERE$)|([,]$))";
                    string tar = "";
                    Regex reg = new Regex(pat);
                    line = reg.Replace(line, tar);
                    string[] colVal = line.Split('=');
                    columns.Add(colVal[0]);
                    values.Add(colVal[1]);
                }
                sqlQuery = regex.Replace(sqlQuery, target);
            }
            string constrLine = "";
            if (flagWhere)
            {
                constrLine = sqlQuery;
            }
            CreateUpdate(tableName, columns, values, constrLine);
        }

        void CreateUpdate(string tableName, List<string> columns, List<string> values, string constrLine)
        {
            Console.WriteLine("EF6");
            string EMBlock = "using (ApplicationContext db = new ApplicationContext())\n{\n\t";

            EMBlock += "db." + tableName;
            if (constrLine.Length > 0)
            {
                EMBlock += ".Where(x => ";
                Dictionary<string, string> replaceStr = new Dictionary<string, string>();
                string[] constrns = constrLine.Split(new[] { " AND ", " OR " }, StringSplitOptions.None);
                foreach (var i in constrns)
                {
                    string pattern = @"^[^=<>]+";
                    Regex regex = new Regex(pattern);
                    foreach (Match match in Regex.Matches(i, pattern))
                    {
                        string newColumn = "x." + match.Value.Replace(" ", "") + " ";
                        if (!replaceStr.ContainsKey(match.Value.Replace(" ", "")))
                        {
                            replaceStr.Add(match.Value.Replace(" ", ""), newColumn);
                        }
                    }
                }

                foreach (var i in replaceStr)
                {
                    constrLine = constrLine.Replace(i.Key, i.Value);
                }
                constrLine = constrLine.Replace("=", "==").Replace("AND", "&&").Replace("OR", "||");
                EMBlock += constrLine + ")";
            }
            EMBlock += ".ToList().ForEach(a =>\n\t{\n";

            for(int i = 0; i < values.Count; i++)
            {
                EMBlock += "\t\ta." + columns[i].Replace(" ", "") + " = " + values[i].Replace(@"'", @"""") + ";\n";
            }
            EMBlock += "\t});\n\tdb.SaveChanges();\n}";
            Console.WriteLine(EMBlock);
            Console.WriteLine("----------------------");
        }

        //delete
        public void SearchDelete(string code)
        {
            string pattern = @"[^;]+(?ixn)""DELETE[ ]+((FROM[ ])|())[^;]+";
            Regex regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(code, pattern))
            {
                SplitDelete(match.Value);
            }
        }
        //Разделяем на элементы запрос DELETE
        public void SplitDelete(string sqlQuery)
        {
            Console.WriteLine("sql запрос");
            Console.WriteLine(sqlQuery);
            string tableName = "";

            //убираем лишние проблемы
            string pattern = @"\s+";
            string target = " ";
            Regex regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"^(.)+(?ixn)DELETE[ ]+((FROM[ ])|())";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            //получаем имя таблицы
            pattern = @"(?ixn)^[ ]*[^ ]+";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                tableName = match.Value.Replace(" ", "");
            }
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"^[ ]*(?ixn)WHERE";
            regex = new Regex(pattern);
            string constrLine = "";
            if (regex.IsMatch(sqlQuery))
            {
                sqlQuery = regex.Replace(sqlQuery, target);
                constrLine = sqlQuery;
            }
            CreateDelete(tableName, constrLine);
        }

        void CreateDelete(string tableName, string constrLine)
        {
            Console.WriteLine("EF6");
            string EMBlock = "using (ApplicationContext db = new ApplicationContext())\n{\n\t";

            EMBlock += "db." + tableName + ".RemoveRange(db." + tableName;

            if (constrLine.Length > 0)
            {
                EMBlock += ".Where(x => ";
                string[] constrns = constrLine.Split(new[] { " AND ", " OR " }, StringSplitOptions.None);
                foreach (var i in constrns)
                {
                    string pattern = @"^[^=<>]+";
                    Regex regex = new Regex(pattern);
                    foreach (Match match in Regex.Matches(i, pattern))
                    {
                        string newColumn = "x." + match.Value.Replace(" ", "") + " ";
                        constrLine = constrLine.Replace(match.Value, newColumn);
                    }
                }

                constrLine = constrLine.Replace("=", "==").Replace("AND", "&&").Replace("OR", "||");
                EMBlock += constrLine + ")";
            }
            EMBlock += ");\n\tdb.SaveChanges();\n}";
            Console.WriteLine(EMBlock);
            Console.WriteLine("----------------------");
        }

        public void SearchSelect(string code)
        {
            string pattern = @"[^;]+(?ixn)""SELECT[^;]+";
            Regex regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(code, pattern))
            {
                SplitSelect(match.Value);
            }
        }

        public void SplitSelect(string sqlQuery)
        {
            Console.WriteLine("sql запрос");
            Console.WriteLine(sqlQuery);
            bool whereBool = false;
            bool orderByBool = false;
            if (sqlQuery.ToUpper().Contains(" WHERE "))
            {
                whereBool = true;
            }
            if (sqlQuery.ToUpper().Contains(" ORDER BY "))
            {
                orderByBool = true;
            }

            //убираем лишние проблемы
            string pattern = @"\s+";
            string target = " ";
            Regex regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            pattern = @"^(.)+(?ixn)SELECT[ ]*";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            //получаем название столбцов
            pattern = @"^.*(?ixn)FROM";
            string columnsLine = "";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                columnsLine = match.Value;
            }
            sqlQuery = regex.Replace(sqlQuery, target);
            pattern = @"(?ixn)FROM$";
            regex = new Regex(pattern);
            columnsLine = regex.Replace(columnsLine, target);
            //получаем название таблиц
            if(whereBool)
            {
                pattern = @"^.*(?ixn)((WHERE))";
            }
            else
            {
                if(orderByBool)
                {
                    pattern = @"^.*(?ixn)((ORDER[ ]BY))";
                }
                else
                {
                    pattern = @"^.*($)";
                }
            }
            string tablesLine = "";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                tablesLine = match.Value;
            }
            sqlQuery = regex.Replace(sqlQuery, target);
            pattern = @"(?ixn)((WHERE)|(ORDER[ ]BY))$";
            regex = new Regex(pattern);
            tablesLine = regex.Replace(tablesLine, target);
            //получаем условия
            string constrainsLine = "";
            if (whereBool)
            {
                if (orderByBool)
                {
                    pattern = @"^.*(?ixn)((ORDER[ ]BY))";
                }
                else
                {
                    pattern = @"^.*($)";
                }
                regex = new Regex(pattern);
                foreach (Match match in Regex.Matches(sqlQuery, pattern))
                {
                    constrainsLine = match.Value;
                }
                sqlQuery = regex.Replace(sqlQuery, target);
                pattern = @"(?ixn)ORDER[ ]BY$";
                regex = new Regex(pattern);
                constrainsLine = regex.Replace(constrainsLine, target);
            }
            string orderByLine = sqlQuery.Trim();
            CreateSelect(columnsLine, tablesLine, constrainsLine, orderByLine);
        }
        void CreateSelect(string columnsLines, string tablesLines, string constrainsLine, string orderByLine)
        {
            Console.WriteLine("EF6");
            string EMBlock = "using (ApplicationContext db = new ApplicationContext())\n{\n\tvar result = ";

            //колонки
            string columnCode = "select new\n\t{\n\t\t";
            if (columnsLines.Trim() == "*") 
            {
                columnCode += "Specify all columns included in the table\n}";
            }
            else{
                string[] columns = columnsLines.Split(',');
                Dictionary<string, string> columnsPsevdo = new Dictionary<string, string>();
                for (int i = 0; i < columns.Length; i++)
                {
                    string[] a = columns[i].ToLower().Replace("as", "").Replace("  ", " ").Trim().Split(' ');
                    if (a.Length == 2)
                    {
                        columnsPsevdo.Add(a[0], a[1]);
                    }
                    else
                    {
                        columnsPsevdo.Add(a[0], a[0]);
                    }
                }
                foreach (var i in columnsPsevdo)
                {
                    if (i.Key.Contains('.'))
                    {
                        columnCode += i.Value + " = " + i.Key + "\n\t\t";
                    }
                    else
                    {
                        columnCode += i.Value + " = t." + i.Key + "\n\t\t";
                    }
                }
                columnCode = columnCode.Trim('\t') + "\t}";
            }

            //условия
            string whereCode = "";
            if (constrainsLine.Length > 0)
            {
                whereCode += "where ";
                string[] constrns = constrainsLine.Split(new[] { " AND ", " OR " }, StringSplitOptions.None);
                foreach (var i in constrns)
                {
                    string pattern = @"^[^=<>]+";
                    Regex regex = new Regex(pattern);
                    foreach (Match match in Regex.Matches(i, pattern))
                    {
                        if (!match.Value.Contains(".")) 
                        {
                            string newColumn = "t." + match.Value.Replace(" ", "") + " ";
                            constrainsLine = constrainsLine.Replace(match.Value, newColumn);
                        }
                    }
                }

                constrainsLine = constrainsLine.Replace("=", "==").Replace("AND", "&&").Replace("OR", "||");
                whereCode += constrainsLine;
            }
            //сортировка
            string orderByCode = "";
            if(orderByLine.Length > 0)
            {
                orderByCode += "orderby ";
                string[] orderBys = orderByLine.Split(',');
                for(int i = 0; i < orderBys.Length; i++)
                {
                    if(i > 0)
                    {
                        orderByCode += ", ";
                    }
                    string[] a = orderBys[i].Trim().Split(' ');
                    if (!a[0].Contains("."))
                    {
                        orderByCode += "t.";
                    }
                    orderByCode += a[0];
                    if (a.Length > 1)
                    {
                        
                        if (a[1].ToUpper() == "DESC")
                        {
                            orderByCode += " descending";
                        }
                    }
                }
            }
            //таблицы
            string fromCode = "from ";
            string[] tableInfo = tablesLines.Split(new[] {" JOIN ", ",", " INNER JOIN " }, StringSplitOptions.None);
            for(int i = 0; i < tableInfo.Length; i++)
            {
                tableInfo[i] = tableInfo[i].Trim();
                string pattern = @"^[^ ]+";
                Regex regex = new Regex(pattern);
                string tableName = "";
                foreach (Match match in Regex.Matches(tableInfo[i], pattern))
                {
                     tableName = match.Value;
                }
                tableInfo[i] = regex.Replace(tableInfo[i], "").Trim();
                string psevd = "";
                if (tableInfo[i].ToUpper().Contains("ON "))
                {
                    pattern = @"^.*(?ixn)[ ]ON[ ]";
                    regex = new Regex(pattern);
                    foreach (Match match in Regex.Matches(tableInfo[i], pattern))
                    {
                        psevd = match.Value.ToLower().Replace("as ", "").Replace(" on ", "");
                    }
                    tableInfo[i] = regex.Replace(tableInfo[i], "").Trim();
                }
                else
                {
                    psevd = tableInfo[i];
                }
                if(psevd.Trim().Length == 0)
                {
                    psevd = tableName;
                }
                if (i == 0)
                {
                    fromCode += psevd + " in db." + tableName;
                }
                else
                {
                    fromCode += " join " + psevd + " in db." + tableName + " on " + tableInfo[i].Replace("=", "equals");
                }
            }
            EMBlock += fromCode + "\n\t" + whereCode + "\n\t" + orderByCode + "\n\t" + columnCode + ";\n}";
            Console.WriteLine(EMBlock);
            Console.WriteLine("----------------------");
        }
    }
}
