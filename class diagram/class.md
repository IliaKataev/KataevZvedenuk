# Диаграмма классов
## **Диаграмма классов**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/3de101b512b047c1e87b37c41407e5bc4ca76afd/class%20diagram/classDiagram4.png)

---

# Классы

## User  
**Поля:**  
- `id` — уникальный идентификатор пользователя  
- `fullName` — полное имя пользователя  
- `login` — логин для входа  
- `password` — пароль  
- `role` — роль пользователя  
- `parameters` — список параметров пользователя  
- `historyApplications` — список заявок пользователя  

---

## UserRole (enum)  
**Значения:**  
- Гражданин  
- Администратор  
- Госслужащий  

---

## Parameter  
**Поля:**  
- `id` — уникальный идентификатор параметра  
- `type` — тип параметра  
- `value` — значение параметра  

---

## ParameterType  
**Поля:**  
- `id` — уникальный идентификатор типа  
- `code` — код типа  
- `name` — название типа  

---

## Application  
**Поля:**  
- `id` — уникальный идентификатор заявки  
- `user` — пользователь, подавший заявку  
- `map` — услуга, на которую подана заявка  
- `status` — текущий статус заявки  
- `resultDescription` — описание результата  
- `creationDate` — дата создания  
- `closureDate` — дата завершения  
- `deadline` — крайний срок оказания услуги  

---

## ApplicationStatus (enum)  
**Значения:**  
- На рассмотрении  
- Оказана  
- Отклонена  
- Отменена  

---

## Map  
**Поля:**  
- `id` — уникальный идентификатор услуги  
- `name` — название  
- `description` — описание  
- `activationDate` — дата активации  
- `deactivationDate` — дата деактивации  
- `rules` — список правил  

---

## Rule  
**Поля:**  
- `id` — уникальный идентификатор  
- `value` — выражение правила  
- `comparisonOperator` — оператор сравнения  
- `neededTypes` — список нужных параметров  

---

# Контроллеры

## UserController  
**Поля:**  
- `userService` — сервис пользователя  

**Методы:**  
- `updateName` — обновляет имя пользователя  
- `updatePassword` — обновляет пароль  

---

## ApplicationController  
**Поля:**  
- `applicationService` — сервис заявок  

**Методы:**  
- `createNewApplication` — создать новую заявку  
- `cancelApplication` — отменить заявку  
- `updateApplication` — обновить заявку  
- `getApplicationsByUser` — получить заявки пользователя  
- `getApplicationStatus` — получить статус заявки  
- `getApplicationsByStatus` — получить заявки по статусу  
- `getApplicationsByMap` — получить заявки по услуге  

---

## CitizenController  
**Поля:**  
- `citizenService` — сервис гражданина  

**Методы:**  
- `addParameter` — добавить параметр 
- `updateParameter` — обновить параметр
- `deleteParameter` — удалить параметр  

---

## AdminController  
**Методы:**  
- `createMap` — создать услугу  
- `addRule` — добавить правило  
- `updateRule` — обновить правило  
- `deleteMap` — удалить услугу  
- `deleteRule` — удалить правило  

---

## ServantController  
**Поля:**  
- `servantService` — сервис госслужащего  

**Методы:**  
- `processApplication` — обработать заявку  
- `updateStatus` — обновить статус  
- `updateApplication` — обновить заявку  

---

# Сервисы

## UserService  
**Поля:**  
- `updateName` — обновляет имя пользователя  
- `updatePassword` — обновляет пароль 

---

## ApplicationService  
**Поля:**  
- `applicationRepository` — репозиторий заявок  

**Методы:**  
- `createNewApplication` — создать новую заявку  
- `cancelApplication` — отменить заявку  
- `getApplicationStatus` — получить заявки по статусу  
- `getApplicationsByStatus` — получить заявки по статусу  

---

## AdminService  
**Поля:**  
- `userRepository` — репозиторий пользователей  
- `mapRepository` — репозиторий услуг  
- `ruleRepository` — репозиторий правил  

**Методы:**  
- `createMap` — создать услугу  
- `addRule` — добавить правило к услуге  
- `updateRule` — обновить правило  
- `getRulesByMap` — получить правила по услуге  

---

## CitizenService  
**Поля:**  
- `userRepository` — репозиторий пользователей  
- `parameterRepository` — репозиторий параметров 
- `parameterTypeRepository` — репозиторий  типа параметров 

**Методы:**  
- `addParameter` — добавить параметр 
- `updateParameter` — обновить параметр
- `deleteParameter` — удалить параметр  

---

## ServantService  
**Поля:**  
- `userRepository` — репозиторий пользователей  
- `applicationRepository` — репозиторий заявок  

**Методы:**  
- `processApplication` — обработать заявку  
- `updateStatus` — обновить статус  
- `updateApplication` — обновить заявку  
- `setResult` — установить результат  

---

# Интерфейсы и Репозитории

## UserRepository  
**Методы:**  
- `save` — сохранить пользователя  
- `delete` — удалить пользователя  
- `findById` — найти пользователя по ID  

---

## ApplicationRepository  
**Методы:**  
- `findById` — найти заявку по ID  
- `findByStatus` — найти заявки по статусу  
- `save` — сохранить заявку  
- `deleteById` — удалить заявку  
- `update` — обновить заявку  

---

## MapRepository  
**Методы:**  
- `save` — сохранить услугу  
- `delete` — удалить услугу  
- `update` — обновить услугу  
- `findById` — найти услугу по ID  

---

## RuleRepository  
**Методы:**  
- `save` — сохранить правило  
- `update` — обновить правило  
- `delete` — удалить правило  
- `findById` — найти правило по ID  

---

## ParameterRepository  
**Методы:**  
- `save` — сохранить параметр  
- `delete` — удалить параметр  
- `findById` — найти параметр по ID  

---

## ParameterTypeRepository  
**Методы:**  
- `save` — сохранить тип параметра  
- `delete` — удалить тип параметра  
- `update` — обновить тип параметра  
- `findById` — найти тип параметра по ID  

---

## Связи между компонентами системы

1. **ApplicationController → ApplicationService**  
   Контроллер использует сервис для обработки логики заявок.

2. **ApplicationService → IApplicationRepository**  
   Сервис использует репозиторий для доступа к данным заявок.

3. **ServantService → IApplicationRepository**  
   Сервис госслужащего агрегирует репозиторий заявок для выполнения операций.

4. **ServantService → Application**  
   Сервис обрабатывает заявку, взаимодействует с её полями и логикой.

5. **Application → Map**  
    Заявка использует услугу, на которую она подана.

6. **ServantController → ServantService**  
   Контроллер госслужащего обращается к своему сервису.

7. **User → Parameter**  
   Пользователь агрегирует список параметров (композиция).

8. **CitizenService → IParameterRepository**  
   Сервис гражданина использует репозиторий параметров.

9. **CitizenService → IParameterTypeRepository**  
   Сервис гражданина использует репозиторий типов параметров.

10. **UserController → UserService**  
    Контроллер пользователя вызывает методы сервиса пользователя.

11. **UserService → IUserRepository**  
    Сервис пользователя агрегирует репозиторий пользователей.

12. **ServantService → IUserRepository**  
    Сервис госслужащего использует репозиторий пользователей.

13. **CitizenService → IUserRepository**  
    Сервис гражданина использует репозиторий пользователей.

14. **AdminService → IUserRepository**  
    Сервис администратора использует репозиторий пользователей.

15. **AdminService → IMapRepository**  
    Сервис администратора использует репозиторий услуг.

16. **AdminService → IRuleRepository**  
    Сервис администратора использует репозиторий правил.

17. **CitizenController → CitizenService**  
    Контроллер гражданина вызывает сервис гражданина.

18. **AdminController → AdminService**  
    Контроллер администратора обращается к сервису администратора.

19. **User ⬌ Parameter**  
    Параметры не существуют без пользователя (композиция).

20. **Map ⬌ Rule**  
    Правила принадлежат услуге и не существуют без неё.

21. **IApplicationRepository → Application**  
    Репозиторий работает с сущностью заявки.

22. **Application → ApplicationStatus**  
    Заявка использует статус из перечисления.

23. **User → UserRole**  
    Пользователь зависит от роли, определённой в перечислении.

24. **IParameterRepository → Parameter**  
    Репозиторий параметров взаимодействует с сущностью параметра.

25. **IParameterTypeRepository → ParameterType**  
    Репозиторий типов параметров работает с сущностью типа параметра.

26. **Parameter → ParameterType**  
    Параметр ссылается на тип параметра.

27. **Rule → ParameterType**  
    Правило использует типы параметров для валидации.

28. **IUserRepository → User**  
    Репозиторий пользователей работает с сущностью пользователя.

29. **IMapRepository → Map**  
    Репозиторий услуг работает с сущностью услуги.

30. **IRuleRepository → Rule**  
    Репозиторий правил взаимодействует с сущностью правила.

---
