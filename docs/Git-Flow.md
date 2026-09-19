\# EventHub - Git Flow



\## 1. Mục đích



EventHub sử dụng Git Flow để quản lý source code trong quá trình phát triển hệ thống.



Mục tiêu:



\- Tách biệt code ổn định và code đang phát triển.

\- Mỗi tính năng được phát triển trên một feature branch riêng.

\- Dễ dàng kiểm tra, review và merge code.

\- Hạn chế xung đột khi nhiều thành viên cùng phát triển.



\---



\## 2. Cấu trúc Branch



EventHub sử dụng các branch chính:



```text

main

└── develop

&#x20;   ├── feature/auth-ui

&#x20;   ├── feature/auth-api

&#x20;   └── feature/refresh-token

