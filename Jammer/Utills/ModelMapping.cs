using AutoMapper;
using Jammer.ProductModule.DTOs;
using Jammer.ProductModule.Models;
using Jammer.UserModule.DTOs;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Jammer.Utills
{
    public class ModelMapping : Profile
    {
        public ModelMapping()
        {
            CreateMap<AddUserRequest,GetUserResponse >();
            CreateMap<AddUserRequestRole, GetUserResponse >();

            CreateMap< UpdateUserRequest, GetUpdateRequest> ();
            CreateMap<UpdateUserByAdminRequest, GetUpdateByAdminRequest> ();


            CreateMap<AddUserProductRequest, Product> ();
            CreateMap<UpdateProductRequestDTO, Product> ();
            CreateMap<UpdateCategory, UpdateCategoryDTO> ();
        }
    }
}

