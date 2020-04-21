using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace SprintData
{
    public class ProcessData
    {
        string startFile;
        string endFile;

        SprintDataCollection startRecs;

        SprintDataCollection endRecs;

        public ProcessData(string start, string end)
        {
            startFile = start;
            endFile = end;
        }

        public void Start()
        {
            if (LoadDataFromFile(startFile, out startRecs) &&
                LoadDataFromFile(endFile, out endRecs))
            {
                DisplayInformation();
            }
            else
            {
                Console.WriteLine("No data to process.");
            }
        }

        public bool LoadDataFromFile(string fileName, out SprintDataCollection recs)
        {
            recs = new SprintDataCollection();
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    var record = new SprintDataRecord();
                    record.issueID = fields[0];//.ToString();
                    record.state = fields[3];
                    record.points = Convert.ToDouble(fields[9]);
                    recs.Add(record);
                }
            }
            return true;
        }

        public void DisplayInformation()
        {
            Console.WriteLine("Stories at start: " + startRecs.Count());
            Console.WriteLine("Stories at end  : " + endRecs.Count());
        }
    }
}