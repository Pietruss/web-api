using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.Exceptions;
using app1API.Models;
using app1API.Models.Owner;
using app1API.Services.Owner;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace app1API.Services
{
    public class OwnerService: IOwnerService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<OwnerService> _logger;

        public OwnerService(RestaurantDbContext dbContext, IMapper mapper, ILogger<OwnerService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public IEnumerable<OwnerDto> GetAll()
        {
            var owners = _dbContext.Owners.ToList();

            return _mapper.Map<List<OwnerDto>>(owners);
        }


        public OwnerDto Get(int Id)
        {
            var owner = _dbContext.Owners.FirstOrDefault(x => x.Id == Id);

            return owner == null ? throw new NotFoundException("Owner not found") : _mapper.Map<OwnerDto>(owner);
        }

        public int Create(CreateOwnerDto createOwner)
        {
            var owner = _mapper.Map<Entities.Owner>(createOwner);
            _dbContext.Owners.Add(owner);
            _dbContext.SaveChanges();

            return owner.Id;
        }

        public void Delete(int id)
        {
            _logger.LogWarning($"Owner with id: {id} DELETE action invoked.");
            
            var owner = _dbContext.Owners.FirstOrDefault(x => x.Id == id);

            if(owner is null)
                throw new NotFoundException("Owner not found");

            _dbContext.Owners.Remove(owner);
            _dbContext.SaveChanges();
        }

        public void Update(UpdateOwnerDto updateOwner, int id)
        {
            var owner = _dbContext.Owners.FirstOrDefault(x => x.Id == id);

            if(owner is null)
                throw new NotFoundException($"Owner with id: {id} not found.");

            owner.Name = updateOwner.Name;

            _dbContext.SaveChanges();
        }
    }
}
