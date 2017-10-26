using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebAppDb4o.Data;
using WebAppDb4o.Models;

namespace WebAppDb4o.Controllers
{
    enum NameGender { Male, Female }
    public class PopName
    {
        // Fields:
        private string m_name;
        private NameGender m_gender;
        private string m_state;
        private int m_year;
        private int m_rank;
        private int m_count;

        // Properties:
        internal string Name { get { return m_name; } set { m_name = value; } }
        internal NameGender Gender { get { return m_gender; } set { m_gender = value; } }
        internal string State { get { return m_state; } set { m_state = value; } }
        internal int Year { get { return m_year; } set { m_year = value; } }
        internal int Rank { get { return m_rank; } set { m_rank = value; } }
        internal int Count { get { return m_count; } set { m_count = value; } }

        public override string ToString()
        {
            return string.Format("{{ Name={0}, Gender={1}, State={2}, Year={3}, Rank={4}, Count={5} }}",
                Name, Gender, State, Year, Rank, Count);
        }
    }
    public class PersonController : Controller
    {
        private List<PopName> _names = new List<PopName>();
        private readonly QueryInfo queryInfo = new QueryInfo();
        private static IEnumerable<PopName> _seqQuery;
        private static ParallelQuery<PopName> _parQuery;
        const int YearStart = 1960;
        const int YearEnd = 2000;
        const int YscaleMax = 1000;
        private const int RunMultiplier = 10;
        private long _lastSeqRun = 0;
        private long _lastParRun = 0;

        private class QueryInfo
        {
            internal string Name;
            internal string State;
        }

        public PersonController()
        {
            InitializeQueries();
        }
        private readonly PersonRepository db = new PersonRepository();
        // GET: Person
        public ActionResult Index()
        {
            return View(db.All());
        }

        public ActionResult Details(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Person model)
        {
            model.RowGuid = Guid.NewGuid().ToString();
            db.Add(model);
            return Redirect("Index");
        }
        #endregion

        #region Edit
        public ActionResult Edit(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }
        [HttpPost]
        public ActionResult Edit(Person model)
        {
            db.Edit(model);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            var person = db.Find(new Person { RowGuid = id });
            return View(person);
        }
        [HttpPost]
        public ActionResult Delete(Person model)
        {
            db.Delete(model);
            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        public ActionResult RunLinqSeq()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RunLinqSeq(float mbSize)
        {
            LoadNames(mbSize);
            queryInfo.Name = "Robert";
            queryInfo.State = "WA";
            try
            {
                // Ejecutar y cronometrar la consulta!!!!
                var sw = new Stopwatch();
                var mre = new System.Threading.ManualResetEvent(false);
                System.Threading.ThreadPool.QueueUserWorkItem(delegate 
                {
                    sw.Start();
                    for (var i = 0; i < RunMultiplier; i++)
                        _names = _seqQuery.ToList();
                    sw.Stop();
                    _lastSeqRun = sw.ElapsedTicks;
                    mre.Set();
                });
                mre.WaitOne();
                // Nota. Tiempo de ejecución:
                ViewBag.LinqTimeLabel = $"{(sw.ElapsedMilliseconds / 1000.0):F2} segundos";

                if (_lastSeqRun != 0 && _lastParRun != 0)
                {
                    ViewBag.SpeedupLabel = $"{((float) _lastSeqRun) / _lastParRun:F2}x speedup";
                }
            }
            finally
            {
            }
            return View();
        }
        public ActionResult RunPLinqSeq()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RunPLinqSeq(float mbSize)
        {
            LoadNames(mbSize);
            queryInfo.Name = "Robert";
            queryInfo.State = "WA";
            try
            {
                // Execute and time the query:
                List<PopName> names = null;
                Stopwatch sw = new Stopwatch();

                System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    sw.Start();
                    for (int i = 0; i < RunMultiplier; i++)
                        names = _parQuery.ToList();
                    sw.Stop();
                    _lastParRun = sw.ElapsedTicks;
                    mre.Set();
                });
                mre.WaitOne();
                // Nota. Tiempo de ejecución:
                ViewBag.LinqTimeLabel = $"{(sw.ElapsedMilliseconds / 1000.0):F2} segundos";
                if (_lastSeqRun != 0 && _lastParRun != 0)
                {
                    ViewBag.SpeedupLabel = $"{((float)_lastSeqRun) / _lastParRun:F2}x speedup";
                }
            }
            finally
            {
            }
            return View();
        }

        void LoadNames(float mbSize)
        {
            const int recordSize = 32; // aprox. 32 bytes por registro.
            var count = (int)((mbSize * 1024 * 1024) / recordSize);
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\popnames.xml";
            try {
                _lastSeqRun = 0; _lastParRun = 0; _names.Clear();
                Console.Write("Cargando nombres desde archivo XML ...");
                var doc = XDocument.Load(path);
                var root = doc.Root;
                if (root != null)
                    foreach (var child in root.Elements())
                    {
                        var name = new PopName
                        {
                            Name = child.Attribute("Name").Value,
                            Gender = (NameGender) Enum.Parse(typeof(NameGender), child.Attribute("Gender").Value),
                            State = child.Attribute("State").Value,
                            Year = int.Parse(child.Attribute("Year").Value),
                            Rank = int.Parse(child.Attribute("Rank").Value),
                            Count = int.Parse(child.Attribute("Count").Value)
                        };
                        _names.Add(name);
                        if (_names.Count == count) break;
                    }
                while (count > _names.Count) _names.AddRange(_names);

                if (_names.Count > count) {
                    var remCount = _names.Count - count; _names.RemoveRange(_names.Count - remCount - 1, remCount);
                }
                _seqQuery.ToList();
                _parQuery.ToList();
            }
            finally
            {
            }
        }
        private void InitializeQueries()
        {
            _seqQuery = from n in _names
                where n.Name.Equals(queryInfo.Name, StringComparison.InvariantCultureIgnoreCase) &&
                      n.State == queryInfo.State &&
                      n.Year >= YearStart && n.Year <= YearEnd
                orderby n.Year
                select n;

            _parQuery = from n in _names.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                where n.Name.Equals(queryInfo.Name, StringComparison.InvariantCultureIgnoreCase) &&
                      n.State == queryInfo.State &&
                      n.Year >= YearStart && n.Year <= YearEnd
                orderby n.Year
                select n;
        }
    }
}