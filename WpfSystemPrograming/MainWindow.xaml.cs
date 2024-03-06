using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfSystemPrograming
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartBtnClick(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            await ReadFilesAsync(cancellationTokenSource.Token);
        }

        private void StopBtnClick(object sender, RoutedEventArgs e)
        {
           
            cancellationTokenSource?.Cancel();

        }

        private async Task ReadFilesAsync(CancellationToken cancellationToken)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

            string fileAPath = Path.Combine(projectDirectory, "TxtFiles", "AAAAA.txt");
            string fileBPath = Path.Combine(projectDirectory, "TxtFiles", "BBBBB.txt");
            string fileCPath = Path.Combine(projectDirectory, "TxtFiles", "CCCCC.txt");

           
            List<Task> tasks = new List<Task>
            {
                ReadFileAsync(fileAPath, listBox1, cancellationToken),
                ReadFileAsync(fileBPath, listBox2, cancellationToken),
                ReadFileAsync(fileCPath, listBox3, cancellationToken)
            };

            await Task.WhenAll(tasks);
        }

        private Task ReadFileAsync(string filePath, ListBox listBox, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            listBox.Items.Add(line);
                        });
                        Thread.Sleep(500);
                    }
                }
            }, cancellationToken);
        }
    }
}
