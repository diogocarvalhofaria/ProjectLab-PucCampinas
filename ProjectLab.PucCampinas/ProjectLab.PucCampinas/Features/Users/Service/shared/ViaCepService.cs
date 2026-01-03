using ProjectLab.PucCampinas.Features.Users.DTOs;

namespace ProjectLab.PucCampinas.Features.Users.Service.shared
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;

        public ViaCepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ViaCepResponse?> GetAddressByCep(string cep)
        {
            var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
            if (response.IsSuccessStatusCode)
            {
                var viaCepResponse = await response.Content.ReadFromJsonAsync<ViaCepResponse>();
                return viaCepResponse;
            }
            return null;
        }

    }
}
