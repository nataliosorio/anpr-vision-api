using AutoMapper;
using Entity.Dtos.Operational;
using Entity.Dtos.Parameter;
using Entity.Dtos.Security;
using Entity.Models.Operational;
using Entity.Models.Parameter;
using Entity.Models.Security;
using System;
using System.Security;
using Utilities.Interfaces;


namespace Utilities.Implementations
{
    public class AutoMapperProfiles : Profile
    {
        

        public AutoMapperProfiles() : base()
        {
         

            // Ejemplo de mapeo
            CreateMap<Form, FormDto>();
            CreateMap<FormDto, Form>();

            CreateMap<Module, ModuleDto>();
            CreateMap<ModuleDto, Module>();

            CreateMap<FormModule, FormModuleDto>();
            CreateMap<FormModuleDto, FormModule>();

            CreateMap<Module, ModuleDto>();
            CreateMap<ModuleDto, Module>();

            CreateMap<Permission, PermissionDto>();
            CreateMap<PermissionDto, Permission>();

            CreateMap<Person, PersonDto>();
            CreateMap<PersonDto, Person>();


            CreateMap<Rol, RolDto>();
            CreateMap<RolDto, Rol>();

            CreateMap<RolFormPermission, RolFormPermissionDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
                .ForMember(dest => dest.RolName, opt => opt.MapFrom(src => src.Rol.Name))
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name));
            CreateMap<RolFormPermissionDto, RolFormPermission>();

            //CreateMap<RolUser, RolUserDto>();

            CreateMap<RolParkingUser, RolParkingUserDto>()
                        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                        .ForMember(dest => dest.RolName, opt => opt.MapFrom(src => src.Rol.Name));

            CreateMap<RolParkingUserDto, RolParkingUser>();

            CreateMap<User, UserDto>()
                 .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Person.FirstName));

            //CreateMap<Client, ClientDto>()
            //    .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person.FirstName));
            //CreateMap<UserDto, User>()
            //.ForPath(dest => dest.Person.Firstname, opt => opt.MapFrom(src => src.PersonName));

            CreateMap<ClientDto, Client>()
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.Person, opt => opt.Ignore());

            CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person.FirstName));

            CreateMap<BlackListDto, BlackList>()
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
                .ForMember(dest => dest.Vehicle, opt => opt.Ignore());

            CreateMap<BlackList, BlackListDto>()
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle.Plate));


            CreateMap<CameraDto, Camera>()
                .ForMember(dest => dest.ParkingId, opt => opt.MapFrom(src => src.ParkingId))
                .ForMember(dest => dest.Parking, opt => opt.Ignore());

            CreateMap<Camera, CameraDto>()
               .ForMember(dest => dest.ParkingId, opt => opt.MapFrom(src => src.ParkingId))
               .ForMember(dest => dest.Parking, opt => opt.MapFrom(src => src.Parking.Name));

            CreateMap<RatesDto, Rates>()
             .ForMember(dest => dest.ParkingId, opt => opt.MapFrom(src => src.ParkingId))
             .ForMember(dest => dest.RatesTypeId, opt => opt.MapFrom(src => src.RatesTypeId))
             .ForMember(dest => dest.TypeVehicleId, opt => opt.MapFrom(src => src.TypeVehicleId))
             .ForMember(dest => dest.Parking, opt => opt.Ignore())
             .ForMember(dest => dest.RatesType, opt => opt.Ignore())
             .ForMember(dest => dest.TypeVehicle, opt => opt.Ignore());


            CreateMap<Rates, RatesDto>()
             .ForMember(dest => dest.ParkingId, opt => opt.MapFrom(src => src.ParkingId))
             .ForMember(dest => dest.RatesTypeId, opt => opt.MapFrom(src => src.RatesTypeId))
             .ForMember(dest => dest.TypeVehicleId, opt => opt.MapFrom(src => src.TypeVehicleId))
             .ForMember(dest => dest.Parking, opt => opt.MapFrom(src => src.Parking.Name))
             .ForMember(dest => dest.RatesType, opt => opt.MapFrom(src => src.RatesType.Name))
             .ForMember(dest => dest.TypeVehicle, opt => opt.MapFrom(src => src.TypeVehicle.Name));


            CreateMap<MembershipsDto, Memberships>()
            .ForMember(dest => dest.MembershipTypeId, opt => opt.MapFrom(src => src.MembershipTypeId))
            .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
            .ForMember(dest => dest.MembershipType, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore());


            CreateMap<Memberships, MembershipsDto>()
             .ForMember(dest => dest.MembershipTypeId, opt => opt.MapFrom(src => src.MembershipTypeId))
             .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId))
             .ForMember(dest => dest.MembershipType, opt => opt.MapFrom(src => src.MembershipType.Name))
             .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle.Plate));

            CreateMap<ParkingDto, Parking>()
               .ForMember(dest => dest.ParkingCategoryId, opt => opt.MapFrom(src => src.ParkingCategoryId))
               .ForMember(dest => dest.ParkingCategory, opt => opt.Ignore());

            CreateMap<Parking, ParkingDto>()
               .ForMember(dest => dest.ParkingCategoryId, opt => opt.MapFrom(src => src.ParkingCategoryId))
               .ForMember(dest => dest.ParkingCategory, opt => opt.MapFrom(src => src.ParkingCategory.Name));





            CreateMap<UserDto, User>();
            CreateMap<User, UserResponseDto>().ReverseMap();

            //CreateMap<BlackList, BlackListDto>().ReverseMap();
            //CreateMap<Client, ClientDto>().ReverseMap();
            CreateMap<Memberships, MembershipsDto>().ReverseMap();
            CreateMap<MemberShipType, MemberShipTypeDto>().ReverseMap();
            //CreateMap<Parking, ParkingDto>().ReverseMap();
            CreateMap<ParkingCategory, ParkingCategoryDto>().ReverseMap();
            //CreateMap<Rates, RatesDto>().ReverseMap();
            CreateMap<RatesType, RatesTypeDto>().ReverseMap();
            CreateMap<RegisteredVehicles, RegisteredVehiclesDto>().ReverseMap();
            CreateMap<Sectors, SectorsDto>().ReverseMap();
            CreateMap<Slots, SlotsDto>().ReverseMap();
            CreateMap<TypeVehicle, TypeVehicleDto>().ReverseMap();
            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<Zones, ZonesDto>().ReverseMap();
            //CreateMap<PersonParking, PersonParkingDto>();
            //CreateMap<PersonParkingDto, PersonParking>();
            //CreateMap<ClientDto, Client>().ReverseMap();

            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<NotificationDto, Notification>().ReverseMap();
            // Mapeo para manual-entry: RegisteredVehiclesDto -> ManualEntryResponseDto
            CreateMap<RegisteredVehiclesDto, ManualEntryResponseDto>()
                .ForMember(dest => dest.TicketPdfBytes, opt => opt.Ignore());


        }
    }
}
