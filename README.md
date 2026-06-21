### Project Manager API

REST API zbudowane w .NET 8, służące do zarządzania projektami oraz zadaniami. System wykorzystuje Entity Framework Core do obsługi bazy danych oraz Microsoft Identity i JWT do bezpiecznego uwierzytelniania użytkowników.

### Technologie
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core (SQL Server / LocalDB)
- Microsoft Identity
- JSON Web Tokens (JWT)
- **Swashbuckle.AspNetCore (Swagger UI)**

### Uruchomienie projektu

1. Upewnij się, że masz zainstalowane .NET 8 SDK.
2. W terminalu, w głównym folderze projektu przywróć pakiety komendą: `dotnet restore`.
3. Utwórz bazę danych komendą: `dotnet ef database update`.
4. Uruchom serwer komendą: `dotnet run`.

### Testowanie API (Swagger)

Aplikacja posiada wbudowany interfejs graficzny do testowania (Swagger).
1. Po uruchomieniu serwera wejdź w przeglądarce pod adres: `http://localhost:<PORT>/swagger`
2. Zarejestruj się i zaloguj, używając endpointów z sekcji **Auth**.
3. Skopiuj wygenerowany token, kliknij zielony przycisk **Authorize** na górze strony i wklej go w formacie: `Bearer <TWÓJ_TOKEN>`.

### Zasady dostępu (Logika biznesowa)

Aplikacja jest bezstanowa. Po zalogowaniu otrzymujesz token JWT, który musisz dołączać do nagłówków HTTP (`Authorization: Bearer <TWÓJ_TOKEN>`) dla chronionych endpointów.

- **Widoczność projektów**: Widzisz tylko te projekty, których jesteś właścicielem, LUB te, w których masz przypisane jakiekolwiek zadanie.
- **Uprawnienia właściciela**: Tylko właściciel projektu może go modyfikować, usuwać, oraz dodawać lub usuwać powiązane z nim zadania.
- **Uprawnienia wykonawcy**: Osoba przypisana do zadania (Assignee) może jedynie aktualizować jego status (np. oznaczyć jako zrobione).

### Lista Endpointów

#### 1. Autoryzacja (Auth)
*(Nie wymagają tokenu)*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| **POST** | `/api/auth/register` | Rejestracja użytkownika (wymaga JSON z username, email, password). |
| **POST** | `/api/auth/login` | Logowanie użytkownika. Przy poprawnych danych zwraca token JWT. |

#### 2. Projekty 
*(Wymagają tokenu JWT)*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| **GET** | `/api/projekty` | Pobiera listę projektów dostępnych dla zalogowanego użytkownika. |
| **POST** | `/api/projekty` | Tworzy nowy projekt. Zalogowany użytkownik automatycznie zostaje właścicielem. |
| **PUT** | `/api/projekty/{id}` | Aktualizuje nazwę i opis wskazanego projektu. |
| **DELETE** | `/api/projekty/{id}` | Usuwa projekt bezpowrotnie. |

#### 3. Zadania
*(Wymagają tokenu JWT)*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| **GET** | `/api/zadania` | Pobiera wszystkie zadania dostępne dla zalogowanego użytkownika. |
| **GET** | `/api/zadania/{id}` | Zwraca szczegóły konkretnego zadania na podstawie jego ID. |
| **POST** | `/api/zadania` | Dodaje nowe zadanie do wskazanego w JSON-ie projektu. |
| **PUT** | `/api/zadania/{id}` | Aktualizuje dane zadania. |
| **DELETE** | `/api/zadania/{id}` | Usuwa wskazane zadanie. |