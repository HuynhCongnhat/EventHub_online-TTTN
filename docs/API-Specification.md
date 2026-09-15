# EventHub API Specification

## 1. Tổng quan

EventHub sử dụng kiến trúc Microservices.

Các API được chia theo từng Microservice:

- AuthService
- EventService
- BookingService
- PaymentService
- NotificationService

Authentication sử dụng JWT Bearer Token.

---

# 2. AuthService

Base URL:

```text
http://localhost:5001

## 2.1 Đăng ký tài khoản

Request
	POST /api/Auth/register
Body
	{
	  "email": "customer@example.com",
	  "password": "123456",
	  "fullName": "Nguyen Van A"
	}
Response 200
	{
	  "message": "Đăng ký tài khoản thành công.",
	  "userId": "uuid",
	  "email": "customer@example.com",
	  "fullName": "Nguyen Van A",
	  "role": "Customer"
	}
### Ghi chú
	Tài khoản đăng ký thông thường được gán role Customer.


# 3. Đăng nhập
Request
	POST /api/Auth/login
Body
	{
	  "email": "customer@example.com",
	  "password": "123456"
	}
Response 200
	{
	  "message": "Đăng nhập thành công",
	  "accessToken": "JWT_TOKEN",
	  "userId": "uuid",
	  "email": "customer@example.com",
	  "fullName": "Nguyen Van A",
	  "role": "Customer"
	}

# 4. Lấy thông tin người dùng hiện tại
Request
	GET /api/Auth/me
Authorization
	Authorization: Bearer {accessToken}
Response 200
	{
	  "userId": "uuid",
	  "email": "customer@example.com",
	  "fullName": "Nguyen Van A",
	  "role": "Customer"
	}
Response 401
	Unauthorized


# 5. EventService

Base URL dự kiến:

	http://localhost:5002

## 5.1 Lấy danh sách sự kiện
	GET /api/events

## 5.2 Lấy thông tin sự kiện
	GET /api/events/{id}

## 5.3 Tạo sự kiện
	POST /api/events

Authorization:
	Bearer Token
	Role: Admin

## 5.4 Cập nhật sự kiện
	PUT /api/events/{id}

Authorization:
	Bearer Token
	Role: Admin

## 5.5 Xóa sự kiện
	DELETE /api/events/{id}

Authorization:
	Bearer Token
	Role: Admin


# 6. BookingService

Base URL dự kiến:

http://localhost:5003

## 6.1 Tạo booking
	POST /api/bookings

Authorization:
	Bearer Token
	Role: Customer

## 6.2 Lấy booking
	GET /api/bookings/{id}

Authorization:
	Bearer Token

## 6.3 Lấy danh sách booking của người dùng
	GET /api/bookings/my-bookings

Authorization:
	Bearer Token

## 6.4 Hủy booking
	PUT /api/bookings/{id}/cancel

Authorization:
	Bearer Token

# 7. PaymentService

Base URL dự kiến:

http://localhost:5004

## 7.1 Tạo thanh toán VNPay
	POST /api/payments/create

Authorization:
	Bearer Token

## 7.2 Nhận kết quả thanh toán VNPay
	GET /api/payments/vnpay-return

## 7.3 Kiểm tra trạng thái thanh toán
	GET /api/payments/{id}

Authorization:
	Bearer Token

# 8. NotificationService

Base URL dự kiến:

http://localhost:5005

## 8.1 Lấy thông báo của người dùng
	GET /api/notifications

Authorization:
	Bearer Token

## 8.2 Đánh dấu thông báo đã đọc
	PUT /api/notifications/{id}/read

Authorization:
	Bearer Token

# 9. Authentication

Các API yêu cầu xác thực sử dụng:

Authorization: Bearer {JWT}

JWT chứa các thông tin:

sub
email
name
role
iss
aud
exp

# 10. Authorization

Các role chính:

Role	Quyền
Admin	Quản lý sự kiện và hệ thống
Customer	Xem sự kiện, đặt vé và thanh toán

# 11. HTTP Status Codes
Status	Ý nghĩa
200	Thành công
201	Tạo mới thành công
400	Request không hợp lệ
401	Chưa xác thực
403	Không có quyền
404	Không tìm thấy
409	Dữ liệu bị trùng
500	Lỗi server