using DemoDrinkShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace DemoDrinkShop.Controllers
{
	[Authorize]
	public class AdminController : Controller
	{
		private IProductRepository repository;
		//private ISession _session;
		private const string ImageChangeKey = "imgChanged", ExtensionKey = "ext", ImageUnchanged = "imgUnchanged";

		public AdminController(IProductRepository repo)
		{
			repository = repo;
			
			//_session = httpContextAccessor.HttpContext.Session;
		}

		public ViewResult Index() => View(repository.Products);
		public ViewResult Edit(int productId)
		{
			Product currentProduct = repository.Products.FirstOrDefault(p => p.ProductID == productId);
			if (currentProduct.ImageURL != null)
			{
				HttpContext.Session.SetString(ImageUnchanged, currentProduct.ImageURL);
			}
			return View(currentProduct);
		}

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
				int? imgChangeFlag = HttpContext.Session.GetInt32(ImageChangeKey);
				if (imgChangeFlag.HasValue && Convert.ToBoolean(imgChangeFlag.Value))
				{
					string extension = HttpContext.Session.GetString(ExtensionKey);

					string pathToImgDir = Environment.CurrentDirectory + "\\wwwroot\\images",
						   
						   pathToTemp = pathToImgDir + "\\tempImg" + extension,
						   imageUrl = product.GetUniformImageNameWithoutExt() + extension,
						   pathToImg = pathToImgDir + "\\product\\" + imageUrl;

                    if (System.IO.File.Exists(pathToTemp))
					{
						System.IO.File.Move(pathToTemp, pathToImg, true);
						product.ImageURL = imageUrl;
					}
					else {
						System.IO.File.Delete(pathToImg);
						product.ImageURL = null;
					}
				}
				else{
					product.ImageURL = HttpContext.Session.GetString(ImageUnchanged);
				}

				repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} has been saved";

				HttpContext.Session.Clear();

                return RedirectToAction("Index");
            }
            else {
                return View(product);
            }
        }

		[HttpPost]
		public async Task<IActionResult> ProposeImage(IFormFile fileData)
		{
			if(fileData == null) { return BadRequest(); }

			HttpContext.Session.SetInt32(ImageChangeKey, 1);
			string ext = Path.GetExtension(fileData.FileName);

            HttpContext.Session.SetString(ExtensionKey, ext);
			string pathForJs = "\\images\\tempImg"+ext;
			string pathToTemp = Environment.CurrentDirectory + "\\wwwroot"+pathForJs;
			
			using(FileStream fs = new FileStream(pathToTemp, FileMode.OpenOrCreate))
			{
				await fileData.CopyToAsync(fs);
			}
			return Ok();
        }

		[HttpPost]
		public JsonResult CancelOrDeleteImage(Product product)
		{
			string delOptionStr = ControllerContext.HttpContext.Request.Form["shouldDelete"].First();
			bool delete = bool.Parse(delOptionStr);

			HttpContext.Session.SetInt32(ImageChangeKey, Convert.ToInt32(delete));
			string pathToTemp = Environment.CurrentDirectory + "\\wwwroot\\images\\tempImg";

			if (System.IO.File.Exists(pathToTemp))
			{
				System.IO.File.Delete(pathToTemp);
			}

			string initialUrl=null;
			if(!delete) 
			{
				initialUrl = HttpContext.Session.GetString(ImageUnchanged);
            }

			return Json(new { pathForJs = delete ? "\\images\\no_photo.jpg" : Product.GetDisplayImageSrc(initialUrl),
							  canRemove = product.ImageFileExists() });
        }

		public ViewResult Create() => View("Edit", new Product());

		[HttpPost]
		public IActionResult Delete(int productId)
		{
			Product deletedProduct = repository.DeleteProduct(productId);
			if (deletedProduct != null)
			{
				TempData["message"] = $"{deletedProduct.Name} was deleted";
			}
			return RedirectToAction("Index");
		}
	}
}
