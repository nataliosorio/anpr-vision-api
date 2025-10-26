using AutoMapper;
using Business.Interfaces.Security;
using Data.Implementations.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations.Security
{
    public class PersonBusiness : RepositoryBusiness<Person, PersonDto>, IPersonBusiness
    {
        private readonly IPersonData _data;
        private readonly IMapper _mapper;
        public PersonBusiness(IPersonData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;

        }

        public async Task<IEnumerable<PersonDto>> GetAllByParkingAsync()
        {
            var persons = await _data.GetAllByParkingAsync();
            return _mapper.Map<IEnumerable<PersonDto>>(persons);
        }
    }
}
