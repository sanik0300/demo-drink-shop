using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace DemoDrinkShop.Models
{
	public enum Category : byte
	{
		Coffee, Tea, Milk
	}
    public class Product
    {
		static public ReadOnlyCollection<string> CategoryNames = new ReadOnlyCollection<string>(
			
			new string[] { "Coffee", "Tea", "Milk" }
		);

        public int ProductID { get; set; }

		[Required(ErrorMessage = "Please enter a product name")]
		public string? Name { get; set; }

		[Required(ErrorMessage = "Please enter a description")]
		public string? Description { get; set; }

        [Required]
		[DataType(DataType.Currency)]
		[Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "Please specify a category")]
		public Category? Category { get; set; }

		public string? ImageURL { get; set; }
		public bool ImageFileExists() => ImageFileExists(this.ImageURL);
		public static bool ImageFileExists(string initUrl)
		{
            if (initUrl == null) { return false; }

            return File.Exists(Environment.CurrentDirectory + "\\wwwroot\\images\\product\\" + initUrl);
        }
		public static string GetDisplayImageSrc(string initUrl)
		{
            if (initUrl == null)
            {
                return "/images/no_photo.jpg";
            }
            else if (ImageFileExists(initUrl))
            {
                return "/images/product/" + initUrl;
            }
            else
            {
                return "/images/not_found.jpg";
            }
        }
		public string GetDisplayImageSrc() => GetDisplayImageSrc(this.ImageURL);
		public string GetUniformImageNameWithoutExt() => $"id{ProductID}";
		
    }
}
