
# Описание диаграммы последовательности

# **Администратор**

**1. Диаграмма последовательности: Вход в систему (администратор)**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/admin/adminLogin.png)

**Основной сценарий:**
1. Администратор отправляет логин и пароль в `AuthController`.
2. `AuthController` передаёт данные в `AuthService`.
3. `AuthService` валидирует корректность введённых данных.
4. Если данные валидны:
   - `AuthService` запрашивает пользователя по логину через `UserRepository`.
   - `UserRepository` выполняет запрос к базе данных для поиска пользователя.
   - Если пользователь найден:
     - Проверяется правильность введённого пароля.
     - При совпадении пароля — авторизация успешна, возвращается `User`.
5. `AuthController` сообщает администратору об успешной авторизации.

**Альтернативные сценарии:**
- **Некорректные данные ввода:**
  - `AuthService` сразу возвращает ошибку некорректных данных.
  - `AuthController` сообщает администратору о некорректном формате логина или пароля.
  
- **Пользователь не найден:**
  - После запроса в базу данных `UserRepository` сообщает, что пользователь отсутствует.
  - `AuthService` возвращает ошибку "пользователь не найден".
  - `AuthController` уведомляет администратора об ошибке.

- **Неверный пароль:**
  - При несоответствии пароля `AuthService` возвращает ошибку неправильного пароля.
  - `AuthController` сообщает администратору о неверном пароле.

---

**2. Диаграмма последовательности: Взаимодействие с услугой**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/admin/adminAddUpdateDeleteService.png)

**Основной сценарий:**
1. Администратор отправляет данные о новой услуге (название, описание, правила и др.) в `AdminController`.
2. `AdminController` передаёт данные в `AdminService`.
3. `AdminService` сохраняет услугу через `ServiceRepository`.
4. `ServiceRepository` делает запрос на добавление услуги в базу данных.
5. После успешного создания услуги база данных подтверждает добавление.
6. `AdminService` инициирует добавление правил к услуге через `RuleRepository`.
7. `RuleRepository` запрашивает наличие правил в базе данных.

**Альтернативные сценарии:**
- **Правила найдены:**
  - База данных возвращает найденные правила.
  - `RuleRepository` обновляет записи правил, привязывая их к созданной услуге.
  - Услуга успешно создаётся вместе с прикреплёнными правилами.
  - Администратор получает сообщение об успешном создании услуги с правилами.

- **Правила не найдены:**
  - `RuleRepository` сообщает об ошибке отсутствия правил.
  - Услуга создаётся без прикрепления правил.
  - Администратор получает сообщение, что услуга создана без правил.

---

**3. Диаграмма последовательности: Создание учетной записи**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/admin/adminCreateUser.png)

**Основной сценарий:**
1. Администратор отправляет данные нового пользователя (`username`, `password`, `role` и др.) в `AdminController`.
2. `AdminController` вызывает метод `CreateUser` в `AdminService`.
3. `AdminService` проверяет, свободен ли указанный логин, обратившись к `UserRepository`.
4. `UserRepository` делает запрос к базе данных на поиск пользователя с заданным логином.
5. База данных возвращает результат:
   - Если логин свободен:
     6. `UserRepository` сообщает об этом `AdminService`.
     7. `AdminService` инициирует добавление нового пользователя через `UserRepository`.
     8. `UserRepository` отправляет запрос на вставку новой записи в базу данных.
     9. База данных подтверждает создание пользователя.
     10. `UserRepository` уведомляет `AdminService` об успешном создании учетной записи.
     11. `AdminService` передаёт успешный результат в `AdminController`.
     12. `AdminController` сообщает администратору об успешном создании учетной записи.

**Альтернативный сценарий:**  
- **Логин занят:**
  1. `UserRepository` сообщает `AdminService`, что логин уже используется.
  2. `AdminService` отправляет в `AdminController` сообщение об ошибке.
  3. `AdminController` уведомляет администратора об ошибке: логин уже занят.

---

# **Гражданин**

**1. Диаграмма последовательности: Создание новой заявки**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/citizen/citizenAddApplication.png)

**Основной сценарий:**
1. Пользователь отправляет запрос на создание новой заявки через `CitizenController`, передавая `serviceId` и `parameters`.
2. `CitizenController` запрашивает текущего пользователя через `AuthSession`.
3. `AuthSession` возвращает `currentUser.id` контроллеру.
4. `CitizenController` вызывает `createNewApplication` в `ApplicationService`, передавая `userId`, `serviceId` и `parameters`.
5. `ApplicationService` обращается к `ServiceRepository`, чтобы получить услугу по `serviceId`.
6. `ServiceRepository` выполняет запрос к базе данных.
7. База данных возвращает либо данные об услуге, либо `null`.

**Ветвление:**
- **Если услуга найдена:**
  8. `ApplicationService` создаёт новую заявку, передавая её в `ApplicationRepository`.
  9. `ApplicationRepository` отправляет команду вставки новой заявки в базу данных.
  10. База данных подтверждает успешную вставку.
  11. `ApplicationRepository` возвращает созданную заявку в `ApplicationService`.
  12. `ApplicationService` отправляет `ApplicationDTO` обратно в `CitizenController`.

- **Если услуга не найдена:**
  8. `ApplicationService` отправляет в `CitizenController` сообщение об ошибке "услуга не найдена".

9. `CitizenController` отправляет пользователю результат операции: подтверждение создания заявки или сообщение об ошибке.

---

**2. Диаграмма последовательности: Вход в систему гражданина**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/citizen/citizenLogin.png)

**Основной сценарий:**
1. Гражданин отправляет запрос на авторизацию в `AuthController` с `login` и `password`.
2. `AuthController` вызывает `TryLogin` у `AuthService`.
3. `AuthService` сначала выполняет валидацию входных данных (`ValidateInput`):
   - Если данные некорректны, ошибка возвращается в `AuthController`, а затем пользователю показывается сообщение об ошибке ("неверные данные").
4. Если данные корректны:
   - `AuthService` запрашивает пользователя по логину через `UserRepository`.
   - `UserRepository` выполняет SQL-запрос к базе данных (`SELECT * FROM Users WHERE login = login`).
   
5. Возможны две ситуации:
   - **Пользователь найден**:
     - Данные возвращаются в `AuthService`.
     - `AuthService` проверяет пароль (`ValidatePassword`):
       - Если пароль неверный — ошибка возвращается, пользователю показывается сообщение об ошибке ("неверный пароль").
       - Если пароль верный — успешная авторизация: `AuthService` отправляет `UserDTO` в `AuthController`, а `AuthController` информирует гражданина об успешной авторизации.
   - **Пользователь не найден**:
     - Ошибка возвращается в `AuthService`, а затем — в `AuthController`.
     - Пользователь получает сообщение об ошибке ("пользователь не найден").

---

**3. Диаграмма последовательности: Просмотр заявок пользователя**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/citizen/citizenViewApplication.png)

**Основной сценарий:**
1. Пользователь инициирует действие `ViewMyApplications()` в `CitizenController`.
2. `CitizenController` запрашивает текущего пользователя через `AuthSession` (`GetCurrentUser()`).
   - `AuthSession` возвращает объект `currentUser`.
3. `CitizenController` вызывает метод `GetMyApplications()` у `CitizenService`.
4. `CitizenService` отправляет запрос в `ApplicationRepository` на получение всех заявок пользователя по его ID (`GetByUserId(currentUserID)`).
5. `ApplicationRepository` делает SQL-запрос в базу данных:
   - `SELECT * FROM Applications WHERE user_id = currentUserID`
6. База данных возвращает список заявок (`List<Application>`).
7. `ApplicationRepository` возвращает этот список в `CitizenService`.
8. `CitizenService` преобразует заявки в DTO (`List<ApplicationDTO`) и передаёт их в `CitizenController`.
9. `CitizenController` отправляет данные пользователю.

**Результат:** Пользователь видит список своих заявок.

---

**4. Диаграмма последовательности: Отмена заявки пользователя**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/citizen/citizenCancelApplication.png)

**Основной сценарий:**
1. Пользователь инициирует запрос на отмену заявки с помощью метода `CancelMyApplication(applicationId)` в `CitizenController`.
2. `CitizenController` обращается к `AuthSession`, чтобы получить текущего пользователя (`GetCurrentUser()`).
   - `AuthSession` возвращает данные о текущем пользователе (`currentUser`).
3. `CitizenController` вызывает метод `CancelApplication(applicationId)` в `CitizenService`.
4. В `CitizenService` происходит запрос в `ApplicationRepository` для поиска заявки по ID (`GetById(applicationId)`).
5. `ApplicationRepository` делает SQL-запрос в базу данных для получения данных о заявке:
   - `SELECT * FROM Applications WHERE id = applicationId`
6. Если заявка найдена:
   - В `CitizenService` обновляется статус заявки на "CANCELED" и этот статус сохраняется в базе данных с помощью запроса `UPDATE Applications SET status = 'CANCELED' WHERE id = applicationId`.
   - База данных подтверждает успешное обновление.
7. Если заявка не найдена, `CitizenService` отправляет ошибку обратно в `CitizenController`.
8. `CitizenController` возвращает пользователю сообщение о том, что заявка отменена, либо что заявка не найдена.

---

**5. Диаграмма последовательности: Изменение данных учетной записи пользователя**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/citizen/CitizenChangeData.png)

**Основной сценарий:**
1. Пользователь инициирует запрос на изменение данных учетной записи через метод `updatePersonalData(fullname, password)` в `CitizenController`.
2. `CitizenController` обращается к `AuthSession`, чтобы получить текущего пользователя (`GetCurrentUser()`).
   - `AuthSession` возвращает данные о текущем пользователе (`currentUser`).
3. `CitizenController` вызывает метод `updatePersonalData(userId, fullname, password)` в `UserService`.
4. В `UserService` происходит запрос в `UserRepository` для поиска пользователя по ID (`GetById(userId)`).
5. `UserRepository` выполняет SQL-запрос в базу данных для получения данных о пользователе:
   - `SELECT * FROM Users WHERE id = userId`
6. Если пользователь найден:
   - В `UserService` происходит обновление данных пользователя с новыми значениями `fullname` и `password`.
   - Эти изменения сохраняются в базе данных с помощью запроса `UPDATE Users SET fullname = fullname, password = password WHERE id = userId`.
   - База данных подтверждает успешное обновление данных.
7. Если пользователь не найден, `UserService` возвращает ошибку, и `CitizenController` передает информацию пользователю.
8. Если обновление прошло успешно, `UserController` возвращает пользователю сообщение о том, что данные учетной записи обновлены.

---

# **Госслужащий**

### **1. Диаграмма последовательности: Вход в систему (Госслужащий)**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/servant/servantLogin.png)

1. **Запрос на вход:**
   - Госслужащий инициирует вход в систему, отправляя запрос с логином и паролем через `AuthController` методом `Login(login, password)`.

2. **Проверка данных:**
   - `AuthController` передает данные в `AuthService`, чтобы проверить правильность ввода.
   - В `AuthService` происходит валидация данных. Если данные некорректны (например, пустое поле логина или пароля), сервис отправляет ошибку через `AuthController`, и тот возвращает сообщение об ошибке пользователю (например, "Неверные данные").

3. **Поиск пользователя в базе данных:**
   - Если данные валидны, `AuthService` отправляет запрос в `UserRepository` для поиска пользователя по логину с помощью метода `GetByLogin(login)`.
   - В `UserRepository` происходит запрос в базу данных `SELECT * FROM Users WHERE login = login` для получения данных о пользователе.

4. **Проверка пользователя:**
   - Если пользователь найден в базе данных, система переходит к проверке пароля. Для этого в `AuthService` выполняется валидация пароля.
   - Если пароль неверный, `AuthService` возвращает ошибку в `AuthController`, и тот выводит сообщение об ошибке "Неверный пароль" пользователю.

5. **Успешная авторизация:**
   - Если пароль верный, `AuthService` отправляет успешный ответ в `AuthController`, и тот передает авторизованному пользователю объект `UserDTO` с необходимыми данными.

6. **Ошибка при отсутствии пользователя:**
   - Если пользователь не найден в базе данных, `UserRepository` возвращает ошибку в `AuthService`, и тот сообщает о том, что пользователь не найден. `AuthController` возвращает ошибку "Пользователь не найден" пользователю.

---

### **2. Диаграмма последовательности: Добавление результата к заявке:**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/servant/servantSetResult.png)

1. **Запрос на обработку заявки:**
   - Госслужащий (актер `civilServant`) отправляет запрос на обработку заявки через `CivilServantController` с идентификатором заявки (`applicationId`) и результатом (`result`) через метод `ProcessApplication(applicationId, result)`.

2. **Обработка заявки в сервисе:**
   - `CivilServantController` передает запрос в `CivilServantService` для обработки заявки, вызвав метод `ProcessApplication(applicationId, result)`.

3. **Поиск заявки в базе данных:**
   - В `CivilServantService` происходит запрос в репозиторий `ApplicationRepository` с целью получить заявку по идентификатору через метод `GetById(applicationId)`.
   - В `ApplicationRepository` выполняется запрос в базу данных: `SELECT * FROM Applications WHERE id = applicationId` для поиска заявки.

4. **Проверка наличия заявки:**
   - Если заявка найдена, данные заявки передаются обратно в `CivilServantService`.
   - Если заявка не найдена, система возвращает ошибку в `CivilServantController`, уведомляя госслужащего о том, что заявка не существует.

5. **Обновление результата заявки:**
   - Если заявка найдена, происходит обновление результата заявки. В `CivilServantService` выполняется запрос на обновление данных заявки в репозитории: `UPDATE Applications SET result = result WHERE id = applicationId`.
   - После успешного обновления, `CivilServantService` возвращает подтверждение о том, что заявка была успешно обновлена.

6. **Ответ госслужащему:**
   - `CivilServantController` получает результат из `CivilServantService` и отправляет уведомление госслужащему о том, что заявка была успешно обновлена.

7. **Ошибка при отсутствии заявки:**
   - Если заявка не найдена в базе данных, возвращается сообщение об ошибке, и госслужащий получает уведомление о том, что заявка не существует.

---

Диаграмма последовательности для **"Изменение статуса заявки"** описывает процесс изменения статуса заявки, инициированный госслужащим. Включает проверку, валидацию статуса и обновление записи в базе данных.

---

### **3. Диаграмма последовательности: Обработка заявки:**

![](https://github.com/IliaKataev/KataevZvedenuk/blob/eb31f887824116c2085350650e35df56620217f8/sequence/images/servant/servantProcessApplication.png)

1. **Запрос на изменение статуса:**
   - Госслужащий (актер `civilServant`) отправляет запрос через `CivilServantController` для изменения статуса заявки. Запрос включает идентификатор заявки (`applicationId`) и новый статус (`newStatus`) через метод `ChangeStatus(applicationId, newStatus)`.

2. **Обработка запроса в сервисе:**
   - `CivilServantController` передает запрос в `CivilServantService`, вызвав метод `ChangeStatus(applicationId, newStatus)`.

3. **Поиск заявки в базе данных:**
   - В `CivilServantService` выполняется запрос в репозиторий `ApplicationRepository`, чтобы получить заявку по идентификатору через метод `GetById(applicationId)`.
   - В `ApplicationRepository` выполняется запрос в базу данных: `SELECT * FROM Applications WHERE id = applicationId` для получения данных заявки.

4. **Проверка наличия заявки:**
   - Если заявка найдена, данные заявки передаются обратно в `CivilServantService`.
   - Если заявка не найдена, система возвращает ошибку в `CivilServantController`, уведомляя госслужащего о том, что заявка не существует.

5. **Валидация нового статуса:**
   - В `CivilServantService` выполняется валидация нового статуса заявки. Если статус является допустимым, процесс продолжается. В случае недопустимого статуса, система возвращает ошибку о недопустимом статусе.

6. **Обновление статуса заявки:**
   - Если статус допустим, в `CivilServantService` выполняется запрос на обновление статуса заявки в базе данных: `UPDATE Applications SET status = newStatus WHERE id = applicationId`.
   - После успешного обновления, система возвращает сообщение о том, что статус был успешно изменен.

7. **Ответ госслужащему:**
   - `CivilServantController` получает результат из `CivilServantService` и отправляет уведомление госслужащему о том, что статус заявки был успешно обновлен.

8. **Ошибка при недопустимом статусе или отсутствии заявки:**
   - Если статус заявки недопустим или заявка не найдена, возвращаются ошибки, и госслужащий получает уведомление об ошибке.

---
