# EventHub – Hệ thống đặt vé sự kiện trực tuyến

## 1. Giới thiệu

**EventHub** là hệ thống đặt vé sự kiện trực tuyến được xây dựng nhằm hỗ trợ người dùng tìm kiếm, xem thông tin và đặt vé các sự kiện như:

- Concert / Ca nhạc
- Hội thảo
- Workshop
- Sự kiện thể thao
- Lễ hội
- Chương trình giải trí

Người dùng có thể lựa chọn sự kiện, loại vé hoặc vị trí ghế, thực hiện đặt vé và thanh toán trực tuyến thông qua VNPay. Sau khi thanh toán thành công, hệ thống xác nhận booking và cung cấp vé điện tử cho người dùng.

Hệ thống được xây dựng theo kiến trúc **Microservices** nhằm tăng khả năng mở rộng, bảo trì và phát triển độc lập từng chức năng.

---

## 2. Mục tiêu dự án

Dự án hướng đến các mục tiêu:

- Xây dựng hệ thống đặt vé sự kiện trực tuyến.
- Quản lý người dùng và phân quyền.
- Quản lý sự kiện, địa điểm và loại vé.
- Hỗ trợ đặt vé và quản lý booking.
- Hỗ trợ thanh toán trực tuyến thông qua VNPay.
- Cung cấp vé điện tử sau khi thanh toán thành công.
- Cập nhật trạng thái vé/ghế theo thời gian thực.
- Gửi thông báo cho người dùng.
- Nghiên cứu tích hợp AI Chatbot sử dụng RAG.
- Nghiên cứu hệ thống Recommendation Engine để đề xuất sự kiện phù hợp.

---

## 3. Chức năng chính

### 3.1. Người dùng

Người dùng có thể:

- Đăng ký tài khoản.
- Đăng nhập.
- Đăng xuất.
- Xem thông tin tài khoản.
- Xem danh sách sự kiện.
- Xem chi tiết sự kiện.
- Lựa chọn loại vé hoặc vị trí ghế.
- Đặt vé.
- Thanh toán trực tuyến.
- Xem lịch sử đặt vé.
- Xem vé điện tử.
- Nhận thông báo.

### 3.2. Quản trị viên

Admin có thể:

- Quản lý người dùng.
- Quản lý danh mục sự kiện.
- Quản lý sự kiện.
- Quản lý địa điểm.
- Quản lý loại vé.
- Quản lý ghế.
- Quản lý booking.
- Theo dõi thanh toán.
- Quản lý thông báo.

### 3.3. AI Chatbot

AI Chatbot được nghiên cứu theo hướng **RAG (Retrieval-Augmented Generation)** nhằm hỗ trợ người dùng:

- Tìm kiếm thông tin sự kiện.
- Hỏi thông tin về sự kiện.
- Hỏi thông tin địa điểm.
- Hỏi thông tin loại vé.
- Hỗ trợ lựa chọn sự kiện phù hợp.
- Trả lời dựa trên dữ liệu thực tế của hệ thống.

### 3.4. Recommendation Engine

Recommendation Engine được nghiên cứu nhằm đề xuất sự kiện phù hợp với người dùng dựa trên:

- Danh mục sự kiện.
- Lịch sử xem.
- Lịch sử đặt vé.
- Sở thích của người dùng.
- Địa điểm.
- Thời gian.
- Mức độ phổ biến của sự kiện.

---

## 4. Kiến trúc hệ thống

EventHub sử dụng kiến trúc Microservices.

```text
                         +------------------+
                         |   EventHub.Web   |
                         |    Blazor Web    |
                         +--------+---------+
                                  |
                                  v
                    +-------------------------+
                    |   EventHub.ApiGateway   |
                    |          YARP           |
                    +-----------+-------------+
                                |
             +------------------+------------------+
             |         |          |        |       |
             v         v          v        v       v
        +---------+ +---------+ +---------+ +---------+ +--------------+
        |  Auth   | |  Event  | | Booking | | Payment | | Notification |
        | Service | | Service | | Service | | Service | |   Service    |
        +----+----+ +----+----+ +----+----+ +----+----+ +------+-------+
             |           |           |           |              |
             v           v           v           v              v
        PostgreSQL    Database     Database    VNPay       Notification



## 6. Cấu trúc thư mục

    EventHub/
    │
    ├── docs/
    │   ├── database-schema.md
    │   ├── API-Specification.md
    │   ├── AI-Architecture.md
    │   └── Recommendation-Architecture.md
    │
    ├── docker/
    │
    ├── src/
    │   ├── EventHub.slnx
    │   │
    │   ├── EventHub.Web/
    │   │
    │   ├── EventHub.ApiGateway/
    │   │
    │   ├── EventHub.AuthService/
    │   │
    │   ├── EventHub.EventService/
    │   │
    │   ├── EventHub.BookingService/
    │   │
    │   ├── EventHub.PaymentService/
    │   │
    │   ├── EventHub.NotificationService/
    │   │
    │   └── EventHub.Contracts/
    │
    ├── docker-compose.yml
    ├── .gitignore
    └── README.md

## 7. Công nghệ sử dụng
- Backend
    + ASP.NET Core
    + C#
    + Entity Framework Core
    + Microservices Architecture
    + RESTful API
    + JWT Authentication
    + BCrypt Password Hashing
- Frontend
    + Blazor
    + ASP.NET Core
- Database
    + PostgreSQL
    + Entity Framework Core
    + Npgsql
- API Gateway
    + YARP (Yet Another Reverse Proxy)
- Authentication
    + JWT Bearer Token
    + Role-based Authorization
    + BCrypt
- Payment
    + VNPay Sandbox
- Real-time
    + SignalR
- AI
    + RAG Chatbot
    + Vector Database
    + Recommendation Engine
- Development & Deployment
    + Docker
    + Docker Compose
    + Git
    + GitHub
    + Swagger / OpenAPI

## 8. Database

Hệ thống sử dụng PostgreSQL làm cơ sở dữ liệu.

Các nhóm dữ liệu chính:

    - AuthService
    - Roles
    - Users
    - RefreshTokens
    - EventService
    - Categories
    - Venues
    - Events
    - TicketTypes
    - Seats
    - EventSeats
    - BookingService
    - Bookings
    - BookingItems
    - Tickets
    - PaymentService
    - Payments
    - NotificationService
    - Notifications

### Chi tiết database schema được trình bày tại: docs/database-schema.md

## 9. Authentication và Authorization

EventHub sử dụng JWT để xác thực người dùng.

Quy trình:

        User
          |
          | Login
          v
        AuthService
          |
          | Verify email/password
          v
        Generate JWT
          |
          v
        Client
          |
          | Authorization: Bearer <token>
          v
        API Gateway / Microservices

Hệ thống hỗ trợ các role:
    - Admin
    - Customer

Người dùng đăng ký thông thường chỉ được tạo tài khoản với role: Customer

Tài khoản Admin được quản lý riêng và không cho phép người dùng tự đăng ký với quyền Admin.

Mật khẩu được hash bằng: BCrypt

## 10. Docker Compose

Docker Compose được sử dụng để chạy các thành phần cần thiết trong môi trường local.

Hiện tại Docker Compose bao gồm:
- PostgreSQL
- AuthService

PostgreSQL
    Database: eventhub_auth
    Username: eventhub
    Password: eventhub123
    Port: 5432

AuthService
    http://localhost:5001

Swagger
Swagger của AuthService: http://localhost:5001/swagger

## 11. Chạy hệ thống bằng Docker Compose

Từ thư mục gốc của project:

    cd D:\EventHub

Khởi động các container:

    docker compose up -d

Kiểm tra container:

    docker compose ps

Xem log:

    docker compose logs

Xem log riêng AuthService:

    docker compose logs authservice

Dừng hệ thống:

    docker compose down

## 12. AuthService API
Đăng ký
    POST /api/Auth/register

Request:

    {
      "email": "user@gmail.com",
      "password": "123456",
      "fullName": "Nguyen Van A"
    }

Người dùng đăng ký thành công sẽ được tạo với role: Customer

Đăng nhập
    POST /api/Auth/login

Request:

    {
      "email": "user@gmail.com",
      "password": "123456"
    }

Response trả về JWT:

    {
      "message": "Đăng nhập thành công",
      "accessToken": "JWT_TOKEN",
      "userId": "USER_ID",
      "email": "user@gmail.com",
      "fullName": "Nguyen Van A",
      "role": "Customer"
    }

Lấy thông tin người dùng hiện tại
    GET /api/Auth/me

Header:

    Authorization: Bearer <JWT_TOKEN>

Response:

    {
      "userId": "USER_ID",
      "email": "user@gmail.com",
      "fullName": "Nguyen Van A",
      "role": "Customer"
    }

Chi tiết API được trình bày tại: docs/API-Specification.md

## 13. Event Flow

Quy trình đặt vé cơ bản:

    Người dùng
        |
        v
    Xem danh sách sự kiện
        |
        v
    Chọn sự kiện
        |
        v
    Chọn loại vé / vị trí ghế
        |
        v
    Tạo Booking
        |
        v
    Thanh toán VNPay
        |
        v
    Thanh toán thành công
        |
        v
    Xác nhận Booking
        |
        v
    Tạo vé điện tử
        |
        v
    Gửi thông báo

## 14. Real-time với SignalR

SignalR được nghiên cứu và sử dụng để hỗ trợ cập nhật trạng thái vé/ghế theo thời gian thực.

Ví dụ:

User A                Server                User B
  |                      |                     |
  | Chọn ghế A01        |                     |
  |--------------------->|                     |
  |                      |                     |
  |                      | SignalR             |
  |                      |-------------------->|
  |                      |                     |
  |                      |              A01 đã được giữ

Mục tiêu là hạn chế tình trạng nhiều người dùng cùng lựa chọn hoặc đặt một vị trí ghế.

## 15. AI Chatbot – RAG

AI Chatbot được thiết kế theo kiến trúc RAG:

                 +----------------+
                 |     User       |
                 +-------+--------+
                         |
                         v
                 +----------------+
                 |  AI Chatbot    |
                 +-------+--------+
                         |
                         v
                 +----------------+
                 |   RAG Service  |
                 +-------+--------+
                         |
                         v
                 +----------------+
                 |  Vector DB     |
                 +-------+--------+
                         |
                         v
                Relevant Context
                         |
                         v
                 +----------------+
                 |      LLM       |
                 +-------+--------+
                         |
                         v
                    AI Response

Nguồn dữ liệu có thể sử dụng:

    Events
    Categories
    Venues
    TicketTypes
    Thông tin sự kiện

Chi tiết được trình bày tại: docs/AI-Architecture.md

## 16. Recommendation Engine

Recommendation Engine hướng đến việc đề xuất sự kiện phù hợp với người dùng.

Các yếu tố có thể sử dụng:
    Category
    Location
    User Behavior
    Booking History
    Search History
    Popularity
    Time

Kiến trúc dự kiến:

    User Behavior
          |
          v
    Data Collection
          |
          v
    Feature Processing
          |
          v
    Recommendation Engine
          |
          v
    Recommended Events
          |
          v
    EventHub.Web

Các hướng tiếp cận:

    Rule-based Recommendation
    Content-based Filtering
    Collaborative Filtering
    Hybrid Recommendation

Chi tiết được trình bày tại: docs/Recommendation-Architecture.md

## 17. Documentation

Các tài liệu của dự án:

Tài liệu	         |  Nội dung
database-schema.md	 | Database schema
API-Specification.md  | API specification
AI-Architecture.md	  | Kiến trúc AI Chatbot RAG
Recommendation-Architecture.md	| Kiến trúc Recommendation Engine
