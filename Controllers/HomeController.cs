
using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.HelperExtensions;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.Logger;

namespace BookApp.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index
            (SortFilterPageOptions options)
        {
            var listService =
                new ListBooksService(_context);

            var bookList = await listService
                .SortFilterPage(options)
                .ToListAsync();

            SetupTraceInfo();

            return View(new BookListCombinedDto(options, bookList));
        }
        

        [HttpGet]
        public JsonResult GetFilterSearchContent
            (SortFilterPageOptions options)
        {
            var service = new
                BookFilterDropdownService(_context);

            var traceIdent = HttpContext.TraceIdentifier;

            return Json(
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                    traceIdent,
                    service.GetFilterDropDownValues(
                        options.FilterBy)));
        }
        


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            var isLocal = Request.IsLocal();
            return View(isLocal);
        }



        public IActionResult Error()
        {
            return View();
        }
    }
}