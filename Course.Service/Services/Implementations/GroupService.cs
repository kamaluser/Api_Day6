﻿using Course.Core.Entities;
using Course.Data;
using Course.Data.Repostories.Interfaces;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Exceptions;
using Course.Service.Mappers;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _repository;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IGroupRepository repository, ILogger<GroupService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public int Create(GroupCreateDto dto)
        {
            _logger.LogInformation("Create method called with No: {No}", dto.No);

            if (_repository.Exists(x => x.No == dto.No && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "No", "No already taken");

            Group group = Mapper<GroupCreateDto, Group>.Map(dto);

            _repository.Add(group);
            _repository.Save();

            _logger.LogInformation("Group created with Id: {Id}", group.Id);
            return group.Id;
        }

        public List<GroupGetDto> GetAll(string? search = null)
        {
            _logger.LogInformation("GetAll method called");

            var result = _repository.GetAll(x => x.No.Contains(search)).Select(Mapper<Group, GroupGetDto>.Map).ToList();

            _logger.LogInformation("GetAll method completed");
            return result;
        }

        public void Edit(int id, GroupEditDto dto)
        {
            _logger.LogInformation("Edit method called for Id: {Id}", id);

            Group group = _repository.Get(x => x.Id == id && !x.IsDeleted);
            if (group == null)
            {
                _logger.LogWarning("Group not found with Id: {Id}", id);
                throw new RestException(StatusCodes.Status404NotFound, "Group not found by given Id!");
            }

            Mapper<GroupEditDto, Group>.Map(dto, group);

            _repository.Save();
            _logger.LogInformation("Group edited successfully for Id: {Id}", id);
        }

        public void Delete(int id)
        {
            _logger.LogInformation("Delete method called for Id: {Id}", id);

            Group group = _repository.Get(x=>x.Id == id && !x.IsDeleted);
            if (group == null)
            {
                _logger.LogWarning("Group not found with Id: {Id}", id);
                throw new RestException(StatusCodes.Status404NotFound, "Group not found by given Id!");
            }

            group.IsDeleted = true;
            _repository.Delete(group);
            _repository.Save();

            _logger.LogInformation("Group deleted successfully for Id: {Id}", id);
        }

        public GroupGetDto GetById(int id)
        {
            _logger.LogInformation("GetById method called for Id: {Id}", id);

            Group group = _repository.Get(x => x.Id == id && !x.IsDeleted);
            if (group == null)
            {
                _logger.LogWarning("Group not found with Id: {Id}", id);
                throw new RestException(StatusCodes.Status404NotFound, "Group not found by given Id!");
            }

            var result = Mapper<Group, GroupGetDto>.Map(group);

            _logger.LogInformation("GetById method completed for Id: {Id}", id);
            return result;
        }
    }
}