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
        //bool IsAdmin();
        Task<List<Service>> GetAllServicesAsync();
        Task<Service> CreateServiceAsync(ServiceDTO serviceDTO);
        Task<Service> UpdateServiceAsync(ServiceDTO serviceDTO);
        Task<List<Rule>> GetRulesForServiceAsync(int serviceId);
        Task AddRuleAsync(RuleDTO ruleDTO);
        Task UpdateRuleAsync(int ruleId, RuleDTO ruleDTO);
        Task DeleteRuleAsync(int ruleId);
        Task<List<ParameterTypeDTO>> GetParameterTypesAsync();
        Task<ParameterType> CreateParameterTypeAsync(ParameterTypeDTO parameterTypeDTO);
        Task DeleteParameterTypeAsync(int typeId);
        Task<User> CreateUserAsync(UserDTO userDTO);
        Task ReplaceAllParameterTypesAsync(List<ParameterTypeDTO> updatedDtos);
    }

    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IParameterTypeRepository _parameterTypeRepository;
        private readonly AuthSession _authSession;  // Внедрение AuthSession

        public AdminService(
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IRuleRepository ruleRepository,
            IParameterTypeRepository parameterTypeRepository,
            AuthSession authSession)  // Инициализация через DI
        {
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _ruleRepository = ruleRepository;
            _parameterTypeRepository = parameterTypeRepository;
            _authSession = authSession;  // Инициализация через DI
        }

        /*public bool IsAdmin()
        {
            // Используем экземпляр _authSession вместо статического свойства
            return _authSession.CurrentUser?.Role == UserRole.ADMIN;
        }*/

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllAsync();  // Получаем все услуги через сервисный репозиторий
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

            await _serviceRepository.UpdateAsync(service);
            return service;
        }

        public async Task<List<Rule>> GetRulesForServiceAsync(int serviceId)
        {
            return await _ruleRepository.GetByServiceIdAsync(serviceId);  // Получаем правила для услуги
        }

        //НЕ ПОНЯТНО ЗАЧЕМ ЭТОТ МЕТОД
        /*public async Task AddRuleToServiceAsync(int serviceId, Rule rule)
        {
            rule.ServiceId = serviceId;
            await _ruleRepository.AddAsync(rule);
        }*/

        public async Task AddRuleAsync(RuleDTO ruleDTO)
        {
            var rule = new Rule
            {
                ServiceId = ruleDTO.ServiceId,
                Value = ruleDTO.Value,
                ComparisonOperator = ruleDTO.ComparisonOperator,
                NeededTypeId = ruleDTO.NeededTypeId
            };

            await _ruleRepository.AddAsync(rule);
        }

        public async Task UpdateRuleAsync(int ruleId, RuleDTO ruleDTO)
        {
            var rule = await _ruleRepository.GetByIdAsync(ruleId);
            if (rule == null) throw new ArgumentException("Rule not found.");

            rule.Value = ruleDTO.Value;
            rule.ComparisonOperator = ruleDTO.ComparisonOperator;
            rule.NeededTypeId = ruleDTO.NeededTypeId;

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

            // 1. Обновляем и добавляем новые
            foreach (var dto in updatedDtos)
            {
                if (dto.Id == 0)
                {
                    // Новый элемент — создаём
                    var newEntity = new ParameterType
                    {
                        Name = dto.Name,
                        Type = dto.Type
                    };
                    await _parameterTypeRepository.AddAsync(newEntity);
                }
                else
                {
                    // Существующий элемент — обновляем
                    var existing = existingEntities.FirstOrDefault(e => e.Id == dto.Id);
                    if (existing != null)
                    {
                        existing.Name = dto.Name;
                        existing.Type = dto.Type;
                        await _parameterTypeRepository.UpdateAsync(existing);
                    }
                    else
                    {
                        // Если Id не найден, можно добавить или бросить ошибку (зависит от логики)
                        // Например, добавим как новый:
                        var newEntity = new ParameterType
                        {
                            Name = dto.Name,
                            Type = dto.Type
                        };
                        await _parameterTypeRepository.AddAsync(newEntity);
                    }
                }
            }

            // 2. Удаляем отсутствующие (те, у кого Id нет в updatedDtos)
            var updatedIds = updatedDtos.Where(d => d.Id != 0).Select(d => d.Id).ToHashSet();

            var toDelete = existingEntities.Where(e => !updatedIds.Contains(e.Id)).ToList();

            foreach (var entityToDelete in toDelete)
            {
                await _parameterTypeRepository.DeleteAsync(entityToDelete.Id);
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
