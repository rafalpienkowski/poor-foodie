using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace PoorFoodie.Models
{
    public class GoogleSpreadsheet
    {
        private readonly string _spreadsheetId;
        private readonly SheetsService _service;
        private Spreadsheet _spreadSheet;
        private Sheet _currentWeekSheet;

        public GoogleSpreadsheet(string spreadsheetId, SheetsService service)
        {
            _spreadsheetId = spreadsheetId;
            _service = service;
        }

        public void Init()
        {
            var requestAll = _service.Spreadsheets.Get(_spreadsheetId);
            requestAll.IncludeGridData = true;
            _spreadSheet = requestAll.Execute();
        }

        public IList<Order> GetOrdersForDate(DateTime date)
        {
            if (_spreadSheet == null)
            {
                Init();
            }
            GetSheetForWeek(date);
            var currentDateRowData = GetRowDataForDate(date);
            return GetOrdersFromDate(currentDateRowData);
        }

        private void GetSheetForWeek(DateTime date)
        {
            var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            _currentWeekSheet = _spreadSheet.Sheets.SingleOrDefault(s => s.Properties.Title.Contains($"Week {weekNumber}"));
            if(_currentWeekSheet == null)
            {
                throw new ArgumentOutOfRangeException($"Couldn't find sheet for: 'Week {weekNumber}'");
            }
        }

        private RowData GetRowDataForDate(DateTime date)
        {
            var dateFormated = date.ToString("dd.MM.yyyy");
            var firstData = _currentWeekSheet.Data.FirstOrDefault();
            var currentDateRow = firstData.RowData.FirstOrDefault(rd => rd.Values[1].FormattedValue == dateFormated);
            if (currentDateRow == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            return currentDateRow;
        }

        private IList<Order> GetOrdersFromDate(RowData currentDateDataRow)
        {
            var firstData = _currentWeekSheet.Data.FirstOrDefault();
            var idx = firstData.RowData.IndexOf(currentDateDataRow);
            var orders = new List<Order>();
            while(firstData.RowData[idx + 1] != null && !string.IsNullOrEmpty(firstData.RowData[idx].Values[1].FormattedValue))
            {
                var row = firstData.RowData[idx];
                orders.Add(new Order(
                    number: row.Values[0].FormattedValue,
                    client: row.Values[1].FormattedValue,
                    soup: row.Values[2].FormattedValue,
                    mainDish: row.Values[3].FormattedValue,
                    basePrice: row.Values[4].FormattedValue,
                    discountPrice: row.Values[5].FormattedValue
                ));
                idx++;
            }
            return orders;
        }

        public IList<Order> GetCurrentOrders()
        {
            return GetOrdersForDate(DateTime.Now);
        }
    }
}