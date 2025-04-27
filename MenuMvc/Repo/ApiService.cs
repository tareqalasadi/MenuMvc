using MenuMvc.Models;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace MenuMvc.Repo
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        #region :: Users

        public async Task<LoginResponse> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + "Login", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            return null;
        }

        public async Task<bool> RegisterUserAsync(UserRegistrationModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + "AddUser", model);
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region :: Categories

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "Category");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Category>>() ?? new List<Category>();
        }

        public async Task<List<CategoryData>> GetCategoriesByUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl + "GetCategoriesByUser/" + userId);
                response.EnsureSuccessStatusCode();

                var menuData = await response.Content.ReadFromJsonAsync<List<CategoryData>>();
                return menuData;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> AddCategoryForUserAsync(AddCategoryForUserModel model)
        {
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "AddCategoryForUser", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> EditCategory(Category model)
        {
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "EditCategory", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId, int userId)
        {
            var model = new
            {
                CategoryId = categoryId, 
                userId = userId         
            };

            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl + "DeleteCategory", content);

            return response.IsSuccessStatusCode;
        }

        #endregion

        #region :: Items

        public async Task<List<ItemModel>> GetItemsByUserCategoryAsync(int categoryId, int userId)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "GetCategoryAndItemsByUser/" + userId);
            response.EnsureSuccessStatusCode();

            var menuData = await response.Content.ReadFromJsonAsync<MenuData>();
            var category = menuData?.Categories.FirstOrDefault(c => c.Category.CategoryId == categoryId);
            return category?.Items ?? new List<ItemModel>();
        }

        public async Task<List<ItemViewModel>> GetItemsByCategoryAsync(int categoryId, int userId)
        {
            var response = await _httpClient.GetAsync(_baseUrl+"GetItemsByCategory/"+categoryId+"/"+userId);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ItemViewModel>>() ?? new List<ItemViewModel>();
            }
            return new List<ItemViewModel>();
        }

        public async Task<bool> AddItemAsync(ItemModel model)
        {
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "AddItem", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(DeleteCategoryRequest deleteCategoryRequest)
        {
            var jsonContent = JsonConvert.SerializeObject(deleteCategoryRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "DeleteItem", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> EditItemAsync(ItemViewModel model)
        {
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "EditItem", content);
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region :: Messages
        public async Task<List<Messages>> GetMessages( int userId)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "GetMessages/" +  userId);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Messages>>() ?? new List<Messages>();
            }
            return new List<Messages>();
        }
        #endregion
    }
}
