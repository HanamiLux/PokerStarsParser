using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows;
using System;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading;

namespace pokerTestParser
{
    public partial class MainWindow : Window
    {
        string _directoryPath = string.Empty;
        string _path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/summay_results.json";
        Thread _secondThread;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Border_Drop(object sender, System.Windows.DragEventArgs e)
        {
            archiveListView.Items.Clear();
            
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)) // Getting directory path
            {
                _directoryPath = Path.GetFullPath(((string[])e.Data.GetData(System.Windows.DataFormats.FileDrop))[0]);
            }

            FileInfo[] files = new DirectoryInfo(_directoryPath).GetFiles("*.txt");
            
            ListViewFill(files); // Getting files to ListView

        }


        private void ListViewFill(FileInfo[] files)
        {
            foreach (FileInfo file in files)
            {
                archiveListView.Items.Add(file.Name);
            }
        }


        private void ReadPokerStarsHistoryFiles(FileInfo[] files)
        {
            List<PokerInfoHand> handsList = new List<PokerInfoHand>();
            int counter = 0;
            float percent = 0f;
            foreach (FileInfo file in files) //Checking each file
            {

                ++counter;
                percent = (100 * counter) / (float)files.Length;
                Dispatcher.Invoke(() => percentsLabel.Content = percent + "%");

                List<string> lines = File.ReadAllLines(file.FullName).ToList(); // Lines of file
                foreach (string line in lines) //Checking each line of file
                {
                    if (line.Contains("PokerStars Game")) // Parsing each hand
                    {
                        
                        List<Player> playersInHand = StartParsePlayersHand(lines, line);
                        handsList.Add(new PokerInfoHand(line, playersInHand));
                    }
                    
                        
                }
                
            }
            SerializeData(_path, new SummaryPokerInfo(handsList));
            System.Windows.MessageBox.Show("Serialized! Check your new json file /(^_^)\\");
            
            Dispatcher.Invoke(() => { 
                exeButton.IsEnabled = true;
                browseButton.IsEnabled = true;
                percentsLabel.Content = "";
            });
            

        }


        private List<Player> StartParsePlayersHand(List<string> lines, string handName)
        {
            bool isStarted = false;
            List<Player> players = new List<Player>();
            foreach (string line in lines)
            {
                if (line.StartsWith("PokerStars Game") && line != handName && players.Count != 0)
                    return players;

                if (line.StartsWith("PokerStars Game") && line != handName) isStarted = false;


                if (line == handName) isStarted = true;

                if(isStarted)
                {
                    if (line.EndsWith("in chips) ")) // Checking players
                    {
                        string[] playerLine = line.Split(' ');
                        string nickname = string.Empty;
                        foreach (string substring in playerLine)
                        { // Getting player nickname
                            if (!substring.StartsWith("($") && !substring.EndsWith("chips)") && substring != playerLine[0] && substring != playerLine[1])
                                nickname = nickname + substring;
                        }
                        nickname = nickname.Remove(nickname.Length - 2);
                        decimal startBalance = decimal.Parse(playerLine.ToList().Where(str => str.StartsWith("($")).ToList()[0].Substring(2), CultureInfo.InvariantCulture);
                        players.Add(new Player(nickname, startBalance, 0));
                    }
                    else if (line.Contains("collected") && line.StartsWith("Seat"))
                    {
                        decimal playerPot = decimal.Parse(line.Split('(').Last().Trim('$', ')'), CultureInfo.InvariantCulture);
                        string nickname = line.Split(' ')[2];
                        int section = 3;
                        while (!line.Split(' ')[section].StartsWith('(') && line.Split(' ')[section] != "folded" && line.Split(' ')[section] != "collected")
                        {
                            nickname += line.Split(' ')[section];
                            ++section;
                        }
                        players.Find(p => p.Name == nickname).SummaryPlayerPot = playerPot;
                    }
                }
                    
            }
            return players;
        }

        private void SerializeData(string path, object _object)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(_object));
        }

        private void exeButton_Click(object sender, RoutedEventArgs e)
        {
            exeButton.IsEnabled = false;
            browseButton.IsEnabled = false;
            FileInfo[] files = new DirectoryInfo(_directoryPath).GetFiles("*.txt");
            _secondThread = new Thread(() => ReadPokerStarsHistoryFiles(files)); //Getting info from files in second thread
            _secondThread.Start();

        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK )
            {
                _directoryPath = dlg.SelectedPath;
                try
                {
                    FileInfo[] files = new DirectoryInfo(_directoryPath).GetFiles("*.txt");
                    ListViewFill(files); // Getting files to ListView
                }
                catch
                {
                    System.Windows.MessageBox.Show("Please select a directory!");
                }
            }
            
        }
    }
}
