# ApiChatCSharp: Chat365 MVP Backend

The backend of the **Chat365 MVP**, a workplace messaging app. It is an ASP.NET Core (.NET 6) REST API for accounts, contacts, one-to-one and group conversations, messages, files, comments and notifications. Real-time delivery goes through a separate Socket.IO server, which this API notifies whenever something changes.

## Features

| Area | Controller | Endpoints | Examples |
| --- | --- | ---: | --- |
| Accounts and contacts | `UserController` | 84 | login, QR login, profile, friend requests, contacts in the same company, OTP |
| Conversations | `ConversationController` | 42 | list and search conversations, groups and members, unread counts, secret conversations |
| Messages | `MessageController` | 24 | send, list (paged), favourite messages, link previews, messages from partner apps |
| Notifications | `NotificationController` | 40 | system and HR notifications (salary, offers, calendar, reports) |
| Files and avatars | `FileController` | 14 | upload and download files, thumbnails, user and group avatars |
| Comments | `CommentController` | 5 | add, list, edit and delete comments |
| Tags | `TagClassifyController` | 3 | conversation tags |
| QR codes | `QRController` | 1 | read the data behind a login or add-friend QR code |
| App downloads | `InstallController` | 4 | serve the Windows installers of the client apps |
| Maintenance | `RunToolController` | 20 | one-off data migration and clean-up jobs |

All routes follow `/<Controller>/<Action>`, for example `POST /Message/SendMessage`.

### Real-time events

After a change, the API emits an event to the Socket.IO server, which pushes it to connected clients. Main events: `SendMessage`, `ReadMessage`, `SendNotification`, `AcceptRequestAddFriend`, `checkonlineUser`, `QRLogin`, `Logout`, `LogoutAllDevice` and `changedPassword`.

## Tech stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core MVC, .NET 6 (C#) |
| Database | MongoDB (MongoDB.Driver 2.17) |
| Real-time | SocketIOClient, connected to an external Socket.IO server |
| Images | SixLabors.ImageSharp, System.Drawing.Common |
| Other | Newtonsoft.Json, HtmlAgilityPack (link previews), FastMember |
| SMS OTP | eSMS REST API |
| Hosting | IIS (`web.config`) |

## Project layout

```text
Chat365/
  Controllers/        REST controllers
  Model/DAO/          MongoDB data access (ConnectDB.cs holds the connection)
  Model/MongoEntity/  MongoDB documents
  Model/Entity/       domain models
  Model/EntityAPI/    request and response DTOs
  Models/             view models and a Quartz job
  Views/              MVC pages for app downloads
  wwwroot/            static files; uploads are written here at runtime
XFUploadFile.sln
```

## Configuration

* **MongoDB:** `mongodb://localhost:27017`, database `Chat365`. Set in `Chat365/Model/DAO/ConnectDB.cs`.
* **Socket.IO server and partner APIs:** URLs are set in the controllers.
* **Environment variables:**

| Variable | Used for |
| --- | --- |
| `ESMS_API_KEY` | eSMS API key for OTP messages |
| `ESMS_SECRET_KEY` | eSMS secret key |
| `CHAT365_SECRET_CODE` | Shared code that lets trusted services read a user's extra account fields |

## Build and run

Requires the .NET 6 SDK and a running MongoDB.

```bash
dotnet restore Chat365/APIChat365.csproj
dotnet run --project Chat365/APIChat365.csproj
```

The `Chat365` launch profile listens on `https://localhost:5001` and `http://localhost:5000`. File storage paths use Windows separators, so the upload endpoints are meant to run on Windows or IIS.

## Known limitations

This is MVP code:

* There is no authentication middleware; callers identify users by ID in the request.
* Passwords are hashed with unsalted MD5.
* Hosts and connection strings are hardcoded rather than read from `appsettings.json`.
* There are no automated tests.

## License

[MIT](LICENSE)
