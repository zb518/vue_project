using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using PPE.Model.Shared;

namespace PPE.Core;

public class ExcelHelper
{


    public static List<T>? ReadExcelBySheetName<T>(string excelFile, string sheetName, List<IdentityError>? errors = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(excelFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(sheetName);
        if (!File.Exists(excelFile))
        {
            errors ??= new List<IdentityError>();
            errors.Add(new OperationErrorDescriber().FileNotExists(excelFile));
            return null;
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(excelFile))
        {
            var workbook = package.Workbook;
            var worksheets = workbook.Worksheets;
            var sheet = worksheets[sheetName];
            if (sheet == null)
            {
                errors ??= new List<IdentityError>();
                errors.Add(new OperationErrorDescriber().ExcelSheetNameNotExists(sheetName));
                return null;
            }
            return ReadToModels<T>(sheet, errors);
        }

    }

    private static List<T>? ReadToModels<T>(ExcelWorksheet sheet, List<IdentityError>? errors)
    {
        List<T> models = new List<T>();
        var columnCount = sheet.Dimension.End.Column;
        var rowCount = sheet.Dimension.End.Row;
        var headers = EntityHelper.GetModelDetails<T>().OrderBy(x => x.Order).ToList();
        if (columnCount != headers.Count)
        {
            errors ??= new List<IdentityError>();
            errors.Add(new OperationErrorDescriber().ExcelColumnCountError(sheet.Name));
            return null;
        }

        // 获取表头
        for (var i = 1; i <= columnCount; i++)
        {
            var title = sheet.Cells[1, i].Value?.ToString();
            if (string.IsNullOrWhiteSpace(title))
            {
                errors ??= new List<IdentityError>();
                errors.Add(new OperationErrorDescriber().ExcelSheetTitleInvalid(sheet.Name, i));
                return null;
            }
            var header = headers.FirstOrDefault(x => x.DisplayName == title);
            if (header == null)
            {
                errors ??= new List<IdentityError>();
                errors.Add(new OperationErrorDescriber().ExcelSheetTitleInvalid(sheet.Name, i));
                return null;
            }
            header.Order = i;
        }
        headers = headers.OrderBy(x => x.Order).ToList();
        for (var i = 2; i <= rowCount; i++)
        {
            var model = Activator.CreateInstance<T>()!;
            var type = model.GetType();
            foreach (var header in headers)
            {
                var value = sheet.Cells[i, header.Order].Value;
                if (value != null)
                {
                    var property = type.GetProperty(header.Name!)!;
                    EntityHelper.SetPropertyInfoValue<T>(model, property, value);
                }
            }
            models.Add(model);
        }
        return models;
    }
}
