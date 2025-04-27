using MenuMvc.Models;
using MenuMvc.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly ApiService _apiService;

    public HomeController(ApiService apiService)
    {
        _apiService = apiService;
    }

    #region :: Login

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid input.";
            return View(model);
        }

        var loginResponse = await _apiService.LoginAsync(model);
        if (loginResponse == null)
        {
            TempData["ErrorMessage"] = "Invalid username or password.";
            return View(model);
        }

        // Store user details in session
        HttpContext.Session.SetInt32("UserId", loginResponse.Id);
        HttpContext.Session.SetString("UserName", loginResponse.NameResEn);
        HttpContext.Session.SetString("UserLogo", loginResponse.Logo);
        HttpContext.Session.SetString("UserQRCode", loginResponse.QRCode);

       
        return RedirectToAction("Dashboard");
    }

    #endregion

    #region :: Register

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid input.";
            return View(model);
        }

        var loginResponse = await _apiService.RegisterUserAsync(model);
        if (loginResponse == null)
        {
            TempData["ErrorMessage"] = "Invalid username or password.";
            return View(model);
        }

        


        return RedirectToAction("Login");
    }

    #endregion

    #region :: Dashboard

    [HttpGet]
    public IActionResult Dashboard()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        ViewBag.UserId = userId;
        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserLogo = HttpContext.Session.GetString("UserLogo");

        return View();
    }

    #endregion

    #region :: Categories

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        var categories = await _apiService.GetCategoriesByUserAsync(userId.Value);
        return Json(categories);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryForUserModel model)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        model.UserId = userId.Value;
        bool isSuccess = await _apiService.AddCategoryForUserAsync(model);
        if (isSuccess)
        {
            TempData["Message"] = "Category added successfully!";
            return RedirectToAction("Dashboard");
        }

        ModelState.AddModelError("", "Failed to add category.");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> EditCategory([FromBody] Category model)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        bool isSuccess = await _apiService.EditCategory(model);
        if (isSuccess)
        {
            TempData["Message"] = "Category Edit successfully!";
            return RedirectToAction("Dashboard");
        }

        ModelState.AddModelError("", "Failed to add category.");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        bool isSuccess = await _apiService.DeleteCategoryAsync(categoryId, userId.Value);
        if (isSuccess)
        {
            return Ok(new { Message = "Category deleted successfully." });
        }

        return BadRequest("Failed to delete category.");
    }

    #endregion

    #region :: Items

    [HttpGet]
    public async Task<IActionResult> Item(int categoryId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> AddItem(int categoryId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        var categoryIdsJson = HttpContext.Session.GetString("UserCategoryIds");
        if (string.IsNullOrEmpty(categoryIdsJson))
        {
            TempData["ErrorMessage"] = "No categories found for the user.";
            return RedirectToAction("Dashboard");
        }

        var userCategoryIds = JsonSerializer.Deserialize<List<int>>(categoryIdsJson);
        var allCategories = await _apiService.GetAllCategoriesAsync();
        var userCategories = allCategories.Where(c => userCategoryIds.Contains(c.Id)).ToList();

        ViewBag.Categories = userCategories;
        ViewBag.CategoryId = categoryId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromForm] ItemModel model, IFormFile? imgFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        model.UserId = userId.Value;

        if (imgFile != null && imgFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imgFile.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());
                model.Img = base64Image;
            }
        }

        bool isSuccess = await _apiService.AddItemAsync(model);
        if (isSuccess)
        {
            TempData["Message"] = "Item added successfully!";
            return RedirectToAction("Dashboard");
        }

        ModelState.AddModelError("", "Failed to add item.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditItem( [FromBody] ItemViewModel model)
    {
        

        var itemToUpdate = await _apiService.EditItemAsync(model);
        if (!itemToUpdate)
        {
            return null;

        }
        return Ok(new { message = "Item updated successfully", item = itemToUpdate });




    }


    [HttpPost]
    public async Task<IActionResult> DeleteItem([FromBody] DeleteCategoryRequest deleteCategoryRequest)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        bool isSuccess = await _apiService.DeleteItemAsync(deleteCategoryRequest);
        if (isSuccess)
        {
            return Ok(new { Message = "Item deleted successfully." });
        }

        return BadRequest("Failed to delete item.");
    }
    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        var items = await _apiService.GetItemsByCategoryAsync(categoryId, userId.Value);
        return Json(items);
    }

    #endregion

    #region :: Message

    [HttpGet]
    public IActionResult Messages()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User not logged in.");
        }

        var items = await _apiService.GetMessages(userId.Value);
        return Json(items);
    }
    #endregion
}