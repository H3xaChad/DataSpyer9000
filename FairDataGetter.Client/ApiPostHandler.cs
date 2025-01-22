using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using FairDataGetter.Client.Class;

namespace FairDataGetter.Client
{
    public class ApiPostHandler
    {
        private const string ApiUrl = "http://localhost:5019/api";

        /// <summary>
        /// Executes a POST request to the given API endpoint.
        /// </summary>
        private async Task<HttpResponseMessage> ExecutePostAsync(string endpoint, object data)
        {
            ArgumentNullException.ThrowIfNull(data);
            
            var camelCaseOption = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            Console.WriteLine($"Executing POST request to: {endpoint}");
            var jsonData = JsonSerializer.Serialize(data, camelCaseOption);
            Console.WriteLine($"Serialized Data: {jsonData}");

            using var httpClient = new HttpClient();
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(endpoint, content);
            Console.WriteLine($"POST request completed with response: {response.ReasonPhrase} | Code: {(int)response.StatusCode}");
            Console.WriteLine(response.Content.ToString());
            return response;
        }

        /// <summary>
        /// Sends an address to the API.
        /// </summary>
        public async Task<int> SendAddressAsync(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);

            const string endpoint = $"{ApiUrl}/Address";
            var addressData = new {
                address.Street,
                address.HouseNumber,
                address.City,
                address.PostalCode,
                address.Country
            };

            var response = await ExecutePostAsync(endpoint, addressData);
            return await ExtractIdFromResponseAsync(response, "Address");
        }

        /// <summary>
        /// Sends a company to the API.
        /// </summary>
        public async Task<int> SendCompanyAsync(Company company)
        {
            ArgumentNullException.ThrowIfNull(company?.Address, nameof(company));

            var addressId = await SendAddressAsync(company.Address);

            const string endpoint = $"{ApiUrl}/Company";
            var companyData = new {
                company.Name,
                AddressId = addressId
            };

            var response = await ExecutePostAsync(endpoint, companyData);
            return await ExtractIdFromResponseAsync(response, "Company");
        }

        /// <summary>
        /// Sends a customer to the API.
        /// </summary>
        public async Task<int> SendCustomerAsync(Customer customer, int? companyId = null)
        {
            ArgumentNullException.ThrowIfNull(customer, nameof(customer));
            
            var addressId = await SendAddressAsync(customer.Address);
            
            const string endpoint = $"{ApiUrl}/Customer";

            using var content = new MultipartFormDataContent();
            AddFormData(content, "FirstName", customer.FirstName);
            AddFormData(content, "LastName", customer.LastName);
            AddFormData(content, "Email", customer.Email);
            AddFormData(content, "AddressId", addressId.ToString());
            AddFormData(content, "InterestedProductGroups", JsonSerializer.Serialize(customer.InterestedProductGroups));

            if (companyId.HasValue)
                AddFormData(content, "CompanyId", companyId.Value.ToString());

            AddImageData(content, customer.ImageBase64, "image/png", "image.png");

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(endpoint, content);
            return await ExtractIdFromResponseAsync(response, "Customer");
        }

        /// <summary>
        /// Extracts the ID from a successful API response.
        /// </summary>
        private async Task<int> ExtractIdFromResponseAsync(HttpResponseMessage response, string entityName)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error creating {entityName}: {errorResponse}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var id = responseData.GetProperty("id").GetInt32();

            Console.WriteLine($"{entityName} POST request successful | id: {id}");
            return id;
        }

        /// <summary>
        /// Adds a simple form field to MultipartFormDataContent.
        /// </summary>
        private void AddFormData(MultipartFormDataContent content, string name, string value)
        {
            content.Add(new StringContent(value), name);
        }

        /// <summary>
        /// Adds an image binary stream to MultipartFormDataContent.
        /// </summary>
        private void AddImageData(MultipartFormDataContent content, string imageBase64, string mediaType, string fileName)
        {
            if (string.IsNullOrWhiteSpace(imageBase64)) {
                throw new ArgumentException("Image data cannot be null or empty.", nameof(imageBase64));
            }
            byte[] imageBytes;
            try {
                imageBytes = Convert.FromBase64String(imageBase64);
            } catch (FormatException ex) {
                throw new ArgumentException("Invalid Base64 string format.", nameof(imageBase64), ex);
            }
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            content.Add(imageContent, "Image", fileName);
        }
    }
}
