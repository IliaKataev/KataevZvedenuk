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
        Task<List<ServiceDTO>> GetAllServices();
        Task<List<RuleDTO>> GetRulesForService(int serviceId);
        Task AddRule(RuleDTO ruleDTO);
        Task UpdateRule(RuleDTO ruleDTO);
        Task DeleteRule(int ruleId);
        Task<List<ParameterTypeDTO>> GetParameterTypes();
        Task<User> CreateUser(UserDTO userDTO);
        Task ReplaceAllParameterTypes(List<ParameterTypeDTO> updatedDtos);
        Task ReplaceAllServices(List<ServiceDTO> updatedDtos);

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

        public async Task<List<ServiceDTO>> GetAllServices()
        {
            var services = await _serviceRepository.GetAll();
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
        public async Task<List<RuleDTO>> GetRulesForService(int serviceId)
        {
            var rules = await _ruleRepository.GetByServiceId(serviceId);

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


        public async Task AddRule(RuleDTO ruleDTO)
        {
            var rule = new Rule
            {
                ServiceId = ruleDTO.ServiceId,
                Value = ruleDTO.Value,
                ComparisonOperator = ruleDTO.ComparisonOperator,
                NeededTypeId = ruleDTO.NeededTypeId,
                DeadlineDays = ruleDTO.DeadlineDays
            };

            await _ruleRepository.Add(rule);
        }

        public async Task UpdateRule(RuleDTO ruleDTO)
        {
            var rule = await _ruleRepository.GetById(ruleDTO.Id);
            if (rule == null) throw new ArgumentException("Rule not found.");

            rule.Value = ruleDTO.Value;
            rule.ComparisonOperator = ruleDTO.ComparisonOperator;
            rule.NeededTypeId = ruleDTO.NeededTypeId;
            rule.DeadlineDays = ruleDTO.DeadlineDays;

            await _ruleRepository.Update(rule);
        }

        public async Task DeleteRule(int ruleId)
        {
            await _ruleRepository.Delete(ruleId);
        }

        public async Task<List<ParameterTypeDTO>> GetParameterTypes()
        {
            var parameterTypes = await _parameterTypeRepository.GetAll();

            return parameterTypes
                .Select(pt => new ParameterTypeDTO
                {
                    Id = pt.Id,
                    Type = pt.Type,
                    Name = pt.Name
                })
                .ToList();
        }

        public async Task ReplaceAllParameterTypes(List<ParameterTypeDTO> updatedDtos)
        {
            // Получаем все текущие типы из репозитория
            var existingEntities = await _parameterTypeRepository.GetAll();

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
                    await _parameterTypeRepository.Add(newEntity);
                }
                else
                {
                    // Существующий обновляем
                    var existing = existingEntities.FirstOrDefault(e => e.Id == dto.Id);
                    if (existing != null)
                    {
                        existing.Name = dto.Name;
                        existing.Type = dto.Type;
                        await _parameterTypeRepository.Update(existing);
                    }
                    else
                    {
                        // Если Id не найден, можно добавить
                        var newEntity = new ParameterType
                        {
                            Name = dto.Name,
                            Type = dto.Type
                        };
                        await _parameterTypeRepository.Add(newEntity);
                    }
                }
            }

            // Удаляем отсутствующие (те, у кого Id нет в updatedDtos)
            var updatedIds = updatedDtos.Where(d => d.Id != 0).Select(d => d.Id).ToHashSet();

            var toDelete = existingEntities.Where(e => !updatedIds.Contains(e.Id)).ToList();

            foreach (var entityToDelete in toDelete)
            {
                await _parameterTypeRepository.Delete(entityToDelete.Id);
            }
        }



        public async Task ReplaceAllServices(List<ServiceDTO> updatedDtos)
        {
            var existingEntities = await _serviceRepository.GetAll();

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
                    await _serviceRepository.Add(newEntity);
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

                        await _serviceRepository.Update(existing);
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
                        await _serviceRepository.Add(newEntity);
                    }
                }
            }        
        }

        public async Task<User> CreateUser(UserDTO userDTO)
        {
            var user = new User
            {
                Login = userDTO.Login,
                Password = userDTO.Password,
                FullName = userDTO.FullName,
                Role = userDTO.Role
            };

            await _userRepository.Add(user);
            return user;
        }
    }



}
