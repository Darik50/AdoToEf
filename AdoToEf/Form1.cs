using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeAnalysis;
using System.IO;

namespace AdoToEf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox1.Text = openFileDialog1.FileName;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string typeFile = "";
            if (textBox1.Text.Contains(".sln"))
            {
                typeFile = "Solution";
            }
            else
            {
                if (textBox1.Text.Contains(".csproj"))
                {
                    typeFile = "Project";
                }
                else
                {
                    if (textBox1.Text.Contains(".cs"))
                    {
                        typeFile = "File";
                    }
                    else
                    {
                        textBox1.Text = "Выбран неверный формат файла";
                    }
                }
            }
            ChangeFile a = new ChangeFile(typeFile, textBox1.Text);
            Dictionary<MethodStruct, List<SqlToEfStruct>> arr = a.StartChange();
            if (comboBox1.Text == "Рекомендации по преобразованию кода в отдельном файле")
            {
                string text = "";
                foreach(var i in arr)
                {
                    text += "Путь к методу для преобразования кода:\n" + i.Key.documentPath + "\n";
                    foreach(var j in i.Value)
                    {
                        text += j.sqlQuery + "\n" + "Для преобразования данного SQL-запроса, необходимо использовать сдледующий код:\n" + j.EfBlock + "\n";
                    }
                }
                StreamWriter sw = new StreamWriter(@"C:\Users\Ильдар\Desktop\Отчет.txt");
                sw.WriteLine(text);
                sw.Close();
            }
            else
            {
            }
        }
    }
}
