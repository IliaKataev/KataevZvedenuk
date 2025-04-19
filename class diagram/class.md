# Диаграмма классов
## **Диаграмма классов**


![](https://github.com/IliaKataev/KataevZvedenuk/blob/e8335dc017040e003b10fd4b301a5dd8e44d850c/class%20diagram/classdiaagram.png)

---

# Описание диаграммы классов

## Классы

### User
**Поля**:
- `fullName` — полное имя пользователя
- `login` — логин для входа
- `password` — пароль
- `role` — роль пользователя
- `parameters` — список параметров пользователя
- `historyApplications` — список заявок пользователя

### UserRole (enum)
**Значения**:
- `CITIZEN`
- `ADMIN`
- `SERVANT`

### Parameter
**Поля**:
- `type` — тип параметра
- `value` — значение параметра

### ParameterType
**Поля**:
- `code` — код типа
- `name` — название типа

### Application
**Поля**:
- `user` — пользователь, подавший заявку
- `service` — услуга, на которую подана заявка
- `status` — текущий статус заявки
- `resultDescription` — описание результата
- `creationDate` — дата создания
- `closureDate` — дата завершения
- `deadline` — крайний срок оказания услуги

### ApplicationStatus (enum)
**Значения**:
- `IN_PROGRESS`
- `COMPLETED`
- `REJECTED`
- `CANCELED`

### Service
**Поля**:
- `name` — название
- `description` — описание
- `activationDate` — дата активации
- `deactivationDate` — дата деактивации
- `rules` — список правил

### Rule
**Поля**:
- `value` — выражение правила
- `timeLimit` — срок для правила
- `comparisonOperator` — оператор сравнения
- `neededTypes` — список нужных параметров

### AuthSession
**Поля**:
- `userId` — ID текущего авторизованного пользователя
- `isAuthenticated` — флаг, указывающий, авторизован ли пользователь

---

## Контроллеры

### UserController
**Поля**:
- `userService` — сервис пользователя

**Методы**:
- `updateName` — обновляет имя пользователя
- `updatePassword` — обновляет пароль

### ApplicationController
**Поля**:
- `applicationService` — сервис заявок

**Методы**:
- `createNewApplication` — создать новую заявку
- `cancelApplication` — отменить заявку
- `updateApplication` — обновить заявку
- `getApplicationsByUser` — получить заявки пользователя
- `getApplicationStatus` — получить статус заявки
- `getApplicationsByStatus` — получить заявки по статусу
- `getApplicationsByService` — получить заявки по услуге

### CitizenController
**Поля**:
- `citizenService` — сервис гражданина

**Методы**:
- `addParameter` — добавить параметр
- `updateParameter` — обновить параметр
- `deleteParameter` — удалить параметр

### AdminController
**Методы**:
- `createService` — создать услугу
- `addRule` — добавить правило
- `updateRule` — обновить правило
- `deleteService` — удалить услугу
- `deleteRule` — удалить правило
- `createParameterType` — создать тип параметра
- `updateParameterType` — обновить тип параметра
- `deleteParameterType` — удалить тип параметра

### ServantController
**Поля**:
- `servantService` — сервис госслужащего

**Методы**:
- `processApplication` — обработать заявку
- `updateStatus` — обновить статус
- `updateApplication` — обновить заявку
- `setResult` — установить результат
- `deleteApplication` — удалить заявку

### AuthController
**Поля**:
- `authService` — сервис аутентификации

**Методы**:
- `login` — выполнить вход
- `logout` — выйти из системы

---

## Сервисы

### UserService
**Методы**:
- `updateName` — обновляет имя пользователя
- `updatePassword` — обновляет пароль

### ApplicationService
**Поля**:
- `applicationRepository` — репозиторий заявок

**Методы**:
- `createNewApplication` — создать новую заявку
- `cancelApplication` — отменить заявку
- `getApplicationStatus` — получить статус заявки
- `getApplicationsByStatus` — получить заявки по статусу

### AdminService
**Поля**:
- `userRepository` — репозиторий пользователей
- `serviceRepository` — репозиторий услуг
- `ruleRepository` — репозиторий правил
- `parameterTypeRepository` — репозиторий типов параметров

**Методы**:
- `addRule` — добавить правило к услуге
- `updateRule` — обновить правило
- `getRulesByService` — получить правила по услуге
- `createService` — создать услугу
- `deleteService` — удалить услугу
- `deleteRule` — удалить правило
- `createParameterType` — создать тип параметра
- `updateParameterType` — обновить тип параметра
- `deleteParameterType` — удалить тип параметра

### CitizenService
**Поля**:
- `userRepository` — репозиторий пользователей
- `parameterRepository` — репозиторий параметров
- `parameterTypeRepository` — репозиторий типов параметров

**Методы**:
- `addParameter` — добавить параметр (на основе существующего типа)
- `updateParameter` — обновить параметр
- `deleteParameter` — удалить параметр

### ServantService
**Поля**:
- `userRepository` — репозиторий пользователей
- `applicationRepository` — репозиторий заявок

**Методы**:
- `processApplication` — обработать заявку
- `updateStatus` — обновить статус
- `updateApplication` — обновить заявку
- `setResult` — установить результат
- `deleteApplication` — удалить заявку

### AuthService
**Поля**:
- `authSession` — текущая сессия аутентификации
- `UserRepository` — текущая сессия аутентификации

**Методы**:
- `login` — аутентификация пользователя по логину и паролю, создание сессии
- `logout` — выход из системы, завершение сессии
- `getCurrentUserId` — получение ID текущего авторизованного пользователя из сессии
- `isAuthenticated` — проверка, авторизован ли пользователь

---

## Репозитории

### UserRepository
**Методы**:
- `save` — сохранить пользователя
- `delete` — удалить пользователя
- `findById` — найти пользователя по ID

### ApplicationRepository
**Методы**:
- `findById` — найти заявку по ID
- `findByStatus` — найти заявки по статусу
- `save` — сохранить заявку
- `deleteById` — удалить заявку
- `update` — обновить заявку

### ServiceRepository
**Методы**:
- `save` — сохранить услугу
- `delete` — удалить услугу
- `update` — обновить услугу
- `findById` — найти услугу по ID

### RuleRepository
**Методы**:
- `save` — сохранить правило
- `update` — обновить правило
- `delete` — удалить правило
- `findById` — найти правило по ID

### ParameterRepository
**Методы**:
- `save` — сохранить параметр
- `delete` — удалить параметр
- `findById` — найти параметр по ID

### ParameterTypeRepository
**Методы**:
- `save` — сохранить тип параметра
- `delete` — удалить тип параметра
- `update` — обновить тип параметра
- `findById` — найти тип параметра по ID

---

## Связи

1. `ApplicationController → ApplicationService` — Агрегация
2. `ApplicationService → ApplicationRepository` — Агрегация
3. `ServantService → ApplicationRepository` — Агрегация
4. `Application → Service` — Агрегация
5. `ServantController → ServantService` — Агрегация
6. `User → Parameter` — Агрегация
7. `CitizenService → ParameterRepository` — Агрегация
8. `CitizenService → ParameterTypeRepository` — Агрегация
9. `UserController → UserService` — Агрегация
10. `UserService → UserRepository` — Агрегация
11. `ServantService → UserRepository` — Агрегация
12. `CitizenService → UserRepository` — Агрегация
13. `AdminService → UserRepository` — Агрегация
14. `AdminService → ServiceRepository` — Агрегация
15. `AdminService → RuleRepository` — Агрегация
16. `AdminService → ParameterTypeRepository` — Агрегация
17. `CitizenController → CitizenService` — Агрегация
18. `AdminController → AdminService` — Агрегация
19. `User ⬌ Parameter` — Агрегация
20. `Map ⬌ Rule` — Агрегация
21. `ApplicationRepository → Application` — Зависимость
22. `Application → ApplicationStatus` — Ассоциация
23. `User → UserRole` — Ассоциация
24. `ParameterRepository → Parameter` — Зависимость
25. `ParameterTypeRepository → ParameterType` — Зависимость
26. `Parameter → ParameterType` — Агрегация
27. `Rule → ParameterType` — Агрегация
28. `UserRepository → User` — Зависимость
29. `ServiceRepository → Service` — Зависимость
30. `RuleRepository → Rule` — Зависимость
31. `AuthController → AuthService` — Агрегация
32. `AuthService → AuthSession` — Агрегация
33. `AuthService → UserRepository` — Агрегация
34. `ApplicationRepository → ApplicationStatus` - Ассоциация
---
