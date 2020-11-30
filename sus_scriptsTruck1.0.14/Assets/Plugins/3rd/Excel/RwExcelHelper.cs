#if UNITY_EDITOR
using System.Collections;
using OfficeOpenXml;
using System.IO;
using Excel;


public class EPPlusHelper
{

    public static RwExcel LoadExcel(string path)
    {
        FileInfo file = new FileInfo(path);
        ExcelPackage ep = new ExcelPackage(file);
        RwExcel xls = new RwExcel(ep.Workbook);
        return xls;
    }
    

    public static void WriteExcel(RwExcel xls, string path)
    {
        FileInfo output = new FileInfo(path);
        ExcelPackage ep = new ExcelPackage();
        for (int i = 0; i < xls.Tables.Count; i++)
        {
            RwExcelSheet table = xls.Tables[i];
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add(table.TableName);
            for (int row = 1; row <= table.NumberOfRows; row++) {
                for (int column = 1; column <= table.NumberOfColumns; column++) {
                    sheet.Cells[row, column].Value = table.GetValue(row, column);
                }
            }
        }
        ep.SaveAs(output);
    }
}




public class ExcelHelper
{
    // public static DataTableCollection LoadExcel(string path)
    // {
    //     FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    //     IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    //
    //     DataSet result = excelReader.AsDataSet();
    //     //int columns = result.Tables[0].Columns.Count;
    //     //int rows = result.Tables[0].Rows.Count;
    //
    //     //tables可以按照sheet名获取，也可以按照sheet索引获取
    //     //return result.Tables[0].Rows;
    //     return result.Tables;//[sheetName].Rows;
    // }

    public static void WriteExcel(string path)
    {
        throw new System.NotSupportedException();
    }
}
#endif