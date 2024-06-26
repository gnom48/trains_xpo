using DevExpress.Xpo;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using trains.models;

namespace trains
{
    internal class Program
    {
        private static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            DbHelper.CreateDatabaseIfNotExists(); // нужно использовать для инициализации dataLayer в DbHelper
            DbHelper.DeleteData();
            DbHelper.LoadDataFromXml();

            //var data = GetDataForReportBySqlQuery("2236", "86560-725-98470");
            //var data = GetDataForReportByLinqLite("2236", "86560-725-98470");
            //var data = GetDataForReportByLinq("2236", "86560-725-98470");

            //GenerateExcelFileObjectFromPattern(data);
            Console.ReadKey();
        }

        /// <summary>
        /// Создает натурный лист поезда в Excel
        /// </summary>
        /// <param name="data">данные необходимые для построение натурного листа поезда</param>
        /// <returns>документ Excel в виде массива байт</returns>
        public static void GenerateNewExcelFileObject(List<OutputData> data)
        {
            using (var package = new ExcelPackage(new FileInfo(@"C:\Users\Egorc\Desktop\report.xlsx")))
            {
                var sheet = package.Workbook.Worksheets.Add("Вывод");

                sheet.Cells["A1:G1"].Merge = true;
                sheet.Cells["A1:G1"].Value = "Натурный лист поезда";

                sheet.Cells["A3"].Value = "Поезд №:";
                sheet.Cells["A4"].Value = "Состав №:";
                sheet.Cells["D3"].Value = "Станция:";
                sheet.Cells["D4"].Value = "Дата:";

                sheet.Cells["A6"].Value = "№";
                sheet.Cells["B6"].Value = "№ вагона";
                sheet.Cells["C6"].Value = "Накладная";
                sheet.Cells["D6"].Value = "Дата отправления";
                sheet.Cells["E6"].Value = "Груз";
                sheet.Cells["F6"].Value = "Вес по документам (т)";
                sheet.Cells["F6"].Style.TextRotation = 90;
                sheet.Cells["G6"].Value = "Последняя операция";

                if (data == null || data.Count() == 0)
                {
                    package.Save();
                    return;
                }

                sheet.Cells["B3"].Value = $"{data[0].RecordsInGroup[0].TrainNumber}";
                sheet.Cells["B4"].Value = $"{data[0].RecordsInGroup[0].TrainIndexCombined.Split('-')[1]}";
                sheet.Cells["E3"].Value = $"{data[0].RecordsInGroup[0].StationName}";
                sheet.Cells["E4"].Value = $"{data[0].RecordsInGroup[0].LastOperationDateTime.ToShortDateString()}";

                int lastRow = 0;
                int counter = 0;
                foreach (var item in data)
                {
                    counter = 0;
                    foreach (var rec in item.RecordsInGroup)
                    {
                        counter++;
                        lastRow++;
                        sheet.Cells[$"A{6 + lastRow}"].Value = $"{counter}";
                        sheet.Cells[$"B{6 + lastRow}"].Value = $"{rec.CarNumber}";
                        sheet.Cells[$"C{6 + lastRow}"].Value = $"{rec.InvoiceName}";
                        sheet.Cells[$"D{6 + lastRow}"].Value = $"{rec.LastOperationDateTime}";
                        sheet.Cells[$"E{6 + lastRow}"].Value = $"{rec.FreightName}";
                        sheet.Cells[$"F{6 + lastRow}"].Value = $"{rec.GrossWeight}";
                        sheet.Cells[$"G{6 + lastRow}"].Value = $"{rec.OperationName}";
                    }
                    lastRow++;
                    sheet.Cells[$"B{6 + lastRow}"].Value = $"{item.TotalGroupCount}";
                    sheet.Cells[$"B{6 + lastRow}"].Style.Font.Bold = true;
                    sheet.Cells[$"E{6 + lastRow}"].Value = $"{item.GroupFreight}";
                    sheet.Cells[$"E{6 + lastRow}"].Style.Font.Bold = true;
                    sheet.Cells[$"F{6 + lastRow}"].Value = $"{item.TotalGroupWeight}";
                    sheet.Cells[$"F{6 + lastRow}"].Style.Font.Bold = true;
                }
                lastRow++;
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Merge = true;
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Value = $"Всего: {data.Select(x => x.TotalGroupCount).Sum()}";
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Style.Font.Bold = true;
                sheet.Cells[$"E{6 + lastRow}"].Value = $"{data.Select(x => x.GroupFreight).Count()}";
                sheet.Cells[$"E{6 + lastRow}"].Style.Font.Bold = true;
                sheet.Cells[$"F{6 + lastRow}"].Value = $"{data.Select(x => x.TotalGroupWeight).Sum()}";
                sheet.Cells[$"F{6 + lastRow}"].Style.Font.Bold = true;

                var table = sheet.Cells[$"A6:G{sheet.Dimension.End.Row}"];
                table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var allCells = sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column];
                allCells.Style.Font.SetFromFont(new Font("Times New Roman", 10));
                allCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                allCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 18));


                sheet.Cells["A3:A4"].Style.Font.Bold = true;
                sheet.Cells["D3:D4"].Style.Font.Bold = true;
                sheet.Cells["A1:G1"].Style.Font.Bold = true;
                sheet.Cells["A6:G6"].Style.Font.Bold = true;
                sheet.Cells[$"A{sheet.Dimension.End.Row}:G{sheet.Dimension.End.Row}"].Style.Font.Bold = true;

                package.Save();
            }
        }

        /// <summary>
        /// Создает натурный лист поезда в Excel на основе шаблона
        /// </summary>
        /// <param name="data">данные необходимые для построение натурного листа поезда</param>
        /// <returns>документ Excel в виде массива байт</returns>
        public static void GenerateExcelFileObjectFromPattern(List<OutputData> data)
        {
            var template = new FileInfo(Directory.GetCurrentDirectory() + @"\resources\report.xlsx");
            var file = template.CopyTo(@"C:\Users\Egorc\Desktop\report.xlsx", true);

            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets["Вывод"];

                if (data == null || data.Count() == 0)
                {
                    package.Save();
                    return;
                }

                sheet.Cells["B3"].Value = $"{data[0].RecordsInGroup[0].TrainNumber}";
                sheet.Cells["B4"].Value = $"{data[0].RecordsInGroup[0].TrainIndexCombined.Split('-')[1]}";
                sheet.Cells["E3"].Value = $"{data[0].RecordsInGroup[0].StationName}";
                sheet.Cells["E4"].Value = $"{data[0].RecordsInGroup[0].LastOperationDateTime.ToShortDateString()}";

                int lastRow = 0;
                int counter = 0;
                foreach (var item in data)
                {
                    counter = 0;
                    foreach (var rec in item.RecordsInGroup)
                    {
                        counter++;
                        lastRow++;
                        sheet.Cells[$"A{6 + lastRow}"].Value = $"{counter}";
                        sheet.Cells[$"B{6 + lastRow}"].Value = $"{rec.CarNumber}";
                        sheet.Cells[$"C{6 + lastRow}"].Value = $"{rec.InvoiceName}";
                        sheet.Cells[$"D{6 + lastRow}"].Value = $"{rec.LastOperationDateTime}";
                        sheet.Cells[$"E{6 + lastRow}"].Value = $"{rec.FreightName}";
                        sheet.Cells[$"F{6 + lastRow}"].Value = $"{rec.GrossWeight}";
                        sheet.Cells[$"G{6 + lastRow}"].Value = $"{rec.OperationName}";
                    }
                    lastRow++;
                    sheet.Cells[$"B{6 + lastRow}"].Value = $"{item.TotalGroupCount}";
                    sheet.Cells[$"B{6 + lastRow}"].Style.Font.Bold = true;
                    sheet.Cells[$"E{6 + lastRow}"].Value = $"{item.GroupFreight}";
                    sheet.Cells[$"E{6 + lastRow}"].Style.Font.Bold = true;
                    sheet.Cells[$"F{6 + lastRow}"].Value = $"{item.TotalGroupWeight}";
                    sheet.Cells[$"F{6 + lastRow}"].Style.Font.Bold = true;
                }
                lastRow++;
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Merge = true;
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Value = $"Всего: {data.Select(x => x.TotalGroupCount).Sum()}";
                sheet.Cells[$"A{6 + lastRow}:B{6 + lastRow}"].Style.Font.Bold = true;
                sheet.Cells[$"E{6 + lastRow}"].Value = $"{data.Select(x => x.GroupFreight).Count()}";
                sheet.Cells[$"E{6 + lastRow}"].Style.Font.Bold = true;
                sheet.Cells[$"F{6 + lastRow}"].Value = $"{data.Select(x => x.TotalGroupWeight).Sum()}";
                sheet.Cells[$"F{6 + lastRow}"].Style.Font.Bold = true;

                var table = sheet.Cells[$"A6:G{sheet.Dimension.End.Row}"];
                table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                var allCells = sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column];
                allCells.Style.Font.SetFromFont(new Font("Times New Roman", 10));
                allCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                allCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A1:G1"].Style.Font.SetFromFont(new Font("Times New Roman", 18));

                sheet.Cells["A3:A4"].Style.Font.Bold = true;
                sheet.Cells["D3:D4"].Style.Font.Bold = true;
                sheet.Cells["A1:G1"].Style.Font.Bold = true;
                sheet.Cells["A6:G6"].Style.Font.Bold = true;
                sheet.Cells[$"A{sheet.Dimension.End.Row}:G{sheet.Dimension.End.Row}"].Style.Font.Bold = true;

                package.Save();
            }
        }

        /// <summary>
        /// Делает запрос к базе данных с помощью Linq и получает данные в виде натурного листа указанного состава
        /// </summary>
        /// <param name="trainNumber">номер поезда</param>
        /// <param name="trainIndexCombined">составной индекс состава</param>
        /// <returns>данные натурного листа</returns>
        public static List<OutputData> GetDataForReportByLinq(string trainNumber, string trainIndexCombined)
        {
            stopwatch.Reset();
            stopwatch.Start();
            using (var uow = new UnitOfWork(DbHelper.dataLayer))
            {
                var lastHistories = uow.Query<History>().GroupBy(h => h.Car).Select(g => g.OrderByDescending(h => h.OperationDateTime).FirstOrDefault()).ToList();

                var data = uow.Query<TrainsCars>().ToList()
                    .Join(uow.Query<Train>().ToList(), tc => tc.Train.Oid, t => t.Oid, (left, rigth) => new
                    {
                        TrainId = rigth.Oid,
                        TrainNumber = rigth.TrainNumber,
                        TrainIndexCombined = rigth.TrainIndexCombined,
                        CarPositionInTrain = left.CarPositionInTrain,
                        CarId = left.Car.Oid
                    })
                    .Join(uow.Query<Car>().ToList(), tct => tct.CarId, c => c.Oid, (left, rigth) => new
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = rigth.CarNumber,
                        CarId = rigth.Oid,
                        InvoiceId = rigth.Invoice.Oid,
                        GrossWeight = rigth.GrossWeight,
                        FreightId = rigth.Freight.Oid,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Join(lastHistories, tctc => tctc.CarId, h => h.Car.Oid, (left, right) => new
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = left.CarNumber,
                        InvoiceId = left.InvoiceId,
                        GrossWeight = left.GrossWeight,
                        FreightId = left.FreightId,
                        OperationDateTime = right.OperationDateTime,
                        OperationId = right.Operation.Oid,
                        LastStationId = right.Station.Oid,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Join(uow.Query<Station>().ToList(), tctcho => tctcho.LastStationId, i => i.Oid, (left, right) => new
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = left.CarNumber,
                        InvoiceId = left.InvoiceId,
                        GrossWeight = left.GrossWeight,
                        FreightId = left.FreightId,
                        OperationDateTime = left.OperationDateTime,
                        OperationId = left.OperationId,
                        LastStationName = right.StationName,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Join(uow.Query<Operation>().ToList(), tctch => tctch.OperationId, o => o.Oid, (left, right) => new
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = left.CarNumber,
                        InvoiceId = left.InvoiceId,
                        GrossWeight = left.GrossWeight,
                        FreightId = left.FreightId,
                        OperationDateTime = left.OperationDateTime,
                        OperationName = right.OperationName,
                        LastStationName = left.LastStationName,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Join(uow.Query<Invoice>().ToList(), tctcho => tctcho.InvoiceId, i => i.Oid, (left, right) => new
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = left.CarNumber,
                        InvoiceNum = right.InvoiceName,
                        GrossFreightKg = left.GrossWeight,
                        FreightId = left.FreightId,
                        OperationDateTime = left.OperationDateTime,
                        OperationName = left.OperationName,
                        LastStationName = left.LastStationName,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Join(uow.Query<Freight>().ToList(), tctchoi => tctchoi.FreightId, f => f.Oid, (left, right) => new SelectResult
                    {
                        TrainNumber = left.TrainNumber,
                        TrainIndexCombined = left.TrainIndexCombined,
                        CarNumber = left.CarNumber,
                        InvoiceName = left.InvoiceNum,
                        GrossWeight = left.GrossFreightKg,
                        StationName = left.LastStationName,
                        FreightName = right.FreightName,
                        LastOperationDateTime = left.OperationDateTime,
                        OperationName = left.OperationName,
                        CarPositionInTrain = left.CarPositionInTrain
                    })
                    .Where(x => x.TrainIndexCombined == trainIndexCombined && x.TrainNumber == trainNumber)
                    .OrderBy(x => x.CarPositionInTrain).ThenBy(x => x.CarNumber)
                    .ToList();

                stopwatch.Stop();
                Console.WriteLine($"Linq выборка заняла {stopwatch.ElapsedMilliseconds} мс");

                return data.GroupBy(x => x.FreightName).Select(group => new OutputData
                {
                    GroupFreight = group.Key,
                    TotalGroupWeight = group.Select(x => x.GrossWeight).Sum(),
                    TotalGroupCount = group.Select(x => x.CarNumber).Count(),
                    RecordsInGroup = group.OrderBy(x => x.CarPositionInTrain).ToList()
                }).ToList();
            }
        }

        /// <summary>
        /// Делает запрос к базе данных с помощью Linq и получает данные в виде натурного листа указанного состава (оптимизировано)
        /// </summary>
        /// <param name="trainNumber">номер поезда</param>
        /// <param name="trainIndexCombined">составной индекс состава</param>
        /// <returns>данные натурного листа</returns>
        public static List<OutputData> GetDataForReportByLinqLite(string trainNumber, string trainIndexCombined)
        {
            stopwatch.Reset();
            stopwatch.Start();
            using (var uow = new UnitOfWork(DbHelper.dataLayer))
            {
                var lastHistories = uow.Query<History>().Where(x => x.Car.TrainsCars_.Any(t => t.Train.TrainIndexCombined == trainIndexCombined && t.Train.TrainNumber == trainNumber)).GroupBy(h => h.Car).Select(g => g.OrderByDescending(h => h.OperationDateTime).FirstOrDefault()).ToList();

                List<SelectResult> data = uow.Query<TrainsCars>().Where(x => x.Train.TrainIndexCombined == trainIndexCombined && x.Train.TrainNumber == trainNumber).Select(x => new SelectResult
                {
                    CarPositionInTrain = x.CarPositionInTrain,
                    CarNumber = x.Car.CarNumber,
                    TrainNumber = x.Train.TrainNumber,
                    TrainIndexCombined = x.Train.TrainIndexCombined,
                    FreightName = x.Car.Freight.FreightName,
                    GrossWeight = x.Car.GrossWeight,
                    InvoiceName = x.Car.Invoice.InvoiceName,
                    LastOperationDateTime = lastHistories.SingleOrDefault(h => h.Car.Oid == x.Car.Oid).OperationDateTime,
                    OperationName = lastHistories.SingleOrDefault(h => h.Car.Oid == x.Car.Oid).Operation.OperationName,
                    StationName = lastHistories.SingleOrDefault(h => h.Car.Oid == x.Car.Oid).Station.StationName
                }).ToList();
                    
                data = data
                .OrderBy(x => x.CarPositionInTrain).ThenBy(x => x.CarNumber)
                .ToList();

                stopwatch.Stop();
                Console.WriteLine($"Сокращенная Linq выборка заняла {stopwatch.ElapsedMilliseconds} мс");

                return data.GroupBy(x => x.FreightName).Select(group => new OutputData
                {
                    GroupFreight = group.Key,
                    TotalGroupWeight = group.Select(x => x.GrossWeight).Sum(),
                    TotalGroupCount = group.Select(x => x.CarNumber).Count(),
                    RecordsInGroup = group.OrderBy(x => x.CarPositionInTrain).ToList()
                }).ToList();
            }
        }

        /// <summary>
        /// Делает прямой Sql запрос к базе данных и получает данные в виде натурного листа указанного состава
        /// </summary>
        /// <param name="trainNumber">номер поезда</param>
        /// <param name="trainIndexCombined">составной индекс состава</param>
        /// <returns>данные натурного листа</returns>
        public static List<OutputData> GetDataForReportBySqlQuery(string trainNumber, string trainIndexCombined)
        {
            stopwatch.Reset();

            using (var uow = new UnitOfWork(DbHelper.dataLayer))
            {
                stopwatch.Start();

                var data = new List<SelectResult>();

                var sqlRes = uow.ExecuteQuery($@"select t.TrainNumber, t.TrainIndexCombined, tc.CarPositionInTrain, 
	                    c.CarNumber, h.LastOperationDateTime, c.GrossWeight, o.OperationName, 
	                    i.InvoiceName, s.StationName, f.FreightName
                    from dbo.TrainsCars as tc 
                        inner join dbo.Car as c on (tc.Car = c.OID)
                        inner join dbo.Train as t on (tc.Train = t.OID)
                        inner join (select Car, max(OperationDateTime) as LastOperationDateTime,
                            max(Operation) as Operation, 
                            max(Station) as Station
                            from dbo.History
                            group by Car)  as h on (h.Car = c.OID)
                        inner join dbo.Operation as o on (o.OID = h.Operation)
                        inner join dbo.Station as s on (h.Station = s.OID)
                        inner join dbo.Freight as f on (f.OID = c.Freight)
                        inner join dbo.Invoice as i on (c.Invoice = i.OID)
                    where t.TrainNumber = '{trainNumber}' and t.TrainIndexCombined = '{trainIndexCombined}'
                    order by tc.CarPositionInTrain, c.CarNumber;
                    ");

                stopwatch.Stop();
                Console.WriteLine($"Sql запрос занял {stopwatch.ElapsedMilliseconds} мс");

                foreach(var val in sqlRes.ResultSet[0].Rows)
                {
                    data.Add(new SelectResult
                    {
                        TrainNumber = val.XmlValues[0].ToString(),
                        TrainIndexCombined = val.XmlValues[1].ToString(),
                        CarPositionInTrain = (int)val.XmlValues[2],
                        CarNumber = val.XmlValues[3].ToString(),
                        LastOperationDateTime = DateTime.Parse(val.XmlValues[4].ToString()),
                        GrossWeight = (int)val.XmlValues[5],
                        OperationName = val.XmlValues[6].ToString(),
                        InvoiceName = val.XmlValues[7].ToString(),
                        StationName = val.XmlValues[8].ToString(),
                        FreightName = val.XmlValues[9].ToString(),
                    });
                    
                }

                return data.GroupBy(x => x.FreightName).Select(group => new OutputData
                {
                    GroupFreight = group.Key,
                    TotalGroupWeight = group.Select(x => x.GrossWeight).Sum(),
                    TotalGroupCount = group.Select(x => x.CarNumber).Count(),
                    RecordsInGroup = group.OrderBy(x => x.CarPositionInTrain).ToList()
                }).ToList();

                return new List<OutputData>();
            }
        }

        /// <summary>
        /// Выводит отчет на основании натурного листа поезда в консоль
        /// </summary>
        /// <param name="data">данные для вывода</param>
        public static void PrintReport(List<OutputData> data)
        {
            if (data == null || data.Count() == 0) return;

            Console.WriteLine("Натурный лист поезда");
            Console.WriteLine($"Поезд:{data[0].RecordsInGroup[0].TrainNumber}");
            Console.WriteLine($"Состав: {data[0].RecordsInGroup[0].TrainIndexCombined.Split('-')[1]}");
            Console.WriteLine($"Станция: {data[0].RecordsInGroup[0].StationName}");
            Console.WriteLine($"Дата: {data[0].RecordsInGroup[0].LastOperationDateTime.ToShortDateString()}");
            Console.WriteLine("#\t# вагона\tНакладная\tДата отправления\t\tГруз\t\tВес по документам (т)\tПоследняя операция");
            foreach(var item in data)
            {
                short counter = 0;
                foreach(var rec in item.RecordsInGroup)
                {
                    counter++;
                    Console.WriteLine($"{counter}\t{rec.CarNumber}\t{rec.InvoiceName}\t{rec.LastOperationDateTime}\t{rec.FreightName}\t\t{rec.GrossWeight}\t{rec.OperationName}");
                }
                Console.WriteLine($"----\t{item.TotalGroupCount}\t---\t---\t{item.GroupFreight}\t---\t{item.TotalGroupWeight}");
            }
            Console.WriteLine($"Всего: {data.Select(x => x.TotalGroupCount).Sum()}\t---\t---\t{data.Select(x => x.GroupFreight).Count()}\t---\t{data.Select(x => x.TotalGroupWeight).Sum()}");
        }
    }
}
