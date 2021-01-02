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
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for Riwayat.xaml
    /// </summary>
    public partial class Riwayat : Window
    {
        static string directory = String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "riwayat.txt");
        string[] data = System.IO.File.ReadAllLines(directory);
        //Tanggal Jam Level Status Banyak_Salah Banyak_Bantuan Banyak_Kosong Lama_Main
        
        public Riwayat()
        {
            InitializeComponent();
            List<Score> list = new List<Score>();
            int c = 5;
            for (int i=data.Length-1; i >= 0 && c > 0; --i)
            {
                string[] b = data[i].Split(' ');
                Score baru = new Score() {
                    Tanggal = b[0],
                    Waktu = b[1],
                    Level = b[2],
                    Status = b[3],
                    Salah = b[4],
                    Bantuan = b[5],
                    Kosong = b[6],
                    Durasi = b[7]
                };
                c--;
                list.Add(baru);
            }
            table.ItemsSource = list;
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Keluar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult yakin = System.Windows.MessageBox.Show("Anda yakin?", "Keluar", System.Windows.MessageBoxButton.YesNo);
            if (yakin == MessageBoxResult.Yes) this.Close();
        }
    }

    public class Score
    {
        public string Tanggal { get; set; }
        public string Waktu { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public string Salah { get; set; }
        public string Bantuan { get; set; }
        public string Kosong { get; set; }
        public string Durasi { get; set; }
    }
}
