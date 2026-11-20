
using System.Linq;
using BizLogic.BasketServices;
using BizLogic.Orders;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;


namespace BookApp.Controllers
{
    public class CheckoutController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public CheckoutController(EfCoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }

        public IActionResult Buy(OrderLineItem itemToBuy)
        {
            var cookie = new BasketCookie(
                HttpContext.Request.Cookies,
                HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(
                cookie.GetValue());
            service.AddLineItem(itemToBuy);
            var cookieOutString = service.EncodeForCookie();
            cookie.AddOrUpdateCookie(cookieOutString);
            SetupTraceInfo();
            return RedirectToAction("Index");
        }
        


        public IActionResult DeleteLineItem(int lineNum)
        {
            var cookie = new BasketCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());
            service.DeleteLineItem(lineNum);
            cookie.AddOrUpdateCookie(service.EncodeForCookie());
            SetupTraceInfo();
            return RedirectToAction("Index");
        }

        public IActionResult PlaceOrder(bool acceptTAndCs)
        {
            var service = new PlaceOrderService(HttpContext.Request.Cookies, HttpContext.Response.Cookies, _context);
            var orderId = service.PlaceOrder(acceptTAndCs);

            if (!service.Errors.Any())
                return RedirectToAction("ConfirmOrder", "Orders", new {orderId});

            foreach (var error in service.Errors)
            {
                var properties = error.MemberNames.ToList();
                ModelState.AddModelError(properties.Any() ? properties.First() : "", error.ErrorMessage);
            }

            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }
    }
}