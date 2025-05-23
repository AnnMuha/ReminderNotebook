﻿# ReminderNotebook

Розумний електронний записник з нагадуваннями  
Фінальний проєкт з програмування, реалізований на C#, WPF та архітектурі MVVM

---

## ✨ Можливості

- Додавання, редагування та видалення нотаток з нагадуваннями
- Пріоритет (Low / Medium / High) і дата виконання
- Сповіщення при настанні часу (MessageBox)
- Пошук і фільтрація за пріоритетом, статусом виконання та текстом
- Сортування нагадувань (новіші/старіші/пріоритет)
- Позначення "Виконано" та підсвітка завершених
- Вікно аналітики (звіт за статусом)
- Автоматичне збереження даних у форматі JSON
- Чистий код і використання патернів проєктування

---

## 💻 Використані технології

- .NET 6 / 7
- C#
- WPF (Windows Presentation Foundation)
- MVVM (Model-View-ViewModel)
- DispatcherTimer
- JSON (System.Text.Json)

---

## 🔧 Етапи реалізації

### ✅ Етап 1 — Логіка та структура MVVM
- Створено моделі: `Reminder`, `Note`
- Реалізовано `MainViewModel` з прив'язками до команд
- Додано підтримку `ObservableCollection`, INotifyPropertyChanged

### ✅ Етап 2 — Головне вікно (MainWindow.xaml)
- Інтерфейс зі списком нагадувань
- Кнопки: ➕ Додати, ✏️ Редагувати, 🗑️ Видалити
- Прив’язка `MainViewModel` через `DataContext`

### ✅ Етап 2.2 — Додавання нового нагадування
- Створено `AddReminderWindow.xaml`
- Передача Reminder через конструктор
- Валідація дати та часу вручну

### ✅ Етап 3 — Редагування нагадувань
- Передача копії Reminder для редагування
- Заміна старого Reminder у списку
- Збереження змін у JSON

### ✅ Етап 4 — Збереження даних
- Створено `StorageService` для збереження/завантаження JSON
- Дані автоматично зберігаються після змін

### ✅ Етап 5 — Видалення нагадування
- Реалізовано `DeleteReminder()`
- Автоматичне оновлення фільтра після видалення

### ✅ Етап 6.1 — Відображення пріоритету
- Додано `PriorityColorConverter`
- Пріоритети відображаються кольором (зелений/оранжевий/червоний)

### ✅ Етап 6.2 — Фільтрація за пріоритетом
- Додано ComboBox для вибору (All / Low / Medium / High)
- Виводиться лише фільтрований список `FilteredReminders`

### ✅ Етап 7.1 — Таймер перевірки нагадувань
- `DispatcherTimer` перевіряє кожні 10 сек
- Спрацьовує при збігу часу

### ✅ Етап 7.2 — Показ повідомлень
- Створено клас `Notifier`
- Виводить `MessageBox` при спрацюванні нагадування

### ✅ Етап 7.3 — Патерн Observer
- Створено інтерфейс `IReminderObserver`
- `MainViewModel` є Subject, `Notifier` — Observer
- Реалізовано підписку/відписку та `Notify(reminder)`

### ✅ Етап 8.1 — Пошук та сортування**
- 🔍 Додано **пошук** за назвою та описом
- 📥 Реалізовано **сортування** нагадувань:
  - `Newest first` — найновіші вгорі
  - `Oldest first` — найстаріші вгорі
  - `By priority` — за рівнем пріоритету

### ✅ Етап 8.2 — Очищення фільтрів**
- 🧹 Додано кнопку **очистити фільтри**
  - Скидає вибрані фільтри за пріоритетом, статусом та поле пошуку

### ✅ Етап 8.3 — Фільтрація за статусом**
- ✅ Додано **фільтр за статусом виконання**:
  - `Completed` — виконані нагадування
  - `Pending` — не виконані
  - `All` — всі нагадування

### ✅ Етап 8.4 — Відмітка виконання та підсвітка**
- 📌 Додано **чекбокс для позначення "виконано"** у кожному нагадуванні
- 🔄 Зміни зберігаються автоматично
- 🎨 **Завершені нагадування** мають інший стиль (сірий фон, закреслення)

### ✅ Етап 8.5 — Вікно статистики**
- 📊 Додано **аналітичне вікно (ReportWindow)**:
  - Загальна кількість нагадувань
  - Скільки виконано та невиконано
  - Кількість нагадувань за останні 7 днів

---

## 🧠 Programming Principles

- **DRY** — уникнення повторення (ApplyFilter, Notify, Save)
- **KISS** — простота UI та логіки
- **SOLID**:
  - **S** — чіткий розподіл обов’язків між класами
  - **O** — легко розширити/змінити Notifier або Storage
  - **D** — використання інтерфейсу `IReminderObserver`
- **Composition over Inheritance** — `MainViewModel` використовує сервіси та інтерфейси

---

## 🧩 Design Patterns

| Патерн       | Де застосовано                             |
|--------------|---------------------------------------------|
| **Observer** | `MainViewModel` → `Notifier`                |
| **Command**  | Команди `AddCommand`, `DeleteCommand`, `EditCommand` |
| **Singleton (опційно)** | `StorageService` як stateless utility           |

---

## 🔄 Refactoring Techniques

- Виділення окремих сервісів (`StorageService`, `Notifier`)
- Робота з ObservableCollection замість List
- `INotifyPropertyChanged` для відображення змін у UI
- Валідація нагадувань при додаванні/редагуванні
- Таймер винесено у ViewModel, логіка перевірки — централізована

---

## 🚀 Запуск

Щоб запустити проєкт на локальному комп'ютері:

1. 📥 **Склонуй або завантаж репозиторій**
   - Через Git:
     ```bash
     git clone https://github.com/AnnMuha/ReminderNotebook.git
     ```
   - Або завантаж ZIP-архів через GitHub

2. 🧠 **Відкрий проєкт у Visual Studio**
   - Запусти файл рішення: `ReminderNotebook.sln`

3. ⚙️ **Переконайся, що обрано конфігурацію Debug**
   - Платформа: `x64` або `Any CPU`
   - Цільова версія: `.NET 6` або `.NET 7`

4. ▶️ **Натисни F5 або кнопку Start (зелена стрілка)**
   - Додаток відкриється як настільне вікно з інтерфейсом WPF

5. ✅ **Використовуй функціонал:**
   - ➕ Додавай нагадування з датою, часом і пріоритетом
   - 🔍 Фільтруй за важливістю
   - ✏️ Редагуй або 🗑️ видаляй записи
   - 🔔 Очікуй автоматичне повідомлення при настанні часу

---

## 🖼 Основний інтерфейс програми

Головне вікно з записами:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/1.jpg)

Пошук нагадувань:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/2.jpg)

Фільтрація по пріоритетам:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/3.jpg)

Сортування нагадувань:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/4.jpg)

Фільтр за статусом:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/5.jpg)

Редагування нагадування:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/6.jpg)

Спрацювання нагадування:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/7.jpg)

Видалення нагадування:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/8.jpg)

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/9.jpg)

Додавання нагадування:

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/10.jpg)

Звіт :

[Переглянути](https://github.com/AnnMuha/ReminderNotebook/blob/master/screenshots/11.jpg)

---

## 📁 Дані

🔒 Дані зберігаються у `reminders.json` у корені проєкту автоматично — навіть після закриття програми твої нагадування залишаються.

---
