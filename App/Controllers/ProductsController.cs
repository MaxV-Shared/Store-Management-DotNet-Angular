﻿using App.Controllers.Base;
using App.Infrastructures.Dbcontexts;
using App.Models.DTOs;
using App.Models.DTOs.Imports;
using App.Models.Entities;
using App.Services.Interface;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Controllers
{

    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly string _pathRootFolder;
        //public ProductsController(IProductService productService)
        //{
        //    _productService = productService;
        //}
        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            _pathRootFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
            _productService = productService;
            _context = context;
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ProductCreateRequest request)
        {
            await _productService.CreateAsync(request);
            return Ok();
        }
        [Route("import")]
        [HttpPost]
        public async Task<ActionResult> ImportProduct([FromForm] ProductImport productImport)
        {
            var files = HttpContext.Request.Form.Files;
            List<ProductImport> Product = new List<ProductImport>();
            //var fileName = "./Users.xlsx";
            foreach (var item in files)
            {
                if (item.Length > 0 && item != null)
                {
                    string file_name = Guid.NewGuid().ToString().Replace("-", "") + "_" + item.FileName;
                    string uploads = Path.Combine(_pathRootFolder, "files");
                    string urlPart = uploads + "/" + file_name;
                    string extension = Path.GetExtension(urlPart);
                    if (extension == ".xls" || extension == ".xlsx")
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (var stream = System.IO.File.Open(urlPart, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                do
                                {
                                    if (reader.Name == "Sheet1" || reader.Name == "page2")
                                    {
                                        while (reader.Read())
                                        {
                                            Product.Add(new ProductImport
                                            {
                                                Category = int.Parse(reader.GetValue(0).ToString()),
                                                Name = reader.GetValue(1).ToString(),
                                                Code = reader.GetValue(2).ToString(),
                                                Price = int.Parse(reader.GetValue(3).ToString()),
                                                PercentDiscount = double.Parse(reader.GetValue(4).ToString()),
                                                MaxDiscountPrice = double.Parse(reader.GetValue(5).ToString()),
                                            });
                                        }
                                    }
                                } while (reader.NextResult());
                            }
                        }
                    }
                }
            }
            return Ok(files);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ProductViewModel request)
        {
            if(id != request.Id)
                return BadRequest();
            var effectedCount = await _productService.UpdateAsync(id, request);
            if(effectedCount > 0)
                return Ok();
            return Accepted();
        }

        [HttpGet("")]
        public async Task<ActionResult> GetAll(string langId = "vi", string searchText = "")
        {
            //if (!_context.Functions.Any())
            //{
            //    _context.Functions.AddRange(new List<Function>
            //    {
            //        new Function {Id = "DASHBOARD", Name = "Thống kê", ParentId = null, SortOrder = 1,Url = "/dashboard",Icon="fa-dashboard" },

            //        new Function {Id = "CONTENT",Name = "Nội dung",ParentId = null,Url = "/contents",Icon="fa-table" },

            //        new Function {Id = "CONTENT_CATEGORY",Name = "Danh mục",ParentId ="CONTENT",Url = "/contents/categories"  },
            //        new Function {Id = "CONTENT_KNOWLEDGEBASE",Name = "Bài viết",ParentId = "CONTENT",SortOrder = 2,Url = "/contents/knowledge-bases",Icon="fa-edit" },
            //        new Function {Id = "CONTENT_COMMENT",Name = "Trang",ParentId = "CONTENT",SortOrder = 3,Url = "/contents/comments",Icon="fa-edit" },
            //        new Function {Id = "CONTENT_REPORT",Name = "Báo xấu",ParentId = "CONTENT",SortOrder = 3,Url = "/contents/reports",Icon="fa-edit" },

            //        new Function {Id = "STATISTIC",Name = "Thống kê", ParentId = null, Url = "/statistics",Icon="fa-bar-chart-o" },

            //        new Function {Id = "STATISTIC_MONTHLY_NEWMEMBER",Name = "Đăng ký từng tháng",ParentId = "STATISTIC",SortOrder = 1,Url = "/statistics/monthly-registers",Icon = "fa-wrench"},
            //        new Function {Id = "STATISTIC_MONTHLY_NEWKB",Name = "Bài đăng hàng tháng",ParentId = "STATISTIC",SortOrder = 2,Url = "/statistics/monthly-newkbs",Icon = "fa-wrench"},
            //        new Function {Id = "STATISTIC_MONTHLY_COMMENT",Name = "Comment theo tháng",ParentId = "STATISTIC",SortOrder = 3,Url = "/statistics/monthly-comments",Icon = "fa-wrench" },

            //        new Function {Id = "SYSTEM", Name = "Hệ thống", ParentId = null, Url = "/systems",Icon="fa-th-list" },

            //        new Function {Id = "SYSTEM_USER", Name = "Người dùng",ParentId = "SYSTEM",Url = "/systems/users",Icon="fa-desktop"},
            //        new Function {Id = "SYSTEM_ROLE", Name = "Nhóm quyền",ParentId = "SYSTEM",Url = "/systems/roles",Icon="fa-desktop"},
            //        new Function {Id = "SYSTEM_FUNCTION", Name = "Chức năng",ParentId = "SYSTEM",Url = "/systems/functions",Icon="fa-desktop"},
            //        new Function {Id = "SYSTEM_PERMISSION", Name = "Quyền hạn",ParentId = "SYSTEM",Url = "/systems/permissions",Icon="fa-desktop"},
            //    });
            //    await _context.SaveChangesAsync();
            //}
            return Ok(await _productService.GetAllDTOAsync(langId, searchText));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);

            if (result != null)
                return Ok(result);
            return NotFound();
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetPaging(int pageIndex, int pageSize, string langId, string searchText = "")
        {
            var result = await _productService.GetPagingAsync(langId, pageIndex, pageSize, searchText);
            return Ok(result);
        }
    }
}
