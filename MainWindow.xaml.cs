using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SudokuGame SudokuGame;
        string[,] question = new string[9, 9];
        string[,] answer = new string[9, 9];
        int K, k, mistake, hint;
        TextBox lastEmptyCell;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();

        static string directory = String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "riwayat.txt");
        string[] data = System.IO.File.ReadAllLines(directory);

        public MainWindow()
        {
            InitializeComponent();
            isEnabledAll(false);
            k = 0;

            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dt.Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
            if(sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                time.Text = string.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds);
            }
        }

        private void isEnabledAll(bool b)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    TextBox tb = (TextBox)this.FindName(string.Format("Cell{0}{1}", i, j));
                    tb.IsEnabled = b;
                }
            }
        }

        private void check(TextBox tb, int i, int j)
        {
            if (tb.Text.Equals(answer[i, j]))
            {
                tb.Foreground = Brushes.Green;
                tb.IsEnabled = false;
                SudokuGame.setFlag(i, j, true);
                k--;
                if (k == 0) end(true);
            }
            else
            {
                mistake++;
                tb.Foreground = Brushes.Red;
            }
        }

        private void end(bool win)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                dt.Stop();
                sw.Reset();
            }
            string waktu = DateTime.Now.ToString();
            string str = string.Format("{0}\n" +
                                       "Level: {1}({2})\n" +
                                       "Status: {3}\n" +
                                       "Banyak Salah: {4}\n" +
                                       "Banyak Bantuan: {5}\n" +
                                       "Sisa Kosong: {6}\n" +
                                       "Lama Main: {7}",
                waktu,level.Text, K, win?"Menang":"Kalah", mistake, hint, k, time.Text);
            MessageBox.Show(str);

            using (StreamWriter sw = File.AppendText(directory))
            {
                sw.WriteLine(string.Format("{0} {1}({2}) {3} {4} {5} {6} {7}",
                    waktu, level.Text, K, win ? "Menang" : "Kalah", mistake, hint, k, time.Text));
            }

            time.Text = "00:00:00";
            K = 0;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            lastEmptyCell = e.Source as TextBox;
        }

        private void MulaiBaru_Click(object sender, RoutedEventArgs e)
        {
            if(k!=0)
            {
                MessageBoxResult yakin = System.Windows.MessageBox.Show("Permainan belum berakhir, Anda ingin mulai baru?", "Mulai Baru", System.Windows.MessageBoxButton.YesNo);
                if (yakin == MessageBoxResult.No) return;
            }

            if (sw.IsRunning)
            {
                sw.Stop();
                dt.Stop();
                sw.Reset();
                time.Text = "00:00:00";
            }

            Random random = new Random();
            if (level.Text.Equals("Demo")) K = 5;
            else if (level.Text.Equals("Normal")) K = random.Next(20, 41);
            else K = 0;

            SudokuGame = new SudokuGame(K);
            k = K;
            hint = 0;
            mistake = 0;
            SudokuGame.generate();
            isEnabledAll(true);

            for (int i = 0; i < 9; ++i)
            {
                for(int j = 0; j < 9; ++j)
                {
                    answer[i, j] = string.Format("{0}", SudokuGame.solution[i, j]);
                    question[i, j] = string.Format("{0}", SudokuGame.arr[i, j]);
                    if (question[i, j].Equals("0")) question[i, j] = "";
                    TextBox tb = (TextBox)this.FindName(string.Format("Cell{0}{1}", i, j));
                    tb.Text = question[i, j];
                    tb.Foreground = Brushes.Black;
                    if(question[i, j].Equals(answer[i, j]))
                    {
                        tb.IsEnabled = false;
                    }
                }
            }

            sw.Start();
            dt.Start();
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Input.Key key = e.Key;
            if(key >= Key.D1 && key <= Key.D9)
            {
                TextBox tb = sender as TextBox;
                tb.Text = "" + (char)('0'+(int)(key - Key.D0));

                string name = tb.Name;
                int i = name[name.Length - 2] - '0';
                int j = name[name.Length - 1] - '0';
                check(tb, i, j);
            }
            else if(key == Key.Back || key == Key.Delete)
            {
                TextBox tb = sender as TextBox;
                tb.Text = "";
            }
            e.Handled = true;
        }

        private void Riwayat_Click(object sender, RoutedEventArgs e)
        {
            if (k != 0)
            {
                MessageBoxResult yakin = System.Windows.MessageBox.Show("Permainan belum berakhir, Anda ingin melihat riwayat?", "Riwayat", System.Windows.MessageBoxButton.YesNo);
                if (yakin == MessageBoxResult.No) return;
            }
            Riwayat riwayat = new Riwayat();
            riwayat.Show();
            this.Close();
        }

        private void Bantuan_Click(object sender, RoutedEventArgs e)
        {
            if (lastEmptyCell == null || lastEmptyCell.Foreground == Brushes.Green ||
                lastEmptyCell.Foreground == Brushes.Purple)
            {
                MessageBox.Show("Silahkan pilih satu kotak");
                return;
            }
            string name = lastEmptyCell.Name;
            int i = name[name.Length - 2] - '0';
            int j = name[name.Length - 1] - '0';
            lastEmptyCell.Text = answer[i, j];
            lastEmptyCell.Foreground = Brushes.Purple;
            lastEmptyCell.IsEnabled = false;
            hint++;
            k--;
            if (k == 0) end(true);
        }

        private void Menyerah_Click(object sender, RoutedEventArgs e)
        {
            if (K == 0) return;
            if (k != 0)
            {
                MessageBoxResult yakin = System.Windows.MessageBox.Show("Anda yakin?", "Menyerah", System.Windows.MessageBoxButton.YesNo);
                if (yakin == MessageBoxResult.No) return;
            }
            for (int index = 0; index < SudokuGame.emptyCell.Count; ++index)
            {
                int i = SudokuGame.emptyCell[index][0];
                int j = SudokuGame.emptyCell[index][1];

                TextBox tb = (TextBox)this.FindName(string.Format("Cell{0}{1}", i, j));
                if (!tb.Text.Equals(answer[i, j]))
                {
                    tb.Text = answer[i, j];
                    tb.Foreground = Brushes.Purple;
                    tb.IsEnabled = false;
                }
            }
            end(false);
        }

        private void Keluar_Click(object sender, RoutedEventArgs e)
        { 
            MessageBoxResult yakin = System.Windows.MessageBox.Show("Anda yakin?", "Keluar", System.Windows.MessageBoxButton.YesNo);
            if(yakin == MessageBoxResult.Yes)  this.Close();
        }
    }
}
