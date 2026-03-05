using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using P2FixAnAppDotNetCode.Models;
using P2FixAnAppDotNetCode.Models.Services;

namespace P2FixAnAppDotNetCode.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICart _cart;
        private readonly IOrderService _orderService;
        private readonly IStringLocalizer<OrderController> _localizer;

        public OrderController(ICart pCart, IOrderService service, IStringLocalizer<OrderController> localizer)
        {
            _cart = pCart;
            _orderService = service;
            _localizer = localizer;
        }

        public ViewResult Index() => View(new Order());

        [HttpPost]
        public IActionResult Index([Bind]Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.Lines = (_cart as Cart)?.Lines.ToArray();
                    _orderService.SaveOrder(order);
                    return RedirectToAction(nameof(Completed));
                }
                return View(order);
            }
            catch
            {
                if (!((Cart)_cart).Lines.Any())
                {
                    ModelState.AddModelError("", _localizer["CartEmpty"]);
                }
                if (order.Name == null)
                {
                    ModelState.AddModelError("Name", _localizer["ErrorMissingName"]);
                }
                if (order.Address == null)
                {
                    ModelState.AddModelError("Adress", _localizer["ErrorMissingAddress"]);
                }
                if (order.City == null)
                {
                    ModelState.AddModelError("City", _localizer["ErrorMissingCity"]);
                }
                if (order.Country == null)
                {
                    ModelState.AddModelError("Country", _localizer["ErrorMissingCountry"]);
                }
                return View(order);
            }
        }

        public ViewResult Completed(Order order)
        {
            _cart.Clear();
            return View();
        }
    }
}
