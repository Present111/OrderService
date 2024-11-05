using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderService.Controllers
{
    public class HomeController : Controller
    {
        private DBContextDataContext _context;
        public HomeController()
        {
            // Retrieve the connection string from Web.config
            var connectionString = ConfigurationManager.ConnectionStrings["SOADemoConnectionString"].ConnectionString;
            _context = new DBContextDataContext(connectionString);
        }
        public ActionResult ListOrder()
        {
            var products = _context.Orders.ToList();
            return View(products);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.InsertOnSubmit(order);
                _context.SubmitChanges();
                return RedirectToAction("ListOrder");
            }
            return View(order);
        }
        public ActionResult Edit(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.Products = new SelectList(_context.Products, "Id", "Name", order.ProductId);
            return View(order);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                var existingOrder = _context.Orders.SingleOrDefault(o => o.OrderId == order.OrderId);
                if (existingOrder == null)
                {
                    return HttpNotFound();
                }

                // Update order details
                existingOrder.ProductId = order.ProductId;
                existingOrder.Quantity = order.Quantity;
                existingOrder.Total = order.Quantity * _context.Products.SingleOrDefault(p => p.Id == order.ProductId)?.Price ?? 0;
                existingOrder.OrderDate = order.OrderDate;

                _context.SubmitChanges();
                return RedirectToAction("ListOrder");
            }

            ViewBag.Products = new SelectList(_context.Products, "Id", "Name", order.ProductId);
            return View(order);
        }
        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Home/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            _context.Orders.DeleteOnSubmit(order);
            _context.SubmitChanges();

            return RedirectToAction("ListOrder");
        }
    }
}