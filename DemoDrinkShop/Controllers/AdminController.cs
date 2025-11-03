using DemoDrinkShop.Infrastructure;
using DemoDrinkShop.Models;
using DemoDrinkShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace DemoDrinkShop.Controllers
{
	[Authorize]
	public class AdminController : Controller
	{
		private const string noPhotoPath = "\\images\\no_photo.jpg",
							 notFoundPath = "/images/not_found.jpg",
                             contentTypeKey = "content", ImageChangeKey = "imgChanged",
							 encodingPrefix = "base64,";
		
		private readonly IProductRepository repository;
		private readonly IImageStorageService _imagesService;
		private static MemoryStream DecodeBytesFromDataUrl(string dataUrl)
		{
			string bytesInString = dataUrl.Split(',')[1];
			return new MemoryStream(Convert.FromBase64String(bytesInString));
		}
		private async Task<ProductWrapperViewModel> GetProductWrapper(Product product)
		{
            if (product.ImageURL == null)
            {
                return new ProductWrapperViewModel() { Product = product, ImageSrc = noPhotoPath };
            }
            bool exists = await _imagesService.ImageExists(product.ImageURL);
			
			return new ProductWrapperViewModel()
			{
				Product = product,
				ImageExists = exists,
				ImageSrc = exists ? _imagesService.GetEndURL(product.ImageURL) : notFoundPath
			};
        }
		
		public AdminController(IProductRepository repo, IImageStorageService storageService)
		{
			repository = repo;
			_imagesService = storageService;
		}

		public ViewResult Index() 
			=> View(repository.Products.Select(pr => GetProductWrapper(pr).Result).ToList());
		
		public async Task<ViewResult> Edit(int productId)
		{
			Product currentProduct = repository.Products.FirstOrDefault(p => p.ProductID == productId);
			return View(await GetProductWrapper(currentProduct));
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Product product, string dataUrl)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			bool imageWasChanged = Convert.ToBoolean(HttpContext.Session.GetInt32(ImageChangeKey) ?? 0);

			if(imageWasChanged && dataUrl.Contains(encodingPrefix)) 
			{
				if(product.ImageURL != null &&  await _imagesService.ImageExists(product.ImageURL))
				{
					await _imagesService.DeleteImageFromDrive(product.ImageURL);
				}

				using(MemoryStream ms = DecodeBytesFromDataUrl(dataUrl))
				{
					string ctType = HttpContext.Session.GetString(contentTypeKey) ?? "image/*";

					string name = $"product_image_{Guid.NewGuid()}";

					string urlResult = await _imagesService.LoadProductImageToDrive(name, ctType, ms);
					product.ImageURL = urlResult;
                }
			}
            else if (imageWasChanged && dataUrl.EndsWith(Path.GetFileName(noPhotoPath)))
            {
                product.ImageURL = null;
            }

            repository.SaveProduct(product);
			TempData["message"] = $"{product.Name} has been saved";

			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult ProposeImage([FromForm] string contentTypeString)
		{
			if(!ModelState.IsValid) { return BadRequest(); }

            HttpContext.Session.SetInt32(ImageChangeKey, 1);
            HttpContext.Session.SetString(contentTypeKey, contentTypeString);
			return Ok();
        }

		[HttpPost]
		public async Task<JsonResult> CancelOrDeleteImage(Product product, bool shouldDelete)
		{
			if(!shouldDelete)
			{
				HttpContext.Session.Remove(ImageChangeKey);
			}

			bool exists = product.ImageURL!=null && await _imagesService.ImageExists(product.ImageURL);

			string resultPathForJs;
			if (shouldDelete || product.ImageURL == null) 
			{
				resultPathForJs = noPhotoPath;
			}
			else {
				resultPathForJs = exists? _imagesService.GetEndURL(product.ImageURL) : notFoundPath;
			}

            if (shouldDelete && product.ImageURL != null)
            {
				if (exists) {
					await _imagesService.DeleteImageFromDrive(product.ImageURL);
				}
				product.ImageURL = null;
            }

			return Json(new { pathForJs = resultPathForJs, canRemove = !shouldDelete && exists });
        }

		public ViewResult Create() => View("Edit", new Product());


		[HttpPost]
		public async Task<IActionResult> Delete(int productId)
		{
			Product deletedProduct = repository.DeleteProduct(productId);
			if (deletedProduct != null)
			{
				HttpContext.Session.Remove(ImageChangeKey);
				TempData["message"] = $"{deletedProduct.Name} was deleted";

				if(deletedProduct.ImageURL!=null && await _imagesService.ImageExists(deletedProduct.ImageURL))
				{
					await _imagesService.DeleteImageFromDrive(deletedProduct.ImageURL);
				}
			}

			return RedirectToAction("Index");
		}
	}
}
