using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinderJobs.MVC.ViewModels
{
    public class RespostaViewDistancia
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class Element
    {
        public Dados distance { get; set; }
        public Dados duration { get; set; }
        public string status { get; set; }
    }

    public class Dados
    {
        public string text { get; set; }
        public int value { get; set; }
    }
}