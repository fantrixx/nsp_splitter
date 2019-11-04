using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;

namespace SplitNSP
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string nspFilePath { get; set; }
        public string outputPath { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilePathSelect_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select NSP File";
            openFileDialog1.Multiselect = false;
            openFileDialog1.DefaultExt = "nsp";
            openFileDialog1.InitialDirectory = @"C:\";

            if (openFileDialog1.ShowDialog() == true)
            {
                nspFilePath = openFileDialog1.FileName;
                if (nspFilePath != null) nspFileName_TextBox.Text = nspFilePath;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(nspFilePath);
            var newFolder = fileInfo.DirectoryName +"\\" + fileInfo.Name + "_split";

            if (!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
                SplitFile(nspFilePath, 1073741824, newFolder);
            }

            // set archive bit
            if(Directory.Exists(newFolder))
            {
                File.SetAttributes(newFolder, FileAttributes.Archive);
            }
        }

        public static void SplitFile(string inputFile, uint chunkSize, string path)
        {
            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            FileInfo fileInfo = new FileInfo(inputFile);

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;
                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + index))
                    {
                        int remaining = unchecked((int)chunkSize);
                        int bytesRead;

                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0, Math.Min(remaining, unchecked((int)BUFFER_SIZE)))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            remaining -= bytesRead;
                        }
                    }
                    index++;
                    //Thread.Sleep(500); // experimental; perhaps try it
                }
            }
        }
    }
}
