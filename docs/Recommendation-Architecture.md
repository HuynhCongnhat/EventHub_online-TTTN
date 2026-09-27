# EventHub Recommendation Engine Architecture

## 1. Mục tiêu

Recommendation Engine có nhiệm vụ đề xuất các sự kiện phù hợp với
sở thích và hành vi của người dùng.

Mục tiêu:

- Cá nhân hóa danh sách sự kiện.
- Tăng khả năng người dùng tìm được sự kiện phù hợp.
- Hỗ trợ khám phá các sự kiện mới.
- Có khả năng mở rộng khi số lượng người dùng và sự kiện tăng.

---

# 2. Dữ liệu đầu vào

Recommendation Engine sử dụng các dữ liệu liên quan đến người dùng và
sự kiện.

## User Behavior

Các hành vi có thể thu thập:

- Xem sự kiện.
- Tìm kiếm sự kiện.
- Xem chi tiết sự kiện.
- Thêm sự kiện vào danh sách yêu thích.
- Đặt vé.
- Hủy booking.
- Lựa chọn danh mục sự kiện.
- Lựa chọn địa điểm.

## Event Data

Các thông tin của sự kiện:

- Tên sự kiện.
- Danh mục.
- Địa điểm.
- Thời gian.
- Loại vé.
- Giá vé.
- Mô tả.

---

# 3. Kiến trúc tổng thể

```text
                    EventHub Web
                         |
                         v
                  Recommendation API
                         |
                         v
                Recommendation Engine
                         |
              +----------+----------+
              |                     |
              v                     v
        User Behavior          Event Data
              |                     |
              +----------+----------+
                         |
                         v
                  Scoring / Model
                         |
                         v
                  Ranked Events
                         |
                         v
                 Recommended Events


# 4. Recommendation Flow
        User
         |
         +---- View Event
         |
         +---- Search Event
         |
         +---- Favorite Event
         |
         +---- Book Ticket
         |
         v
        User Behavior
         |
         v
        Recommendation Engine
         |
         v
        Calculate Score
         |
         v
        Rank Events
         |
         v
        Recommended Events
         |
         v
        User

# 5. Recommendation Score

    Mỗi sự kiện có thể được tính điểm dựa trên nhiều yếu tố.

Ví dụ:

    Recommendation Score =
        Category Score
      + Location Score
      + Behavior Score
      + Popularity Score
      + Time Score

Trong đó:

- Category Score: mức độ phù hợp với danh mục người dùng quan tâm.
- Location Score: mức độ phù hợp với địa điểm người dùng thường quan tâm.
- Behavior Score: mức độ tương đồng với lịch sử tương tác của người dùng.
- Popularity Score: mức độ phổ biến của sự kiện.
- Time Score: mức độ phù hợp về thời gian.



# 6. Các phương pháp Recommendation
## 6.1. Content-Based Filtering

    Đề xuất các sự kiện có đặc điểm tương tự với những sự kiện người dùng đã quan tâm.

### Ví dụ:

        User xem:
            Concert A
            Concert B

        Cả hai thuộc:
            Music / Concert

        Hệ thống đề xuất:
            Concert C

## 6.2. Collaborative Filtering

Đề xuất dựa trên hành vi của những người dùng có sở thích tương tự.

Ví dụ:

    User A và User B có hành vi tương tự.

    User A đã đặt:
        Concert A
        Concert B

    User B đã đặt:
        Concert A

    Hệ thống có thể đề xuất:
        Concert B cho User B

## 6.3. Hybrid Recommendation

Kết hợp nhiều phương pháp:

    Content-Based
          +
     Collaborative Filtering
          +
       Popularity
          +
       Context
          |
          v
    Hybrid Recommendation



# 7. Cold Start Problem

Đối với người dùng mới chưa có lịch sử hoạt động, hệ thống chưa có đủ dữ liệu để cá nhân hóa.

Trong trường hợp này có thể sử dụng:

- Sự kiện phổ biến.
- Sự kiện mới.
- Sự kiện theo thành phố.
- Sự kiện theo danh mục người dùng chọn ban đầu.

Đối với sự kiện mới chưa có dữ liệu tương tác, hệ thống có thể sử dụng thông tin nội dung của sự kiện để đề xuất.

# 8. Kiến trúc triển khai dự kiến
    +-------------------+
    |   EventHub Web    |
    +---------+---------+
              |
              v
    +-------------------------+
    | Recommendation API     |
    +-----------+-------------+
                |
                v
    +-------------------------+
    | Recommendation Engine  |
    +-----------+-------------+
                |
          +-----+-----+
          |           |
          v           v
    +----------+  +----------+
    | Behavior |  |  Events  |
    |   Data   |  |   Data   |
    +----------+  +----------+
          |           |
          +-----+-----+
                |
                v
          Recommendation
             Results

# 9. Tích hợp với EventService

Recommendation Engine không truy cập trực tiếp database của EventService.

Dữ liệu sự kiện được lấy thông qua API hoặc event/message.

        EventService
             |
             | API / Event
             v
        Recommendation Engine

Khi có sự kiện mới hoặc sự kiện được cập nhật, dữ liệu recommendation có thể được cập nhật tương ứng.

# 10. Tích hợp với BookingService

BookingService cung cấp các sự kiện liên quan đến hành vi đặt vé.

        BookingService
              |
              | Booking Event
              v
        Recommendation Engine
              |
              v
        User Behavior

Ví dụ:

    Customer đặt:
        Workshop A

    Recommendation Engine ghi nhận:
        User → Workshop

    Sau đó có thể ưu tiên:
        Workshop B
        Workshop C

# 11. Tích hợp với AI Chatbot

Recommendation Engine và RAG Chatbot có thể hỗ trợ lẫn nhau.

                  User
                   |
          +--------+--------+
          |                 |
          v                 v
     AI Chatbot      Recommendation
          |                 |
          v                 v
         RAG             User Profile
          |                 |
          +--------+--------+
                   |
                   v
             Event Results

Ví dụ:

    User:
    "Tôi thích các chương trình âm nhạc,
    hãy tìm sự kiện phù hợp cho tôi."

    Chatbot
        |
        +--> RAG tìm thông tin sự kiện
        |
        +--> Recommendation Engine
                |
                +--> User preferences
                |
                +--> Event ranking

# 12. Giai đoạn phát triển
Giai đoạn 1 - Rule-Based

    Sử dụng các quy tắc đơn giản:

    Category
    Location
    Popularity
    Time

    Ưu điểm:

    Dễ triển khai.
    Dễ kiểm thử.
    Không cần lượng dữ liệu lớn.
Giai đoạn 2 - Content-Based

    Sử dụng đặc điểm của sự kiện và lịch sử người dùng.

    User Profile
          |
          v
    Event Similarity
          |
          v
    Recommended Events
Giai đoạn 3 - Hybrid / Machine Learning

    Khi hệ thống có đủ dữ liệu:

    User Behavior
          |
          v
    Machine Learning Model
          |
          v
    Recommendation Score
          |
          v
    Ranked Events

# 13. Yêu cầu phi chức năng

Recommendation Engine cần:
- Có khả năng mở rộng.
- Không làm ảnh hưởng trực tiếp đến quá trình đặt vé.
- Có thể cập nhật dữ liệu recommendation độc lập.
- Có khả năng xử lý lượng lớn hành vi người dùng.
- Có thể thay đổi thuật toán recommendation.

# 14. Kết luận

- Recommendation Engine được thiết kế độc lập với các Microservice nghiệp vụ.

- Trong giai đoạn đầu, hệ thống có thể sử dụng Rule-Based và Content-Based Recommendation.

- Khi dữ liệu người dùng tăng lên, có thể phát triển thành Hybrid Recommendation hoặc Machine Learning Recommendation.

- Recommendation Engine kết hợp với RAG Chatbot tạo thành lớp AI hỗ trợ tìm kiếm, tư vấn và cá nhân hóa trải nghiệm người dùng trên EventHub.