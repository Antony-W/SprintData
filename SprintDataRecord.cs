using System;
using System.Collections.Generic;

namespace SprintData
{
    public class SprintDataRecord
    {
        public string issueID;

        public string title;

        public string state;

        public double points;

        public string[] tags;
    }

    public class SprintDataCollection : List<SprintDataRecord>
    {

    }
}