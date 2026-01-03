using ProjectLab.PucCampinas.Features.Users.DTOs;

namespace ProjectLab.PucCampinas.Features.Users.Service.shared
{
    public interface IViaCepService
    {
        Task<ViaCepResponse?> GetAddressByCep(string cep);
    }
}
