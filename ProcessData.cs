using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

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
            Console.WriteLine($"Parsing {fileName}");
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
                    record.tags = GetTags(fields[6]);
                    if (!record.tags.Contains("BAT"))
                    {
                        recs.Add(record);
                    }
                    else
                    {
                        Console.WriteLine($"Omitting story {record.issueID} because of BAT tag");
                    }
                }
            }
            Console.WriteLine();
            return true;
        }

        public void DisplayInformation()
        {
            Console.WriteLine("Non-BAT Stories at start: {0}, points {1}", startRecs.Count(), startRecs.Sum(p =>p.points));
            Console.WriteLine();

            Console.WriteLine("Non-BAT Stories at end : {0}, points {1}",
                    endRecs.Count(),
                    endRecs.Sum(p => p.points));

            Console.WriteLine();
            Console.WriteLine("Non-BAT added:");
            var addedCount = 0;
            var addedPoints = 0.0;
            foreach (var item in endRecs)
            {
                if (startRecs.Find(i => item.issueID == i.issueID) == null)
                {
                    addedCount++;
                    addedPoints += item.points;
                    Console.WriteLine($"   {item.issueID}");
                }
            }
            Console.WriteLine("Total {0}, points {1}", addedCount, addedPoints);

            Console.WriteLine();
            Console.WriteLine("Non-BAT removed:");
            var removed = 0;
            var removedPoints = 0.0;
            var atEnd = 0;
            foreach (var item in startRecs)
            {
                if (endRecs.Find(i => item.issueID == i.issueID) != null)
                {
                    atEnd++;
                }
                else
                {
                    removed++;
                    removedPoints += item.points;
                    Console.WriteLine($"   {item.issueID}");
                }
            }
            Console.WriteLine("Total {0}, points {1}", removed, removedPoints);

            Console.WriteLine();
            var closedStories = endRecs.FindAll(x => x.state == "Closed").ToList();
            Console.WriteLine("Non-BAT completed {0}, points {1}",
                    closedStories.Count(),
                    closedStories.Sum(p => p.points));

            if (endRecs.Count() != startRecs.Count() - removed + addedCount)
            {
                Console.WriteLine("*************  Data is incorrect ************");
            }
        }

        private string[] GetTags(string tags)
        {
            var tagList = new string[] { };
            if (tags.Length > 0)
            {
                tagList = tags.Split(';');
            }
            return tagList;
        }
    }
}