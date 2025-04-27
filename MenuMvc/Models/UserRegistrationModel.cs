using System.ComponentModel.DataAnnotations;

namespace MenuMvc.Models
{
    public class UserRegistrationModel
    {
        public string NameResEn { get; set; } = string.Empty;
        public string NameResAr { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new();
    }
    public class Category
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public int Id { get; set; }
        public string NameResEn { get; set; } = string.Empty;
        public string NameResAr { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty; public List<Category> Categories { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string NameResEn { get; set; } = string.Empty;
        public string NameResAr { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
    }
    public class ItemModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Name (English) is required.")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name (Arabic) is required.")]
        public string NameAr { get; set; } = string.Empty;

        public string Img { get; set; } = string.Empty;

        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
    }

    public class ItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemNameEn { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public decimal ItemPrice { get; set; }
        public string ItemImg { get; set; } = string.Empty;
    }


    public class AddCategoryForUserModel
    {
        public int UserId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
    }
    public class MenuData
    {
        public int UserId { get; set; }
        public string RestaurantNameEn { get; set; } = string.Empty;
        public string RestaurantNameAr { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public List<CategoryItem> Categories { get; set; } = new();
    }
    public class CategoryData
    {
        public int CategoryId { get; set; }

        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
    }
    public class CategoryItem
    {
        public CategoryModel Category { get; set; } = new();
        public List<ItemModel> Items { get; set; } = new();
    }
    public class DeleteCategoryRequest
    {
        public int CategoryId { get; set; }
        public int ItemId { get; set; }
    }

    public class Messages
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
