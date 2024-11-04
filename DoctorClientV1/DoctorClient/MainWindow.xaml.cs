using System.ComponentModel;
using System.Text;
using System.Timers;
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
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace DoctorClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        private static ObservableCollection<string> bikeClients = new ObservableCollection<string>();
        private static ObservableCollection<Data> bikeData = new ObservableCollection<Data>();
        private static ObservableCollection<Dictionary<int, List<Data>>> History = new ObservableCollection<Dictionary<int, List<Data>>>();

        private Connection connection;

        private static string Message;
        private DispatcherTimer updateTimer;

        private ObservableCollection<Data> sessions;
        private int? lastSelectedDataID = null;



        public MainWindow(Connection connection)
        {
            InitializeComponent();
            this.connection = connection;
            ActiveSessions.ItemsSource = bikeData;
            bikeClients.CollectionChanged += BikeClients_CollectionChanged;
            bikeData.CollectionChanged += BikeData_CollectionChanged;
            History.CollectionChanged += History_CollectionChanged;

        }


        private void BikeClients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateBikeClientList();
        }
        
        private void BikeData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateBikeDataList();
        }
        
        private void History_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            HistoryListview.Items.Clear();

            foreach (var dictionary in History)
            {
                foreach (var sessionId in dictionary.Keys)
                {
                    HistoryListview.Items.Add(sessionId);
                }
            }

        }

        private void HistoryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (HistoryListview.SelectedItem is int selectedSessionId)
            {
                DataListView.Items.Clear();

                foreach (var dictionary in History)
                {
                    if (dictionary.TryGetValue(selectedSessionId, out List<Data> dataList))
                    {
                        foreach (var dataItem in dataList)
                        {
                            DataListView.Items.Add(dataItem);
                        }
                        break; 
                    }
                }
            }
        }


        public void UpdateLog(string message)
        {
            logTextBox.AppendText(message + Environment.NewLine);
            logTextBox.ScrollToEnd(); 
        }

        public static void UpdateLogOut(string message)
        {
            Message = message;

        }



        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveSessions.SelectedItem is Data selectedData)
            {
                lastSelectedDataID = selectedData.ID;
                resistanceTextBox.Text = selectedData.Resistance.ToString();
            }
            else
            {
                lastSelectedDataID = null;
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

            //string selectedItem = ConnectedClients.SelectedItem.ToString();
            int selectedItem = int.Parse(ConnectedClients.SelectedItem.ToString().Split(':')[0]);
            connection.Write("Start " + selectedItem);
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Write("STOP " + lastSelectedDataID);

        }

        public  void UpdateBikeClientList()
        {
            ConnectedClients.ItemsSource = null; 
            ConnectedClients.ItemsSource = bikeClients;
        }
        public  void UpdateBikeDataList()
        {
            UpdateLog(bikeData.Count().ToString());
        }

        public static void UpdateBikeClient(List<string> clients)
        {
            bikeClients.Clear();
            foreach (var client in clients)
            {
                bikeClients.Add(client);
            }
        }

        public static void UpdateBikeData(Data newData)
        {
            var existingData = bikeData.FirstOrDefault(d => d.ID == newData.ID);

            if (existingData != null)
            {
                existingData.Speed = newData.Speed;
                existingData.Distance = newData.Distance;
                existingData.Time = newData.Time;
                existingData.HeartBeat = newData.HeartBeat;
                existingData.Resistance = newData.Resistance;
            }
            else
            {
                bikeData.Add(newData);
            }
        }
        
        public static void UpdateHistory(Dictionary<int, List<Data>> historyData)
        {
            History.Clear();  
            History.Add(historyData);
        }

       



        private void ConnectedClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void RefreshData()
        {
            //List<Data> newData = connection.readData();

            //sessions.Clear();

            //foreach (var item in newData)
            //{
            //    sessions.Add(item);
            //}
        }

        private void SendResistanceButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Write("RESISTANCE|" + lastSelectedDataID + "|" + resistanceTextBox.Text);
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Write("MESSAGE|"+lastSelectedDataID+"|"+MessageTextBox.Text);

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            connection.Write("GETHISTORY");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            connection.Write("SENDMESSAGETOALL|"+ MessageTextBox.Text);


        }

        private void Button_Click_3(object sender, RoutedEventArgs e)

        {
            connection.Write("STOP " + lastSelectedDataID);
            connection.Write("RESISTANCE|" + lastSelectedDataID + "|" + 200);


        }
    }
}

public class Data : INotifyPropertyChanged
{
    public Data(int id, double speed, double distance, int time, int heartBeat, int resistance, string naam)
    {
        this.ID = id;
        this.Speed = speed;
        this.Distance = distance;
        this.Time = time;
        this.HeartBeat = heartBeat;
        this.Resistance = resistance;
        this.Naam = naam;
    }

    public int ID { get; set; }

    private double speed;
    public double Speed
    {
        get => speed;
        set
        {
            speed = value;
            OnPropertyChanged(nameof(Speed));
        }
    }

    private double distance;
    public double Distance
    {
        get => distance;
        set
        {
            distance = value;
            OnPropertyChanged(nameof(Distance));
        }
    }

    private int time;
    public int Time
    {
        get => time;
        set
        {
            time = value;
            OnPropertyChanged(nameof(Time));
        }
    }

    private int heartBeat;
    public int HeartBeat
    {
        get => heartBeat;
        set
        {
            heartBeat = value;
            OnPropertyChanged(nameof(HeartBeat));
        }
    }

    private int resistance;
    public int Resistance
    {
        get => resistance;
        set
        {
            resistance = value;
            OnPropertyChanged(nameof(Resistance));
        }
    }

    public string Naam { get; } 

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"ID: {ID}, Naam: {Naam}, Speed: {Speed}, Distance: {Distance}, Time: {Time}, HeartBeat: {HeartBeat}, Resistance: {Resistance}";
    }
}




