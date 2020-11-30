#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;

public class RwExcel
{
    public List <RwExcelSheet> Tables = new List<RwExcelSheet>();

    public RwExcel()
    {
		
    }

    public RwExcel(ExcelWorkbook wb)
    {
        for (int i = 1; i <= wb.Worksheets.Count; i++)
        {
            ExcelWorksheet sheet = wb.Worksheets[i];
            RwExcelSheet table = new RwExcelSheet(sheet);
            Tables.Add(table);
        }
    }
}




public class RwExcelCell
{
    public int RowIndex;
    public int ColumnIndex;
    public string Value;

    public RwExcelCell(int row, int column, string value)
    {
        RowIndex = row;
        ColumnIndex = column;
        Value = value;
    }
}


public class RwExcelSheet
{
    private Dictionary<int, Dictionary<int, RwExcelCell>> cells = new Dictionary<int, Dictionary<int, RwExcelCell>>();

    public string TableName;
    public int NumberOfRows;
    public int NumberOfColumns;


    public RwExcelSheet()
    {

    }

    public RwExcelSheet(ExcelWorksheet sheet)
    {
        TableName = sheet.Name;
        if (sheet.Dimension == null) return;

        NumberOfRows = sheet.Dimension.Rows;
        NumberOfColumns = sheet.Dimension.Columns;
        for (int row = 1; row <= NumberOfRows; row++)
        {
            for (int column = 1; column <= NumberOfColumns; column++)
            {
                object obj = sheet.Cells[row, column].Value;
                string value = "";
                if (obj != null) value = obj.ToString();

                SetValue(row, column, value);
            }
        }
    }

    public RwExcelCell SetValue(int row, int column, string value)
    {
        if (!cells.ContainsKey(row))
        {
            cells[row] = new Dictionary<int, RwExcelCell>();
        }
        if (cells[row].ContainsKey(column))
        {
            cells[row][column].Value = value;
            return cells[row][column];
        }
        else
        {
            RwExcelCell cell = new RwExcelCell(row, column, value);
            cells[row][column] = cell;
            CorrectSize(row, column);
            return cell;
        }
    }

    public object GetValue(int row, int column)
    {
        RwExcelCell cell = GetCell(row, column);
        if (cell != null)
        {
            return cell.Value;
        }
        else
        {
            return SetValue(row, column, "").Value;
        }
    }

    public RwExcelCell GetCell(int row, int column)
    {
        if (cells.ContainsKey(row))
        {
            if (cells[row].ContainsKey(column))
            {
                return cells[row][column];
            }
        }
        return null;
    }

    int Max(int a, int b)
    {
        if (a > b) return a;
        return b;
    }

    public void CorrectSize(int row, int column)
    {
        NumberOfRows = Max(row, NumberOfRows);
        NumberOfColumns = Max(column, NumberOfColumns);
    }
}
#endif