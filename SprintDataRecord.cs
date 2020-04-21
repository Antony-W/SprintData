using System;
using System.Collections.Generic;

namespace SprintData
{
    public class SprintDataRecord
    {
        public string issueID;

        public string state;

        public double points;
    }

    public class SprintDataCollection : List<SprintDataRecord>
    {

    }
}