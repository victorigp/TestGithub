using System.IO.Compression;

namespace TestGithub
{
    public partial class MainPage : ContentPage
    {
        private int _count;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            _count++;

            CounterBtn.Text = _count == 1
                ? $"Clicked {_count} time"
                : $"Clicked {_count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        /// <summary>
        /// Método de prueba para probar CodeQL en Github
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="targetDirectory"></param>
        public static void ExtractZip(string archivePath, string targetDirectory)
        {
            using ZipArchive archive = ZipFile.OpenRead(archivePath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destinationPath = Path.Combine(targetDirectory, entry.FullName);
                entry.ExtractToFile(destinationPath);
            }
        }
    }
}
