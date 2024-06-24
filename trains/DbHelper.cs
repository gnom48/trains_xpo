using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using trains.code;
using trains.models;

namespace trains
{
    /// <summary>
    /// Класс DbHelper
    /// предоставляет методы для работы с базой данных
    /// </summary>
    public class DbHelper
    {
        /// <summary>
        /// Создает новую базу данных, если не существует, иначе ничего не делает
        /// </summary>
        public static void CreateDatabaseIfNotExists()
        {
            IDataLayer dataLayer = XpoDefault.GetDataLayer(TrainsDbContext.connectionString, AutoCreateOption.DatabaseAndSchema);
            using (Session context = new Session(dataLayer))
            {
                context.UpdateSchema(
                    typeof(Invoice), 
                    typeof(Freight), 
                    typeof(Operation), 
                    typeof(Station), 
                    typeof(Car),
                    typeof(Train),
                    typeof(History),
                    typeof(TrainsCars));
            }
        }

        /// <summary>
        /// Заполняет таблицы базы данных информацией из входного файла
        /// </summary>
        /// <param name="filename">путь до файла с данными в формате XML (необязательный, по умолчанию тестовые данные)</param>
        //public static void LoadDataFromXml(string filename = @"\resources\Data.xml")
        //{
        //    // построчное чтение файла
        //    StringBuilder xml = new StringBuilder();
        //    using (var fs = new StreamReader(Directory.GetCurrentDirectory() + filename))
        //    {
        //        while (!fs.EndOfStream)
        //        {
        //            xml.Append(fs.ReadLine());
        //        }
        //    }

        //    // десериализация в объект Root
        //    Root root;
        //    XmlSerializer serializer = new XmlSerializer(typeof(Root));
        //    using (StringReader reader = new StringReader(xml.ToString()))
        //    {
        //        root = (Root)serializer.Deserialize(reader);
        //        root.Rows = root.Rows.OrderBy(x => x.TrainNumber).ThenBy(x => x.TrainIndexCombined).ThenBy(x => x.CarNumber).ThenByDescending(x => DateTime.Parse(x.WhenLastOperation)).ToList();
        //    }

        //    var uniqueCars = root.Rows.Select(x => x.CarNumber).OrderBy(x => x).ToList();
        //    uniqueCars = uniqueCars.Distinct().ToList();

        //    var freights = new List<Freight>();
        //    var operations = new List<Operation>();
        //    var stations = new List<Station>();
        //    var invoices = new List<Invoice>();
        //    var cars = new List<Car>();
        //    var trainses = new List<Train>();
        //    var trainsCars = new List<TrainsCars>();
        //    var histories = new List<History>();

        //    // заполнение таблиц-справочников
        //    foreach (var r in root.Rows)
        //    {
        //        freights.Add(new Freight { FreightName = r.FreightEtsngName });
        //        operations.Add(new Operation { OperationName = r.LastOperationName });
        //        stations.Add(new Station { StationName = r.LastStationName });
        //        stations.Add(new Station { StationName = r.FromStationName });
        //        stations.Add(new Station { StationName = r.ToStationName });
        //        invoices.Add(new Invoice { InvoiceName = r.InvoiceNum });
        //    }

        //    // удаление дублирующихся записей из справочников
        //    freights = freights.GroupBy(x => x.FreightName).Select(g => g.First()).ToList();
        //    operations = operations.GroupBy(x => x.OperationName).Select(g => g.First()).ToList();
        //    invoices = invoices.GroupBy(x => x.InvoiceName).Select(g => g.First()).ToList();
        //    stations = stations.GroupBy(x => x.StationName).Select(g => g.First()).ToList();

        //    // занесение справочников в базу данных
        //    using (var context = new TrainsDbContext())
        //    {
        //        foreach (var x in freights) 
        //            //if (context.Freights.Where(f => f.FreightName == x.FreightName).ToList().Count() == 0) 
        //                context.Freights.Add(x);
        //        foreach (var x in operations) 
        //            //if (context.Operations.Where(f => f.OperationName == x.OperationName).ToList().Count() == 0) 
        //                context.Operations.Add(x);
        //        foreach (var x in stations) 
        //            //if (context.Stations.Where(f => f.StationName == x.StationName).ToList().Count() == 0) 
        //                context.Stations.Add(x);
        //        foreach (var x in invoices) 
        //            //if (context.Invoices.Where(f => f.InvoiceName == x.InvoiceName).ToList().Count() == 0) 
        //                context.Invoices.Add(x);
        //        context.SaveChanges();
        //    }

        //    using (var context = new TrainsDbContext())
        //    {
        //        // заполнение зависимых таблиц
        //        foreach (var r in root.Rows)
        //        {
        //            if (uniqueCars.Contains(r.CarNumber))
        //            {
        //                cars.Add(new Car
        //                {
        //                    CarNumber = r.CarNumber.ToString(),
        //                    GrossWeight = r.FreightTotalWeightKg,
        //                    InvoiceId = context.Invoices.Where(x => x.InvoiceName == r.InvoiceNum).FirstOrDefault().InvoiceId,
        //                    FreightId = context.Freights.Where(x => x.FreightName == r.FreightEtsngName).FirstOrDefault().FreightId,
        //                });

        //                uniqueCars.Remove(r.CarNumber);
        //            }

        //            trainses.Add(new Train
        //            {
        //                TrainIndexCombined = r.TrainIndexCombined,
        //                TrainNumber = r.TrainNumber.ToString(),
        //                FromStationId = context.Stations.Where(x => x.StationName == r.FromStationName).FirstOrDefault().StationId,
        //                ToStationId = context.Stations.Where(x => x.StationName == r.ToStationName).FirstOrDefault().StationId,
        //            });
        //        }

        //        // удаление дублирующихся записей из зависимых таблиц
        //        cars.OrderBy(x => x.CarNumber);
        //        trainses = trainses.Distinct(new TrainsComparer()).ToList();
        //        trainses.OrderBy(x => x.TrainIndexCombined).OrderBy(x => x.TrainNumber);

        //        // занесение данных в базу данных
        //        //context.Cars.AddRange(cars);
        //        //context.Trains.AddRange(trainses);

        //        foreach (var x in cars) 
        //            //if (context.Cars.Where(f => f.CarNumber == x.CarNumber).ToList().Count() == 0) 
        //                context.Cars.Add(x);
        //        foreach (var x in trainses) 
        //            //if (context.Trains.Where(f => f.TrainNumber == x.TrainNumber && f.TrainIndexCombined == x.TrainIndexCombined).ToList().Count() == 0) 
        //                context.Trains.Add(x);

        //        context.SaveChanges();
        //    }

        //    using (var context = new TrainsDbContext())
        //    {
        //        // заполнение смежной таблицы
        //        var tmp_cars = context.Cars.ToList();
        //        var tmp_trains = context.Trains.ToList();
        //        foreach (var r in root.Rows)
        //        {
        //            var tc = new TrainsCars
        //            {
        //                CarId = tmp_cars.Where(x => x.CarNumber == r.CarNumber.ToString()).FirstOrDefault().CarId,
        //                TrainId = tmp_trains.Where(x => x.TrainNumber == r.TrainNumber.ToString() && x.TrainIndexCombined == r.TrainIndexCombined).FirstOrDefault().TrainId,
        //                CarPositionInTrain = r.PositionInTrain
        //            };
        //            //if (context.TrainsCars.Where(x => x.CarId == tc.CarId && x.TrainId == tc.TrainId).ToList().Count() == 0) 
        //                trainsCars.Add(tc);
        //        }

        //        trainsCars = trainsCars.Distinct(new TrainsCarsComparer()).ToList();

        //        // занесение в базу данных записей смежной таблицы
        //        context.TrainsCars.AddRange(trainsCars);
        //        context.SaveChanges();
        //    }

        //    using (var context = new TrainsDbContext())
        //    {
        //        var tmp_cars = context.Cars.ToList();
        //        var tmp_operations = context.Operations.ToList();
        //        var tmp_stations = context.Stations.ToList();
        //        foreach (var r in root.Rows)
        //        {
        //            histories.Add(new History
        //            {
        //                CarId = tmp_cars.Where(x => x.CarNumber == r.CarNumber.ToString()).First().CarId,
        //                OperationId = tmp_operations.Where(x => x.OperationName == r.LastOperationName).First().OperationId,
        //                StationId = tmp_stations.Where(x => x.StationName == r.LastStationName).First().StationId,
        //                OperationDateTime = DateTime.Parse(r.WhenLastOperation)
        //            });
        //        }

        //        context.Histories.AddRange(histories);
        //        context.SaveChanges();
        //    }
        //}

        /// <summary>
        /// Очищает данные в таблицах и обнуляет начальные значения идентификаторов
        /// </summary>
        public static void DeleteData()
        {
            using (var context = TrainsDbContext.GetUnitOfWork())
            {
                context.ExecuteNonQuery("TRUNCATE TABLE dbo.TrainsCars");
                context.ExecuteNonQuery("TRUNCATE TABLE dbo.Trains");
                context.ExecuteNonQuery("TRUNCATE TABLE dbo.Cars");
                context.ExecuteNonQuery("TRUNCATE TABLE dbo.Histories");
            }
        }
    }
}
