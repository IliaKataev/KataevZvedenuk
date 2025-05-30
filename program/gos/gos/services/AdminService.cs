using gos.models.DTO;
using gos.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.repositories;

namespace gos.services
{
    public interface IAdminService
    {
        Task<List<ServiceDTO>> GetAllServicesAsync();
        Task<Service> CreateServiceAsync(ServiceDTO serviceDTO);
        Task<Service> UpdateServiceAsync(ServiceDTO serviceDTO);
        Task<List<RuleDTO>> GetRulesForServiceAsync(int serviceId);
        Task AddRuleAsync(RuleDTO ruleDTO);
        Task UpdateRuleAsync(RuleDTO ruleDTO);
        Task DeleteRuleAsync(int ruleId);
        Task<List<ParameterTypeDTO>> GetParameterTypesAsync();
        Task<ParameterType> CreateParameterTypeAsync(ParameterTypeDTO parameterTypeDTO);
        Task DeleteParameterTypeAsync(int typeId);
        Task<User> CreateUserAsync(UserDTO userDTO);
        Task ReplaceAllParameterTypesAsync(List<ParameterTypeDTO> updatedDtos);
        Task ReplaceAllServicesAsync(List<ServiceDTO> updatedDtos);

    }

    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IParameterTypeRepository _parameterTypeRepository;
        private readonly AuthSession _authSession;

        public AdminService(
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IRuleRepository ruleRepository,
            IParameterTypeRepository parameterTypeRepository,
            AuthSession authSession)
        {
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _ruleRepository = ruleRepository;
            _parameterTypeRepository = parameterTypeRepository;
            _authSession = authSession;
        }

        public async Task<List<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return services
                .Select(pt => new ServiceDTO
                {
                    Id = pt.Id,
                    Description = pt.Description,
                    Name = pt.Name,
                    ActivationDate = pt.ActivationDate,
                    DeactivationDate = pt.DeactivationDate,
                })
                .ToList();
        }

        public async Task<Service> CreateServiceAsync(ServiceDTO serviceDTO)
        {
            var service = new Service
            {
                Name = serviceDTO.Name,
                Description = serviceDTO.Description,
                ActivationDate = serviceDTO.ActivationDate
            };

            await _serviceRepository.AddAsync(service);
            return service;
        }

        public async Task<Service> UpdateServiceAsync(ServiceDTO serviceDTO)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceDTO.Id);
            if (service == null) throw new ArgumentException("Service not found.");

            service.Name = serviceDTO.Name;
            service.Description = serviceDTO.Description;
            service.ActivationDate = serviceDTO.ActivationDate;
            service.DeactivationDate = serviceDTO.DeactivationDate;

            await _serviceRepository.UpdateAsync(service);
            return service;
        }

        public async Task<List<RuleDTO>> GetRulesForServiceAsync(int serviceId)
        {
            var rules = await _ruleRepository.GetByServiceIdAsync(serviceId);

            return rules
                .Select(pt => new RuleDTO
                {
                    Id = pt.Id,
                    ServiceId = serviceId,
                    Value = pt.Value,
                    ComparisonOperator = pt.ComparisonOperator,
                    NeededTypeId = pt.NeededTypeId,
                    DeadlineDays = pt.DeadlineDays
                })
                .ToList();

        }


        public async Task AddRuleAsync(RuleDTO ruleDTO)
        {
            var rule = new Rule
            {
                ServiceId = ruleDTO.ServiceId,
                Value = ruleDTO.Value,
                ComparisonOperator = ruleDTO.ComparisonOperator,
                NeededTypeId = ruleDTO.NeededTypeId,
                DeadlineDays = ruleDTO.DeadlineDays
            };

            await _ruleRepository.AddAsync(rule);
        }

        public async Task UpdateRuleAsync(RuleDTO ruleDTO)
        {
            var rule = await _ruleRepository.GetByIdAsync(ruleDTO.Id);
            if (rule == null) throw new ArgumentException("Rule not found.");

            rule.Value = ruleDTO.Value;
            rule.ComparisonOperator = ruleDTO.ComparisonOperator;
            rule.NeededTypeId = ruleDTO.NeededTypeId;
            rule.DeadlineDays = ruleDTO.DeadlineDays;

            await _ruleRepository.UpdateAsync(rule);
        }

        public async Task DeleteRuleAsync(int ruleId)
        {
            await _ruleRepository.DeleteAsync(ruleId);
        }

        public async Task<List<ParameterTypeDTO>> GetParameterTypesAsync()
        {
            var parameterTypes = await _parameterTypeRepository.GetAllAsync();

            return parameterTypes
                .Select(pt => new ParameterTypeDTO
                {
                    Id = pt.Id,
                    Type = pt.Type,
                    Name = pt.Name
                })
                .ToList();
        }

        public async Task<ParameterType> CreateParameterTypeAsync(ParameterTypeDTO parameterTypeDTO)
        {
            var parameterType = new ParameterType
            {
                Type = parameterTypeDTO.Type,
                Name = parameterTypeDTO.Name
            };

            await _parameterTypeRepository.AddAsync(parameterType);
            return parameterType;
        }

        public async Task ReplaceAllParameterTypesAsync(List<ParameterTypeDTO> updatedDtos)
        {
            // Получаем все текущие типы из репозитория
            var existingEntities = await _parameterTypeRepository.GetAllAsync();

            //Обновляем и добавляем новые
            foreach (var dto in updatedDtos)
            {
                if (dto.Id == 0)
                {
                    //Новый элемент
                    var newEntity = new ParameterType
                    {
                        Name = dto.Name,
                        Type = dto.Type
                    };
                    await _parameterTypeRepository.AddAsync(newEntity);
                }
                else
                {
                    // Существующий обновляем
                    var existing = existingEntities.FirstOrDefault(e => e.Id == dto.Id);
                    if (existing != null)
                    {
                        existing.Name = dto.Name;
                        existing.Type = dto.Type;
                        await _parameterTypeRepository.UpdateAsync(existing);
                    }
                    else
                    {
                        // Если Id не найден, можно добавить
                        var newEntity = new ParameterType
                        {
                            Name = dto.Name,
                            Type = dto.Type
                        };
                        await _parameterTypeRepository.AddAsync(newEntity);
                    }
                }
            }

            // Удаляем отсутствующие (те, у кого Id нет в updatedDtos)
            var updatedIds = updatedDtos.Where(d => d.Id != 0).Select(d => d.Id).ToHashSet();

            var toDelete = existingEntities.Where(e => !updatedIds.Contains(e.Id)).ToList();

            foreach (var entityToDelete in toDelete)
            {
                await _parameterTypeRepository.DeleteAsync(entityToDelete.Id);
            }
        }



        public async Task ReplaceAllServicesAsync(List<ServiceDTO> updatedDtos)
        {
            var existingEntities = await _serviceRepository.GetAllAsync();

            foreach (var dto in updatedDtos)
            {
                if (dto.Id == 0)
                {
                    // Новый создаём
                    var newEntity = new Service
                    {
                        Name = dto.Name,
                        Description = dto.Description,
                        ActivationDate = dto.ActivationDate,
                        DeactivationDate = dto.DeactivationDate
                    };
                    await _serviceRepository.AddAsync(newEntity);
                }
                else
                {
                    // Существующий обновляем
                    var existing = existingEntities.FirstOrDefault(e => e.Id == dto.Id);
                    if (existing != null)
                    {
                        existing.Name = dto.Name;
                        existing.Description = dto.Description;
                        existing.DeactivationDate = dto.DeactivationDate;

                        await _serviceRepository.UpdateAsync(existing);
                    }
                    else
                    {
                        var newEntity = new Service
                        {
                            Name = dto.Name,
                            Description = dto.Description,
                            ActivationDate = dto.ActivationDate,
                            DeactivationDate = dto.DeactivationDate
                        };
                        await _serviceRepository.AddAsync(newEntity);
                    }
                }
            }        
        }


        public async Task DeleteParameterTypeAsync(int typeId)
        {
            await _parameterTypeRepository.DeleteAsync(typeId);
        }

        public async Task<User> CreateUserAsync(UserDTO userDTO)
        {
            var user = new User
            {
                Login = userDTO.Login,
                Password = userDTO.Password,
                FullName = userDTO.FullName,
                Role = userDTO.Role
            };

            await _userRepository.AddAsync(user);
            return user;
        }
    }



}
