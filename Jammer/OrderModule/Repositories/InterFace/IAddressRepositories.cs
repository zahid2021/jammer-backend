using Jammer.OrderModule.DTOs;
using Jammer.OrderModule.Models;
using Jammer.ProductModule.DTOs;

namespace Jammer.OrderModule.Repositories.InterFace
{
    public interface IAddressRepositories


    {
         Task<bool> AddAddress(AddAddressRequest request);
         Task<bool> DeleteAddress(int id);
        Task<bool> UpdateAddress(Address request);
        Task<List<Address>> GetAddress(string userid);
    }
}
