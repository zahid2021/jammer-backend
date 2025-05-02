using System.ComponentModel.DataAnnotations;
namespace Jammer.OrderModule.DTOs
{
    public class AddAddressRequestDTO
    {
        

        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
    }
        public class AddAddressRequest
    {
        

        public string UserId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public  AddAddressRequest( AddAddressRequestDTO model,string userid) { 
            UserId = userid;
            City = model.City;
            Street = model.Street;
            PostalCode = model.PostalCode ;
            Region = model.Region ;

        }
    }
}
