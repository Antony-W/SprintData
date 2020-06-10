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
            bool processingEndRecs = startRecs != null;
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
                    var record = new SprintDataRecord
                    {
                        issueID = fields[0],
                        state = fields[3],
                        points = Convert.ToDouble(fields[9]),
                        tags = GetTags(fields[6])
                    };
                    if (!record.tags.Contains("BAT"))
                    {
                        recs.Add(record);
                    }
                    else
                    {
                        Console.Write($"Omitting story {record.issueID} because of BAT tag");

                        // If a record is labelled BAT part way through a sprint, it will
                        // only get removed from the end list. It should also be removed
                        // from the start list.
                        if (processingEndRecs)
                        {
                            if (startRecs.RemoveAll(r => r.issueID == record.issueID) > 0)
                            {
                                Console.Write($" and removing unlabelled BAT entry from start list");
                            }
                        }
                        Console.WriteLine();
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

            // Unit test data
            var completed = endRecs.Count(r => r.tags.Contains("ALT-UnitTest-Completed"));
            var partial = endRecs.Count(r => r.tags.Contains("ALT-UnitTest-Partial"));
            var notRequired = endRecs.Count(r => r.tags.Contains("ALT-UnitTest-NotRequired"));
            var total = completed + partial + notRequired;

            double Percent(int v, int t) => Convert.ToDouble(v) / Convert.ToDouble(t) * 100;

            Console.WriteLine();
            Console.WriteLine($"Unit test data: Complete : {completed} ({Percent(completed, total):0.00}%)");
            Console.WriteLine($"Unit test data: Partial : {partial} ({Percent(partial, total):0.00}%)");
            Console.WriteLine($"Unit test data: Not Required : {notRequired} ({Percent(notRequired, total):0.00}%)");
        }

        private string[] GetTags(string tags)
        {
            var tagList = new string[] { };
            if (tags.Length > 0)
            {
                tagList = tags.Split(';');
                for (var i = 0; i < tagList.Length; i++)
                {
                    tagList[i] = tagList[i].Trim();
                }
            }
            return tagList;
        }
    }
}