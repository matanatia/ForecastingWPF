using System;
using System.Collections.Generic;
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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using weka.classifiers.functions;
using weka.classifiers.timeseries;
using weka.core;

namespace WekaForecastingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //seting the series objects in our chart
        public ChartValues<ObservablePoint> Current_Data { get; set; }
        public ChartValues<ObservablePoint> Next_Data { get; set; }
        public ChartValues<ObservablePoint> Predicted_Data { get; set; }
        public ChartValues<ObservablePoint> Negetive_Predicted_Data { get; set; }

        //seting the listBox_items
        public List<string> listBox_items { get; set; }

        //we will forecast using 90% of the current data and check the forecasting we made with the last 10% of the current data
        public double data_precentage_split = 0.9;

        //prop that contains the next data points we didnt use in the prediction. 
        public Tuple<double, double>[] next_data { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //fill the series in our chart with empty ChartValues
            Current_Data = new ChartValues<ObservablePoint>();
            Next_Data = new ChartValues<ObservablePoint>();
            Predicted_Data = new ChartValues<ObservablePoint>();
            Negetive_Predicted_Data = new ChartValues<ObservablePoint>();
            DataContext = this;

            fill_list_box();

        }

        private void fill_list_box()
        {// fill the list box with the hashtags name

            listBox_items = new List<string>();

            //take the hashtags files from their derectory --> or from the object in the main twist program that contains the hashtags
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo("D:\\Twist_DB\\hashtag_signal");
            var files = dir.GetFiles("*.txt");

            //fill _items with the hashtags files name
            foreach (var file in files)
            {
                listBox_items.Add(file.Name.Replace(".txt", ""));
            }

            //fill the list box with the names of the hashtags
            listBox.ItemsSource = listBox_items;
        }

        private bool check_user_input( int option)
        {// checks if the user input is currect if not popup window with the right message
            int num = -1;

            if ((option==1 || option==2) && listBox.SelectedItem == null)
            {//hashtag was not selected from the hashtag list and the user still click the current data btn or forecast btn
                PopupMassege.Text = "Choose an hashtag from the list";
                Popup.IsOpen = true;
                return false;
            }
            else if (option == 2 && next_data==null)
            {//the user push the forecast btn befor the current data btn - the next data to calculte the forecast wasn't created
                PopupMassege.Text = "Push Current Data first";
                Popup.IsOpen = true;
                return false;
            }
            else if (option == 2 && (!int.TryParse(TextBox.Text,out num)||Convert.ToInt32(TextBox.Text.ToString()) < 1 || Convert.ToInt32(TextBox.Text.ToString()) > 15) )
            {//the user didn't enter the number of steps to forecast in the right range or didn't enter a number at all
             //and the user still forecast btn
                PopupMassege.Text = "Enter forecast steps between 1-15";
                Popup.IsOpen = true;
                return false;
            }
            else if (option == 2 && next_data.Length < 1)
            {//the user choose hashtg with not enough data to use for the forcasting
                PopupMassege.Text = "Not enough data, choose new hashtag";
                Popup.IsOpen = true;
                return false;
            }

            Popup.IsOpen = false;
            return true;
        }

        private void Current_Data_btn_Click(object sender, RoutedEventArgs e)
        {//show the user the current data we will use for forcasting from the hashtag file the user choose 

            if (check_user_input(1))
            {
                Popup.IsOpen = false;

                //clear the charts if it has points alrady
                Current_Data.Clear();
                Next_Data.Clear();
                Predicted_Data.Clear();
                Negetive_Predicted_Data.Clear();

                //create the arff data file from 90% of the current data from the hashtag that the user choose
                //and save in arff.next_data an array with the next data point (the last 10% of the current data)
                SignalToSignalArff arff = new SignalToSignalArff("D:\\Twist_DB\\hashtag_signal\\" + listBox.SelectedItem.ToString() + ".txt", data_precentage_split);

                //fill the next data points to the next_data prop
                if (arff.next_data != null)
                {
                    next_data = new Tuple<double, double>[arff.next_data.Length];
                    next_data = arff.next_data;

                }

                //fixing chart prespective 
                Next_Data.Add(new ObservablePoint(0, 0));
                Predicted_Data.Add(new ObservablePoint(0, 0));
                Negetive_Predicted_Data.Add(new ObservablePoint(0, 0));

                // path to the signal data - found in the project folder ..WekaForecasting\bin\Debug
                String pathToData = ".\\Signal.arff";

                // load the signal data
                Instances data = new Instances(new java.io.BufferedReader(new java.io.FileReader(pathToData)));

                //fill  Current_Data series with the data from the hashtag file the user choose
                for (int i = 0; i < data.numInstances(); i++)
                {
                    Current_Data.Add(new ObservablePoint(data.instance(i).value(0), data.instance(i).value(1)));
                }
            }
        }

        private void fill_next_data()
        {//append the next data points to the chart
            for (int i = 0; i < next_data.Length; i++)
            {
                if (next_data[i] != null)
                {
                    Next_Data.Add(new ObservablePoint(next_data[i].Item1, next_data[i].Item2));
                }
            }
        }

        //Create a Delegate that matches 
        //the Signature of the ProgressBar's SetValue method
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private int returnBestMaxLagFit(WekaForecaster forecaster, Instances data)
        {//return max lag that fits the best to the prediction 

            //calculate the best fit MaxLag for our predection
            int bestFit = 1;
            double sumOfDeferance = 100;

            //Configure the ProgressBar
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 100;
            ProgressBar.Value = 0;

            //Stores the value of the ProgressBar
            double value = 0;

            //Create a new instance of our ProgressBar Delegate that points
            // to the ProgressBar's SetValue method.
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar.SetValue);


            //calculate the difference between the prediction points to the actual next data if the next culclation is smaller
            //then befor replace the bestFit value with i;
            for (int i = 1; i <= 24; i++)
            {
                if (ProgressBar.Value != ProgressBar.Maximum)
                {
                    value += 4.16;

                    /*Update the Value of the ProgressBar:
                        1) Pass the "updatePbDelegate" delegate
                           that points to the ProgressBar1.SetValue method
                        2) Set the DispatcherPriority to "Background"
                        3) Pass an Object() Array containing the property
                           to update (ProgressBar.ValueProperty) and the new value */
                    Dispatcher.Invoke(updatePbDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { ProgressBar.ValueProperty, value });
                }
                double sum = 0;

                //define the max lag
                forecaster.getTSLagMaker().setMaxLag(i);

                // build the model
                forecaster.buildForecaster(data);

                // prime the forecaster with enough recent historical data
                forecaster.primeForecaster(data);

                //if we dont have at list 10 points in next data we will use all the next data points else we use only 10 
                int forecastUnits = 10;
                if (next_data.Length < 10) { forecastUnits = next_data.Length; }


                // forecast for the number of forecastUnits we set beyond the end of the training data
                var mylist = forecaster.forecast(forecastUnits);

                //get the forcasted data and place it in the PredectedList
                for (int j = 3; j < forecastUnits; j++)
                {
                    String predict = mylist.get(j).ToString().Split(' ')[2];
                    sum = sum + Math.Abs(Convert.ToDouble(predict) - next_data[j-2].Item2);
                }

                //calculate Standard Error
                sum = sum / forecastUnits;

                if (sum < sumOfDeferance)
                {//save the smallest Standard Error
                    sumOfDeferance = sum;
                    bestFit = i;
                }

            }

            //show Standard Error in the maim window
            Standard_Error.Content = String.Format("Standard Error: {0:0.00000}", sumOfDeferance);
            return bestFit;
        }

        private void Forecast_Calc_Fill()
        {//using WekaForcaster to culclate the predicted data for our current signal data.
         //and show it in a chart for the user
            try
            {
                // path to the signal data - found in the project folder ..WekaForecasting\bin\Debug
                String pathToData = ".\\Signal.arff";

                // load the signal data
                Instances data = new Instances(new java.io.BufferedReader(new java.io.FileReader(pathToData)));

                // new forecaster
                WekaForecaster forecaster = new WekaForecaster();

                // set the target we want to forecast - the attribute in our arff file.
                forecaster.setFieldsToForecast("frequency");

                // default underlying classifier is SMOreg (SVM) - we'll use
                // MultilayerPerceptron for regression instead
                forecaster.setBaseForecaster(new MultilayerPerceptron());

                // set the attribute in our arff file that will be our time stamp.
                forecaster.getTSLagMaker().setTimeStampField("time");

                // set the lag creation to the data
                forecaster.getTSLagMaker().setMinLag(1);
                forecaster.getTSLagMaker().setMaxLag(returnBestMaxLagFit(forecaster, data));


                // build the model
                forecaster.buildForecaster(data);

                // prime the forecaster with enough recent historical data
                forecaster.primeForecaster(data);

                //xFactor will help us to calculate the forecast data after the last data piont in the next_data
                int xFactor = next_data.Length; 

                // forecast for the number of forecastUnits we set beyond the end of the training data
                int forecastUnits = Convert.ToInt32(TextBox.Text) + xFactor;
                double[] PredectedList = new double[forecastUnits];
                var mylist = forecaster.forecast(forecastUnits);

                //get the forcasted data and place it in the PredectedList
                for (int i = 0; i < forecastUnits; i++)
                {
                    String predict = mylist.get(i).ToString().Split(' ')[2];
                    PredectedList[i] = Convert.ToDouble(predict);
                }

                //lag represent our fixed lag between the time stamp of the predictions
                double lag = forecaster.getTSLagMaker().getDeltaTime();

                //DeltaTime will represent our changing lag between each time stamp at each prediction
                double DeltaTime = 0;

                //getting the last time stamp in our last point in the current data
                double lastValidTime = data.lastInstance().value(0);

                //clear Predicted_Data if alredy have data inside
                Predicted_Data.Clear();
                Negetive_Predicted_Data.Clear();

                // output the predictions with their time stamp in the chart.
                for (int i = 0; i < forecastUnits; i++)
                {
                    if (i < next_data.Length-1)
                    {//we still in the next data points and not in the predected points the user want to predict
                        Predicted_Data.Add(new ObservablePoint(next_data[i].Item1, PredectedList[i]));

                        //checks if the predected point is negetive if is negetive add the the black points series
                        if (PredectedList[i] < 0)
                        {
                            Negetive_Predicted_Data.Add(new ObservablePoint(next_data[i].Item1, PredectedList[i]));
                        }

                        //get the x from the last point in our next data to be the starting point in the predecred data
                        lastValidTime = next_data[i].Item1;
                    }
                    else 
                    {//we are in the range of the predected points the user want to predict

                        //advance the current time to correspond to the forecasted values 
                        DeltaTime = DeltaTime + lag;

                        //append the predicted data to the chart
                        Predicted_Data.Add(new ObservablePoint(lastValidTime + DeltaTime, PredectedList[i]));

                        //checks if the predected point is negetive if is negetive add the the black points series
                        if (PredectedList[i] < 0)
                        {
                            Negetive_Predicted_Data.Add(new ObservablePoint(lastValidTime + DeltaTime, PredectedList[i]));
                        }

                    }

                }

                //clear progressBar in the end of the progress
                ProgressBar.Value = 0;

                //checks if their wasn't negetive predected point and add the (0,0) point to the black points series
                //for preserving the chart prespective
                if (Negetive_Predicted_Data.Count == 0)
                {
                    Negetive_Predicted_Data.Add(new ObservablePoint(0, 0));
                }

            }
            catch (Exception ex) { ex.ToString(); }
        }

        private void Forecast_Click(object sender, RoutedEventArgs e)
        {//calculate the predected data and show to the user

            if (check_user_input(2))
            {
                //clear Next_Data if alredy have data inside
                Next_Data.Clear();

                //fill next data to the chart
                fill_next_data();

                //fill predicted data to the chart
                Forecast_Calc_Fill();
            }
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {//close the popup message
            Popup.IsOpen = false;
        }


    }
}
