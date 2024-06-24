using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using trains.models;
using System.Runtime.Remoting.Contexts;
using System.Diagnostics.Contracts;

namespace trains
{
    internal class Program
    {
        private static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            DbHelper.CreateDatabaseIfNotExists();

            var data = GetDataForReportByLinq("2236", "86560-725-98470");
            //GenerateExcelFileObjectFromPattern(data);
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
            return new List<OutputData>();
            /*stopwatch.Reset();

            IDataLayer dataLayer = XpoDefault.GetDataLayer(TrainsDbContext.connectionString, AutoCreateOption.None);
            using (Session context = new Session(dataLayer))
            {
                stopwatch.Start();
                using (var uow = new UnitOfWork(dataLayer))
                {
                    var lastHistories = context.Query<History>().GroupBy(h => h.CarId).Select(g => g.OrderByDescending(h => h.OperationDateTime).FirstOrDefault()).ToList();
                    
                    var data = context.Query<TrainsCars>()
                        .Join(context.Query<Train>(), tc => tc.TrainId, t => t.TrainId, (left, rigth) => new {
                            TrainId = rigth.TrainId,
                            TrainNumber = rigth.TrainNumber,
                            TrainIndexCombined = rigth.TrainIndexCombined,
                            CarPositionInTrain = left.CarPositionInTrain,
                            CarId = left.CarId
                        })
                        .Join(context.Query<Car>(), tct => tct.CarId, c => c.CarId, (left, rigth) => new {
                            TrainNumber = left.TrainNumber,
                            TrainIndexCombined = left.TrainIndexCombined,
                            CarNumber = rigth.CarNumber,
                            CarId = rigth.CarId,
                            InvoiceId = rigth.InvoiceId,
                            GrossWeight = rigth.GrossWeight,
                            FreightId = rigth.FreightId,
                            CarPositionInTrain = left.CarPositionInTrain
                        })
                        .Join(context.Query<History>().GroupBy(h => h.CarId).Select(g => g.OrderByDescending(h => h.OperationDateTime).FirstOrDefault()), tctc => tctc.CarId, h => h.CarId, (left, right) => new {
                            TrainNumber = left.TrainNumber,
                            TrainIndexCombined = left.TrainIndexCombined,
                            CarNumber = left.CarNumber,
                            InvoiceId = left.InvoiceId,
                            GrossWeight = left.GrossWeight,
                            FreightId = left.FreightId,
                            OperationDateTime = right.OperationDateTime,
                            OperationId = right.OperationId,
                            LastStationId = right.StationId,
                            CarPositionInTrain = left.CarPositionInTrain
                        })
                        .Join(context.Query<Station>(), tctcho => tctcho.LastStationId, i => i.StationId, (left, right) => new {
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
                        .Join(context.Query<Operation>(), tctch => tctch.OperationId, o => o.OperationId, (left, right) => new {
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
                        .Join(context.Query<Invoice>(), tctcho => tctcho.InvoiceId, i => i.InvoiceId, (left, right) => new {
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
                        .Join(context.Query<Freight>(), tctchoi => tctchoi.FreightId, f => f.FreightId, (left, right) => new SelectResult
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
            }*/
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
