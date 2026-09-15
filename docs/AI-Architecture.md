# EventHub AI Architecture

## 1. Mục tiêu

EventHub dự kiến tích hợp AI nhằm hỗ trợ người dùng tìm kiếm và lựa chọn
sự kiện phù hợp.

Hai chức năng AI chính:

- AI Chatbot sử dụng kiến trúc RAG.
- Recommendation Engine đề xuất sự kiện phù hợp với người dùng.

---

# 2. AI Chatbot sử dụng RAG

## 2.1. Mục tiêu

Chatbot hỗ trợ người dùng hỏi thông tin liên quan đến các sự kiện trên
hệ thống.

Ví dụ:

- "Cuối tuần này có concert nào không?"
- "Có workshop nào ở Huế không?"
- "Sự kiện này tổ chức ở đâu?"
- "Giá vé VIP của sự kiện này bao nhiêu?"
- "Sự kiện nào phù hợp với sinh viên?"

Chatbot cần sử dụng dữ liệu của EventHub để tạo câu trả lời phù hợp.

---

## 2.2. Kiến trúc RAG

```text
User
  |
  v
AI Chatbot
  |
  v
RAG Service
  |
  +----------------------+
  |                      |
  v                      v
Knowledge Base       Vector Database
  |                      |
  +----------+-----------+
             |
             v
       Relevant Context
             |
             v
            LLM
             |
             v
        AI Response
             |
             v
            User


## 2.3. Quy trình hoạt động
Bước 1 - Người dùng đặt câu hỏi

    Ví dụ:

        "Tìm cho tôi các workshop ở Huế trong tháng này."
Bước 2 - Xử lý câu hỏi

    RAG Service phân tích câu hỏi và tạo embedding cho nội dung cần tìm kiếm.

Bước 3 - Tìm kiếm dữ liệu

    Hệ thống tìm kiếm các thông tin liên quan trong Knowledge Base và Vector Database.

    Dữ liệu có thể bao gồm:

        Tên sự kiện
        Mô tả sự kiện
        Thời gian
        Địa điểm
        Danh mục
        Loại vé
        Giá vé
Bước 4 - Retrieval

    Các dữ liệu liên quan nhất được lấy ra làm context.

Bước 5 - Generation

    Context và câu hỏi của người dùng được gửi tới LLM.

    LLM sử dụng context để tạo câu trả lời.

Bước 6 - Trả kết quả

    Chatbot trả câu trả lời cho người dùng.

# 3. Knowledge Base

    Knowledge Base được xây dựng từ dữ liệu EventHub.

    Nguồn dữ liệu chính:

    EventService
        |
        +-- Events
        +-- Categories
        +-- Venues
        +-- TicketTypes

    Dữ liệu có thể được đồng bộ định kỳ hoặc khi có sự kiện mới/cập nhật.

# 4. Vector Database

    Vector Database lưu embedding của dữ liệu sự kiện.

    Mỗi tài liệu có thể chứa:

        Event ID
        Event Name
        Description
        Category
        Venue
        Start Time
        End Time
        Ticket Information

    Vector Database được sử dụng để tìm kiếm thông tin theo ngữ nghĩa.

# 5. RAG Flow
    User Question
          |
          v
    Question Processing
          |
          v
    Embedding
          |
          v
    Vector Search
          |
          v
    Relevant Event Data
          |
          v
    Prompt + Context
          |
          v
         LLM
          |
          v
       Answer

# 6. Recommendation Engine
## 6.1. Mục tiêu

    Recommendation Engine đề xuất các sự kiện phù hợp với sở thích và
hành vi của người dùng.

    Ví dụ:

    Người dùng thường xem:
    - Concert
    - Music Festival

    Hệ thống có thể đề xuất:
    - Concert A
    - Concert B
    - Music Festival C

## 6.2. Dữ liệu đầu vào

    Recommendation Engine có thể sử dụng:
        Lịch sử xem sự kiện.
        Lịch sử đặt vé.
        Danh mục sự kiện đã quan tâm.
        Sự kiện đã yêu thích.
        Thành phố/địa điểm thường quan tâm.
        Khoảng thời gian sự kiện.
        Loại vé đã lựa chọn.

## 6.3. Kiến trúc
    User Behavior
          |
          v
    Recommendation Engine
          |
          +----------------+
          |                |
          v                v
    User Profile       Event Data
          |                |
          +-------+--------+
                  |
                  v
           Recommendation Model
                  |
                  v
           Ranked Events
                  |
                  v
              EventHub Web

# 7. Recommendation Flow
    User
     |
     +-- View Event
     |
     +-- Favorite Event
     |
     +-- Book Ticket
     |
     +-- Search Event
     |
     v
    User Behavior Data
     |
     v
    Recommendation Engine
     |
     v
    Calculate Relevance Score
     |
     v
    Rank Events
     |
     v
    Recommended Events

# 8. Giai đoạn triển khai
Giai đoạn 1

    Nghiên cứu kiến trúc và chuẩn bị dữ liệu.

    Event Data
       |
       v
    Knowledge Base
Giai đoạn 2

    Xây dựng RAG Chatbot.

    User
     |
     v
    Chatbot
     |
     v
    RAG
     |
     v
    LLM
Giai đoạn 3

    Xây dựng Recommendation Engine.

    User Behavior
     |
     v
    Recommendation Model
     |
     v
    Recommended Events

# 9. Tích hợp với Microservices

    AI không truy cập trực tiếp database của các Microservice.

    Kiến trúc dự kiến:

                    +----------------+
                    | EventService   |
                    +-------+--------+
                            |
                            v
                    +---------------+
                    | AI/RAG Layer  |
                    +-------+-------+
                            |
                 +----------+----------+
                 |                     |
                 v                     v
          Vector Database       Recommendation
                 |                  Engine
                 +---------+---------+
                           |
                           v
                       AI Chatbot

    AI Layer giao tiếp với các Microservice thông qua API hoặc cơ chế
message/event phù hợp.

# 10. Công nghệ dự kiến

    Các công nghệ cụ thể sẽ được lựa chọn trong giai đoạn triển khai.

    Kiến trúc cần đảm bảo:
        Có khả năng mở rộng.
        Tách biệt AI Layer khỏi các Microservice nghiệp vụ.
        Có thể thay đổi LLM.
        Có thể thay đổi Vector Database.
        Có thể cập nhật Knowledge Base khi dữ liệu sự kiện thay đổi.


# 11. Kết luận

    AI trong EventHub được thiết kế thành hai chức năng chính:
        RAG Chatbot: hỗ trợ người dùng hỏi và tìm kiếm thông tin sự kiện.
        Recommendation Engine: đề xuất sự kiện dựa trên dữ liệu và hành vi người dùng.

    Hai thành phần được thiết kế độc lập với các Microservice nghiệp vụ
nhưng có thể lấy dữ liệu thông qua API hoặc event/message.