# EventHub Database Schema

## 1. Tổng quan

EventHub sử dụng PostgreSQL làm hệ quản trị cơ sở dữ liệu.

Hệ thống được thiết kế theo kiến trúc Microservices. Mỗi Microservice quản lý
nhóm dữ liệu thuộc phạm vi trách nhiệm của mình.

Các nhóm database chính:

- AuthService
- EventService
- BookingService
- PaymentService
- NotificationService

---

# 2. AuthService Database

## 2.1 Roles

Lưu thông tin các vai trò của người dùng trong hệ thống.

| Column | Data Type | Constraint | Description |
|---|---|---|---|
| Id | integer | PK | Mã vai trò |
| Name | varchar(50) | NOT NULL, UNIQUE | Tên vai trò |

### Dữ liệu mẫu

| Id | Name |
|---:|---|
| 1 | Admin |
| 2 | Customer |

---

## 2.2 Users

Lưu thông tin tài khoản người dùng.

| Column | Data Type | Constraint | Description |
|--------|-----------|------------|-------------|
|     Id |      uuid |         PK | Mã người dùng |
| Email  |varchar(255)| NOT NULL, UNIQUE | Email đăng nhập |
| PasswordHash | varchar | NOT NULL | Mật khẩu đã được hash bằng BCrypt |
| FullName | varchar(150) | NOT NULL | Họ và tên |
| RoleId | integer | FK | Vai trò của người dùng |
| CreatedAt | timestamp | NOT NULL | Thời gian tạo tài khoản |

### Relationships

```text
Roles 1 ──────── N Users

## 2.3 RefreshTokens

Lưu Refresh Token của người dùng.

Column |	Data Type |	Constraint|	Description|
Id|	uuid	|PK|	Mã Refresh | Token
UserId|	uuid	|FK, NOT NULL	|Người sở hữu token
Token| 	varchar(500)|	NOT NULL, UNIQUE|	Refresh Token
ExpiresAt|	timestamp|	NOT NULL|	Thời gian hết hạn
RevokedAt|	timestamp|	NULL	|Thời gian thu hồi

### Relationships

Users 1 ──────── N RefreshTokens

# 3. EventService Database
## 3.1 Categories

Lưu danh mục sự kiện.

Column	Data Type	Constraint	Description
Id	integer	PK	Mã danh mục
Name	varchar(100)	NOT NULL	Tên danh mục
Description	text	NULL	Mô tả danh mục

Dữ liệu mẫu
Concert
Workshop
Seminar
Sports
Festival
Entertainment


## 3.2 Venues

Lưu thông tin địa điểm tổ chức sự kiện.

Column	Data Type	Constraint	Description
Id	integer	PK	Mã địa điểm
Name	varchar(200)	NOT NULL	Tên địa điểm
Address	varchar(255)	NOT NULL	Địa chỉ
City	varchar(100)	NOT NULL	Thành phố
Capacity	integer	NOT NULL	Sức chứa


## 3.3 Events

Lưu thông tin các sự kiện.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã sự kiện
Name	varchar(200)	NOT NULL	Tên sự kiện
Description	text	NULL	Mô tả
StartTime	timestamp	NOT NULL	Thời gian bắt đầu
EndTime	timestamp	NOT NULL	Thời gian kết thúc
ImageUrl	varchar(500)	NULL	URL hình ảnh
CategoryId	integer	FK	Danh mục
VenueId	integer	FK	Địa điểm
Status	varchar(50)	NOT NULL	Trạng thái
CreatedAt	timestamp	NOT NULL	Thời gian tạo

### Relationships
Categories 1 ──────── N Events

Venues 1 ──────── N Events



## 3.4 TicketTypes

Lưu các loại vé của một sự kiện.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã loại vé
EventId	uuid	FK, NOT NULL	Sự kiện
Name	varchar(100)	NOT NULL	Tên loại vé
Price	decimal(18,2)	NOT NULL	Giá vé
Quantity	integer	NOT NULL	Số lượng
Description	text	NULL	Mô tả

### Relationships
Events 1 ──────── N TicketTypes


## 3.5 Seats

Lưu thông tin ghế tại địa điểm.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã ghế
VenueId	integer	FK, NOT NULL	Địa điểm
SeatNumber	varchar(20)	NOT NULL	Số ghế
Row	varchar(20)	NOT NULL	Hàng ghế
Section	varchar(50)	NULL	Khu vực

### Relationships
Venues 1 ──────── N Seats


## 3.6 EventSeats

Quản lý trạng thái ghế theo từng sự kiện.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã
EventId	uuid	FK, NOT NULL	Sự kiện
SeatId	uuid	FK, NOT NULL	Ghế
TicketTypeId	uuid	FK, NOT NULL	Loại vé
Status	varchar(30)	NOT NULL	Trạng thái ghế

Giá trị Status
Available
Held
Booked

### Relationships
Events 1 ──────── N EventSeats

Seats 1 ──────── N EventSeats

TicketTypes 1 ──────── N EventSeats

EventSeats là bảng quan trọng để quản lý trạng thái ghế theo từng sự kiện
và hỗ trợ cơ chế giữ ghế realtime bằng SignalR.

# 4. BookingService Database
## 4.1 Bookings

Lưu thông tin đơn đặt vé.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã booking
UserId	uuid	FK	Người đặt
EventId	uuid	FK	Sự kiện
BookingCode	varchar(50)	NOT NULL, UNIQUE	Mã booking
TotalAmount	decimal(18,2)	NOT NULL	Tổng tiền
Status	varchar(30)	NOT NULL	Trạng thái
CreatedAt	timestamp	NOT NULL	Thời gian tạo
ExpiresAt	timestamp	NULL	Thời gian hết hạn giữ vé

### Relationships
Users 1 ──────── N Bookings

Events 1 ──────── N Bookings


## 4.2 BookingItems

Lưu chi tiết các vé trong một booking.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã chi tiết
BookingId	uuid	FK, NOT NULL	Booking
TicketTypeId	uuid	FK, NOT NULL	Loại vé
EventSeatId	uuid	FK, NULL	Ghế sự kiện
Quantity	integer	NOT NULL	Số lượng
UnitPrice	decimal(18,2)	NOT NULL	Đơn giá
Amount	decimal(18,2)	NOT NULL	Thành tiền

### Relationships
Bookings 1 ──────── N BookingItems

TicketTypes 1 ──────── N BookingItems

EventSeats 1 ──────── N BookingItems

EventSeatId có thể NULL đối với các loại vé không gắn với ghế cụ thể.

# 5. PaymentService Database
## 5.1 Payments

Lưu thông tin giao dịch thanh toán.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã thanh toán
BookingId	uuid	FK	Booking
TransactionCode	varchar(100)	NOT NULL	Mã giao dịch
Amount	decimal(18,2)	NOT NULL	Số tiền
PaymentMethod	varchar(50)	NOT NULL	Phương thức thanh toán
Status	varchar(30)	NOT NULL	Trạng thái
PaidAt	timestamp	NULL	Thời gian thanh toán
CreatedAt	timestamp	NOT NULL	Thời gian tạo

Dữ liệu mẫu
PaymentMethod = VNPay

Status:
- Pending
- Success
- Failed
- Cancelled

### Relationships
Bookings 1 ──────── N Payments


# 6. Ticket / E-ticket Database
## 6.1 Tickets

Lưu vé điện tử sau khi thanh toán thành công.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã vé
BookingItemId	uuid	FK	Chi tiết booking
UserId	uuid	FK	Người sở hữu
TicketCode	varchar(100)	NOT NULL, UNIQUE	Mã vé
QRCode	text	NULL	Dữ liệu QR Code
Status	varchar(30)	NOT NULL	Trạng thái
IssuedAt	timestamp	NOT NULL	Thời gian phát hành
UsedAt	timestamp	NULL	Thời gian sử dụng

### Relationships
BookingItems 1 ──────── N Tickets

Users 1 ──────── N Tickets


# 7. NotificationService Database
## 7.1 Notifications

Lưu thông báo gửi đến người dùng.

Column	Data Type	Constraint	Description
Id	uuid	PK	Mã thông báo
UserId	uuid	FK	Người nhận
Title	varchar(200)	NOT NULL	Tiêu đề
Message	text	NOT NULL	Nội dung
Type	varchar(50)	NOT NULL	Loại thông báo
IsRead	boolean	NOT NULL	Đã đọc hay chưa
CreatedAt	timestamp	NOT NULL	Thời gian tạo

### Relationships
Users 1 ──────── N Notifications


# 8. Tổng hợp Database theo Microservice
    - AuthService
        + Roles
        + Users
        + RefreshTokens
    - EventService
        + Categories
        + Venues
        + Events
        + TicketTypes
        + Seats
        + EventSeats
    - BookingService
        + Bookings
        + BookingItems
    - PaymentService
        + Payments
    - Ticket
        + Tickets
    - NotificationService
        + Notifications


# 9. Tổng quan quan hệ
Roles
  │
  └── 1:N ── Users
                │
                ├── 1:N ── RefreshTokens
                ├── 1:N ── Bookings
                ├── 1:N ── Tickets
                └── 1:N ── Notifications

Categories
  │
  └── 1:N ── Events ── N:1 ── Venues
                │
                ├── 1:N ── TicketTypes
                │
                ├── 1:N ── EventSeats ── N:1 ── Seats
                │                   │
                │                   └── N:1 ── TicketTypes
                │
                └── 1:N ── Bookings
                              │
                              ├── 1:N ── BookingItems
                              │              │
                              │              ├── N:1 ── TicketTypes
                              │              ├── N:1 ── EventSeats
                              │              └── 1:N ── Tickets
                              │
                              └── 1:N ── Payments