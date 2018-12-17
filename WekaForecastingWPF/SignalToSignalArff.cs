using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WekaForecastingWPF
{
    class SignalToSignalArff
    {// this calss will transform our signal data to an arff file according the % of data the user want to transform to an arff file
     // the file we will create or fill (if alrady created) will name signal.arff (we will use this file in the main program for prediction)
     // the arff file created at the project folder ..WekaForecasting\bin\Debug

        //properties to save the next data points after the points use for our prediction 
        public Tuple<double, double>[] next_data { get; set; }

        public SignalToSignalArff(string Path_to_signal, double data_precentage_split)
        {
            //array that will hold all of the current data points of a signal
            int NumberOfLines = System.IO.File.ReadAllLines(Path_to_signal).Count(s => s != null);
            Tuple<double, double>[] signal_data = new Tuple<double, double>[NumberOfLines];

            //define next_data array
            if (data_precentage_split < 1.0)
            {
                next_data = new Tuple<double, double>[Convert.ToInt32(NumberOfLines * (1 - data_precentage_split))];
            }

            //reading the lines from the signal file
            string[] signal_lines = System.IO.File.ReadAllLines(@Path_to_signal);


            int index = 0;

            //enter the data points from the file to signal_data array
            foreach (string line in signal_lines)
            {
                if (line != null)
                {
                    var split = line.Split();
                    signal_data[index] = new Tuple<double, double>(Convert.ToDouble(split[0]), Convert.ToDouble(split[1]));
                    index++;
                }
            }

            //write to arff
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(".\\Signal.arff"))
            {
                //write the first lines in the arff - relation and attributs 
                file.WriteLine("@RELATION Signal");
                file.WriteLine("@ATTRIBUTE time NUMERIC");
                file.WriteLine("@ATTRIBUTE frequency NUMERIC");
                file.WriteLine("");
                file.WriteLine("@DATA");
                //file.WriteLine(signal_data.Last().ToString());

                //split the data points according the % of data the user determine
                index = 0;
                for (int i = 0; i < signal_data.Length; i++)
                {
                    if (i < signal_data.Length * data_precentage_split)
                    {//enter the point to the arff
                        file.WriteLine(signal_data[i].Item1.ToString() + "," + signal_data[i].Item2.ToString());
                    }
                    else if (data_precentage_split < 1.0)
                    {//we are not using 100% of the data points and we need to enter the remaining points to next_data array
                        next_data[index] = signal_data[i];
                        index++;
                    }
                }


            }



        }

    }
}
