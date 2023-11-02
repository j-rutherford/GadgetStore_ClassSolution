using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GadgetStore.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;

namespace GadgetStore.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly GadgetStoreContext _context;

        public ProductsController(GadgetStoreContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            #region LINQ Notes
            /*
             * Language INtegrated Query (LINQ)
             * - LINQ is a standardized syntax/set of methods that allow us to operate on collections in C# just as we would
             * a SQL result set.  We can filter, group, select, etc...
             * 
             * SYNTAX EXAMPLE
             * var [filteredCollection] = _context.[Entity].Where([otf] => [otf].[Property] [[Condition]]).ToListAsync();
             * 
             * - filteredCollection --> the resulting collection once you filter out records you do not need
             * - Entity --> the specific DB table you are retrieving results from
             * - otf --> an on-the-fly variable created by you to represent a single Entity from the table
             * - Property --> the property from the Entity to be evaluated in the condition
             * - Condition --> your filter criteria
             * */
            #endregion
            var products = _context.Products
                .Where(p => !p.IsDiscontinued)// SELECT * FROM Products p WHERE p.IsDiscontinued <> 0
                .Include(p => p.Category)// JOIN Categories ON Category
                .Include(p => p.Supplier)// JOIN Suppliers ON Supplier
                .Include(p => p.OrderProducts);//JOIN OrderProducts 

            #region other linq examples
            //Eventually, filters like these will be applied by the user when we implement search/filter functionality

            //only show weapons
            //var products = _context.Products
            //    .Where(p => p.Category.CategoryName.ToLower().Contains("weapon"))// LIKE '%weapon%'
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            //Show products made by marvelous artifacts
            //var products = _context.Products
            //    .Where(p => p.Supplier.SupplierName.Contains("marvelous artifacts"))//LinqToEF is case insensitive
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            //Show products that are both out of stock and pending order.
            //var products = _context.Products
            //    .Where(p => p.UnitsInStock == 0 && p.UnitsOnOrder > 0)
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            //MINI LAB!
            ////Show products that cost more than $500
            //var products1 = _context.Products
            //    .Where(p => p.ProductPrice > 500)
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            ////Show all products that are a 'Tool' from 'United Federation of Planets'
            //var products2 = _context.Products
            //    .Where(p => p.Category.CategoryName.Contains("tool") && p.Supplier.SupplierName.Contains("united federation of planets"))
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);
            ////Show all products that contain the word 'star' in the name
            //var products3 = _context.Products
            //    .Where(p => p.ProductName.Contains("star"))
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);
            #endregion

            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> TiledProducts()
        {
            var products = _context.Products
                .Where(p => !p.IsDiscontinued)// SELECT * FROM Products p WHERE p.IsDiscontinued <> 0
                .Include(p => p.Category)// JOIN Categories ON Category
                .Include(p => p.Supplier)// JOIN Suppliers ON Supplier
                .Include(p => p.OrderProducts);//JOIN OrderProducts 

            return View(await products.ToListAsync());
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'GadgetStoreContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
