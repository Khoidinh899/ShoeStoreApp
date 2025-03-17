using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using System.Collections.Generic;

namespace ShoeStoreApp.Services
{
    public abstract class ExcelExporter
    {
        protected ShoeStoreAppContext _context;
        protected ExcelService _excelService;
        protected ExcelExporter(ShoeStoreAppContext context)
        {
            _context = context;
            _excelService = ExcelService.GetInstance();
        }
        public abstract object getData();
        public abstract byte[] ExportFromDataToExcel(object data);
        public async Task<FileStreamResult> CreateExcelFile(
            byte[] data, ControllerBase controller, string fileName)
        {
            var memoryStream = new MemoryStream(data);
            return controller.File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public Task<FileStreamResult> ExportDataToExcel(
            ControllerBase controller, string fileName)
        {
            object data = getData();
            byte[] dataToByteArray = ExportFromDataToExcel(data);
            return CreateExcelFile(dataToByteArray, controller, fileName);
        }
    }
}
