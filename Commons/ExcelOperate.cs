using OfficeOpenXml;
using System.Reflection;

namespace Commons;

public class ExcelOperate
{
    /// <summary>
    /// Receive a generic collection tClass and export the Excel file to the specified path.
    /// ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    /// </summary>
    /// <typeparam name="T">class T</typeparam>
    /// <param name="tClass">a generic collection</param>
    /// <param name="filePath">specified path</param>
    public async static Task OutputExcelAsync<T>(List<T> tClass, string filePath) where T : class
    {
        using var excelPackage = new ExcelPackage();
        Type type = typeof(T);

        var worksheet = excelPackage.Workbook.Worksheets.Add(type.Name);

        var infos = type.GetProperties();

        int column = 2;
        foreach (var info in infos)
        {
            worksheet.Cells[1, column].Value = info.Name;
            column++;
        }

        int row = 2;
        foreach (T item in tClass)
        {
            worksheet.Cells[row, 1].Value = row - 1; // 行号
            int col = 2;
            foreach (var property in infos)
            {
                worksheet.Cells[row, col].Value = property.GetValue(item);
                col++;
            }
            row++;
        }
        await excelPackage.SaveAsAsync(new FileInfo(filePath));
    }

    /// <summary>
    /// Given the specified file path, the Excel file is converted into a collection of generic types
    /// </summary>
    /// <typeparam name="T">class T where T : class, new()</typeparam>
    /// <param name="filePath">file path</param>
    /// <param name="startRow">The number of starting rows</param>
    /// <param name="startCol">The number of starting columns</param>
    /// <returns></returns>
    public static List<T>? Init<T>(string filePath, int startRow, int startCol) where T : class, new()
    {
        var file = new FileInfo(filePath);
        //文件不存在
        if (!file.Exists)
            return null;

        using var package = new ExcelPackage(file);
        //没有数据表
        if (package.Workbook.Worksheets.Count <= 0)
            return null;

        var ws = package.Workbook.Worksheets[0];
        var list = new List<T>();
        var infos = typeof(T).GetProperties();
        //数据表列数大于实体属性数
        if (ws.Dimension.Columns - startCol > infos.Length)
            return null;

        Dictionary<int, string> pairs = [];
        for (int pairsCol = startCol; pairsCol <= ws.Dimension.Columns; pairsCol++)
        {
            if (ws.Cells[startRow - 1, pairsCol].Value != null)
                pairs.Add(pairsCol, ws.Cells[startRow - 1, pairsCol].Value.ToString());
        }

        for (int row = startRow; row <= ws.Dimension.Rows; row++)
        {
            T item = new T();
            for (int col = startCol; col <= ws.Dimension.Columns; col++)
            {
                var cellValue = ws.Cells[row, col].Value;

                var pairValue = pairs.FirstOrDefault(p => p.Key == col);
                if (pairValue.Value == null)
                    return null;
                var info = infos.FirstOrDefault(i => i.Name == pairValue.Value);
                if (info == null)
                    return null;
                SetCellValue(item, cellValue, info);
            }
            list.Add(item);
        }
        return list;
    }

    private static void SetCellValue<T>(T item, object cellValue, PropertyInfo info) where T : class, new()
    {
        if (cellValue != null)
        {
            try
            {
                var convertedValue = Convert.ChangeType(cellValue, info.PropertyType);
                info.SetValue(item, convertedValue);
            }
            catch
            {
                SetDefaultValue(item, info);
                throw new FormatException($"Unable to convert value '{cellValue}' to the specified type '{info.PropertyType}', which has been set as the default.");
            }
        }
        else
            SetDefaultValue(item, info);
    }

    private static void SetDefaultValue<T>(T item, PropertyInfo info) where T : class, new()
    {
        // 当 cellValue 为空时，将对应的属性值赋为 null  
        if (info.PropertyType.IsClass) // 如果属性是引用类型（包括字符串）  
            info.SetValue(item, null);
        else if (info.PropertyType.IsValueType) // 如果属性是值类型  
            info.SetValue(item, Activator.CreateInstance(info.PropertyType)); // 针对值类型，可以赋默认值或使用 Nullable<T>设置为默认值  
    }
}
