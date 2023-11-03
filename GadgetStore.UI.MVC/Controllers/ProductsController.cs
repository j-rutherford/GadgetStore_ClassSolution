using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GadgetStore.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;//added for file upload
using GadgetStore.UI.MVC.Utilities;//added for image resize utility

namespace GadgetStore.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly GadgetStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(GadgetStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product) //added Image to the Bind properties
        {
            if (ModelState.IsValid)
            {
                #region File Upload - CREATE
                if (product.Image == null)
                {
                    product.ProductImage = "noimage.png";
                    //goto end;//a new form of flow control. jumps to the section with the name "end:"
                }
                else
                {
                    //else, a file was uploaded
                    //check the file type
                    string ext = Path.GetExtension(product.Image.FileName);
                    List<string> validExt = new() { ".jpeg", ".jpg", ".gif", ".png" };
                    // verfiy the uploaded file has an extension matching one of the valid extensions
                    if (validExt.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
                        //Generate a unique filename
                        product.ProductImage = Guid.NewGuid() + ext;

                        //save the image file to the web server
                        //path to webroot
                        string webrootPath = _webHostEnvironment.WebRootPath;
                        string fullImagePath = webrootPath + "/images/";

                        //Create a MemoryStream to read the image into the server memory
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);//transferring from object to server memory
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //only do this if you're uploading images.
                                //Image utility 
                                //1) (int) max image size
                                //2) (int) max thumbnail size
                                //3) (string) full path
                                //4) (Image) an image
                                //5) (string) filename
                                int maxImage = 500;//in pixels
                                int maxThumbSize = 100;//in pixels
                                ImageUtilities.ResizeImage(fullImagePath, product.ProductImage, img, maxImage, maxThumbSize);
                                //for NOT images:
                                //fileName.Save("path/to/folder","filename");
                            }
                        }
                    }
                }
                #endregion
                //end:
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
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product) //added Image to Bind list
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region File Upload - Edit
                //save the old image name so we can delete if necessary
                string? oldImageName = product.ProductImage;
                if (product.Image != null)
                {
                    string ext = Path.GetExtension(product.Image.FileName);
                    //list valid extensions:
                    List<string> validExts = new() { ".jpeg", ".jpg", ".png", ".gif" };

                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
                        //Get new image name
                        product.ProductImage = Guid.NewGuid() + ext;
                        //get the full path
                        string fullPath = _webHostEnvironment.WebRootPath + "/images/";
                        //delete the old image
                        if (oldImageName != null && !oldImageName.ToLower().StartsWith("noimage"))
                        {
                            ImageUtilities.Delete(fullPath, oldImageName);
                        }
                        //save the new image
                        using var memoryStream = new MemoryStream();
                        await product.Image.CopyToAsync(memoryStream);
                        using var img = Image.FromStream(memoryStream);
                        int maxImgSize = 500;
                        int maxThumbSize = 100;
                        ImageUtilities.ResizeImage(fullPath, product.ProductImage, img, maxImgSize, maxThumbSize);

                    }
                }
                #endregion

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
                //get the file name
                string? oldImageName = product.ProductImage;
                string fullPath = _webHostEnvironment.WebRootPath + "/images/";
                if (oldImageName != null && !oldImageName.ToLower().Contains("noimage"))
                {
                    ImageUtilities.Delete(fullPath, oldImageName);
                }
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
