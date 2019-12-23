using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpExam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int END_INTERVAL = 1000;
        private Task Result { get; set; }
        private StringBuilder NumberString { get; set; } = new StringBuilder();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task WriteToFileAsync()
        {
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileName = $"{DateTime.Now.ToString("ddMMyyyy-hhmmss")}.txt";
            using (var outputFile = new StreamWriter(System.IO.Path.Combine(savePath, fileName)))
            {
                await outputFile.WriteAsync(NumberString.ToString());
            }

        }

        private async Task WriteToDatabaseAsync()
        {
            using (var context = await Task.Run(() => new Context()))
            {
                await context.Records.AddAsync(new Record { Content = NumberString.ToString() });
                await context.SaveChangesAsync();
            }
        }

        private async void StartCount(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            Parallel.For(0, END_INTERVAL, index => NumberString.Append($"Дата создания - {DateTime.Now}\n{index.ToString()} "));
            MessageBox.Show(NumberString.ToString());

            Result = Task.WhenAll(WriteToDatabaseAsync(), WriteToFileAsync());
            await Result;
            if (Result.Status == TaskStatus.RanToCompletion)
            {
                MessageBox.Show("Успешно записано в файл и в базу данных");
            }
            else if (Result.Status == TaskStatus.Faulted)
            {
                MessageBox.Show("Ошибка записи в файл или в базу данных");
            }
        }
    }
}
