using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    class SearchSQL
    {
        public void SearchInto(string code)
        {
            string pattern = @"[^;]+(?ixn)INSERT[ ]+INTO[^;]+";
            Regex regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(code, pattern)) 
            {
                SplitInto(match.Value);
            }
        }

        public void SplitInto(string sqlQuery)
        {
            string tableName = "";
            string[] columns = new string[1];
            string[] values = new string[1];

            Console.WriteLine(sqlQuery);

            string pattern = @"\s+";
            string target = " ";
            Regex regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);
            Console.WriteLine(sqlQuery);

            pattern = @"^(.)+(?ixn)INSERT[ ]INTO[ ]";
            target = " ";
            regex = new Regex(pattern);
            sqlQuery = regex.Replace(sqlQuery, target);

            Console.WriteLine("---table name---");           
            pattern = @"(?ixn)^[ ]*[^ (]+([ ]|[(])";
            regex = new Regex(pattern);
            foreach (Match match in Regex.Matches(sqlQuery, pattern))
            {
                tableName = match.Value.Replace(" ", "").Replace("(", "");
            }
            sqlQuery = regex.Replace(sqlQuery, target);
            Console.WriteLine(tableName);

            pattern = @"(?ixn)^[ ]*VALUES[ ]*[(]";
            regex = new Regex(pattern);
            if (!regex.IsMatch(sqlQuery))
            {
                Console.WriteLine("---columns name---");
                pattern = @"^[ ]*[^)]+[)]";
                regex = new Regex(pattern);
                foreach (Match match in Regex.Matches(sqlQuery, pattern))
                {
                    columns = match.Value.Replace(" ", "").Replace(")", "").Replace("(", "").Split(',');
                }
                sqlQuery = regex.Replace(sqlQuery, target);
                foreach (var column in columns)
                {
                    Console.WriteLine(column);
                }
            }
            Console.WriteLine("---values---");
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
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
