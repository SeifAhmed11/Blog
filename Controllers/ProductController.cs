using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;

namespace Blog.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null || productDto.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            else
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var imageFileExtension = Path.GetExtension(productDto.ImageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(imageFileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Invalid image file type. Only JPG, JPEG, and PNG are allowed.");
                }

                // Validate file size (e.g., max 5MB)
                if (productDto.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File size exceeds the 5MB limit.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // Save image file to server
            var originalFileName = Path.GetFileNameWithoutExtension(productDto.ImageFile.FileName);
            var fileExtension = Path.GetExtension(productDto.ImageFile.FileName);
            var uniqueFileName = $"{originalFileName}_{Guid.NewGuid()}{fileExtension}";

            // Concatenate path strings
            var filePath = Path.Combine(_env.WebRootPath, "products", uniqueFileName);

            // Ensure directory exists
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // Save product information
            var product = new Product
            {
                Name = productDto.Name,
                Category = productDto.Category,
                Description = productDto.Description,
                CreateAt = DateTime.Now,
                ImageFileName = uniqueFileName // Save the path or filename of the uploaded image
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Category = product.Category,
                Description = product.Description

            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreateAt.ToString("MM/dd/YYYY");
            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Update product details
            product.Name = productDto.Name;
            product.Category = productDto.Category;
            product.Description = productDto.Description;

            if (productDto.ImageFile != null && productDto.ImageFile.Length > 0)
            {
                // Validate and save image file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var imageFileExtension = Path.GetExtension(productDto.ImageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(imageFileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Invalid image file type. Only JPG, JPEG, and PNG are allowed.");
                    return View(productDto);
                }

                if (productDto.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File size exceeds the 5MB limit.");
                    return View(productDto);
                }

                // Save new image file
                var originalFileName = Path.GetFileNameWithoutExtension(productDto.ImageFile.FileName);
                var fileExtension = Path.GetExtension(productDto.ImageFile.FileName);
                var uniqueFileName = $"{originalFileName}_{Guid.NewGuid()}{fileExtension}";
                var filePath = _env.WebRootPath + Path.DirectorySeparatorChar + "products" + Path.DirectorySeparatorChar + uniqueFileName;

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // Update the product image filename
                product.ImageFileName = uniqueFileName;
            }

            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Delete the image file if it exists
            if (!string.IsNullOrEmpty(product.ImageFileName))
            {
                var filePath = Path.Combine(_env.WebRootPath, "products", product.ImageFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
