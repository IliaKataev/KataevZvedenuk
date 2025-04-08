# Диаграмма классов
## **Диаграмма классов**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/772ae48d6c4476122ba2cceaa5cdceabdf947236/class%20diagram/classdiagram5.png)

---

# Классы

## User  
**Поля:**  
- `fullName` — полное имя пользователя  
- `login` — логин для входа  
- `password` — пароль  
- `role` — роль пользователя  
- `parameters` — список параметров пользователя  
- `historyApplications` — список заявок пользователя  

---

## UserRole (enum)  
**Значения:**  
- CITIZEN  
- ADMIN  
- SERVANT  

---

## Parameter  
**Поля:**  
- `type` — тип параметра  
- `value` — значение параметра  

---

## ParameterType  
**Поля:**  
- `code` — код типа  
- `name` — название типа  

---

## Application  
**Поля:**  
- `user` — пользователь, подавший заявку  
- `service` — услуга, на которую подана заявка  
- `status` — текущий статус заявки  
- `resultDescription` — описание результата  
- `creationDate` — дата создания  
- `closureDate` — дата завершения  
- `deadline` — крайний срок оказания услуги  

---

## ApplicationStatus (enum)  
**Значения:**  
- IN_PROGRESS  
- COMPLETED  
- REJECTED  
- CANCELED  

---

## Service  
**Поля:**  
- `name` — название  
- `description` — описание  
- `activationDate` — дата активации  
- `deactivationDate` — дата деактивации  
- `rules` — список правил  

---

## Rule  
**Поля:**  
- `value` — выражение правила  
- `timeLimit` — срок для правила   
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
- `getApplicationsByService` — получить заявки по услуге  

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
- `createService` — создать услугу  
- `addRule` — добавить правило  
- `updateRule` — обновить правило  
- `deleteService` — удалить услугу  
- `deleteRule` — удалить правило  

---

## ServantController  
**Поля:**  
- `servantService` — сервис госслужащего  

**Методы:**  
- `processApplication` — обработать заявку  
- `updateStatus` — обновить статус  
- `updateApplication` — обновить заявку 
- `setResult`- установить результат
- `deleteApplication` - удалить заявку

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
- `serviceRepository` — репозиторий услуг  
- `ruleRepository` — репозиторий правил  

**Методы:**  
- `addRule` — добавить правило к услуге  
- `updateRule` — обновить правило  
- `getRulesByService` — получить правила по услуге  
- `createService` — создать услугу  
- `deleteService` - удалить услугу
- `deleteRule` - удалить правила 

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
- `deleteApplication` - удалить заявку

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

## ServiceRepository  
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
   Агрегация - Контроллер использует сервис для обработки логики заявок.

2. **ApplicationService → ApplicationRepository**  
   Агрегация - Сервис использует репозиторий для доступа к данным заявок.

3. **ServantService → ApplicationRepository**  
   Агрегация - Сервис госслужащего агрегирует репозиторий заявок для выполнения операций.

5. **Application → Service**  
   Агрегация - Заявка использует услугу, на которую она подана.

6. **ServantController → ServantService**  
   Агрегация - Контроллер госслужащего обращается к своему сервису.

7. **User → Parameter**  
   Агрегация - Пользователь агрегирует список параметров.

8. **CitizenService → ParameterRepository**  
   Агрегация - Сервис гражданина использует репозиторий параметров.

9. **CitizenService → ParameterTypeRepository**  
   Агрегация - Сервис гражданина использует репозиторий типов параметров.

10. **UserController → UserService**  
    Агрегация - Контроллер пользователя вызывает методы сервиса пользователя.

11. **UserService → UserRepository**  
    Агрегация - Сервис пользователя агрегирует репозиторий пользователей.

12. **ServantService → UserRepository**  
    Агрегация - Сервис госслужащего использует репозиторий пользователей.

13. **CitizenService → UserRepository**  
    Агрегация - Сервис гражданина использует репозиторий пользователей.

14. **AdminService → UserRepository**  
    Агрегация - Сервис администратора использует репозиторий пользователей.

15. **AdminService → ServiceRepository**  
    Агрегация - Сервис администратора использует репозиторий услуг.

16. **AdminService → RuleRepository**  
    Агрегация - Сервис администратора использует репозиторий правил.

17. **CitizenController → CitizenService**  
    Агрегация - Контроллер гражданина вызывает сервис гражданина.

18. **AdminController → AdminService**  
    Агрегация - Контроллер администратора обращается к сервису администратора.

19. **User ⬌ Parameter**  
    Агрегация - Параметры не существуют без пользователя.

20. **Map ⬌ Rule**  
    Агрегация - Правила принадлежат услуге и не существуют без неё.

21. **ApplicationRepository → Application**  
    Зависимость - Репозиторий работает с сущностью заявки.

22. **Application → ApplicationStatus**  
    Ассоцияция - Заявка использует статус из перечисления.

23. **User → UserRole**  
    Ассоцияция - Пользователь зависит от роли, определённой в перечислении.

24. **ParameterRepository → Parameter**  
    Зависимость - Репозиторий параметров взаимодействует с сущностью параметра.

25. **ParameterTypeRepository → ParameterType**  
    Зависимость - Репозиторий типов параметров работает с сущностью типа параметра.

26. **Parameter → ParameterType**  
    Агрегация - Параметр ссылается на тип параметра.

27. **Rule → ParameterType**  
    Агрегация - Правило использует типы параметров для валидации.

28. **UserRepository → User**  
    Зависимость - Репозиторий пользователей работает с сущностью пользователя.

29. **ServiceRepository → Service**  
    Зависимость - Репозиторий услуг работает с сущностью услуги.

30. **RuleRepository → Rule**  
    Зависимость - Репозиторий правил взаимодействует с сущностью правила.

---
