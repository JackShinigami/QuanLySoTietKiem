﻿USE QL_STK
-- Chèn dữ liệu vào bảng KHACH_HANG
INSERT INTO KHACH_HANG (CCCD, HOTEN, DIACHI)
VALUES
('123456789012', N'Nguyễn Văn A', N'Số 1, Đường ABC, Thành phố X'),
('987654321098', N'Trần Thị B', N'Số 2, Đường XYZ, Thành phố Y'),
('456789012345', N'Lê Văn C', N'Số 3, Đường DEF, Thành phố Z'),
('345678901234', N'Phạm Thị D', N'Số 4, Đường GHI, Thành phố W'),
('567890123456', N'Hoàng Văn E', N'Số 5, Đường JKL, Thành phố V');

-- Chèn dữ liệu vào bảng SO_TIET_KIEM
INSERT INTO SO_TIET_KIEM (MASO, CCCD, NGAYMOSO, NGAYDONGSO, SODU, LOAITIETKIEM, TRANGTHAI, LAISUAT, SONGAYDUOCRUT, TIENGUITOITHIEU)
VALUES
(1, '123456789012', '2023-01-01', '2023-01-31', 1000000, 1, 1, 0.05, 30, 5000000),
(2, '123456789012', '2023-02-01', '2023-02-28', 2000000, 2, 1, 0.07, 28, 10000000),
(3, '987654321098', '2023-03-01', '2023-03-31', 3000000, 1, 1, 0.05, 31, 5000000),
(4, '987654321098', '2023-04-01', '2023-04-30', 4000000, 2, 1, 0.07, 30, 10000000),
(5, '456789012345', '2023-05-01', '2023-05-31', 5000000, 1, 1, 0.05, 31, 5000000);

-- Chèn dữ liệu vào bảng PHIEU_GUI
INSERT INTO PHIEU_GUI (MAPHIEUGUI, MASO, NGAYGUI, SOTIEN)
VALUES
(1, 1, '2023-01-05', 500000),
(2, 1, '2023-01-10', 1000000),
(3, 2, '2023-02-08', 1500000),
(4, 2, '2023-02-15', 2000000),
(5, 3, '2023-03-12', 2500000);

-- Chèn dữ liệu vào bảng PHIEU_RUT
INSERT INTO PHIEU_RUT (MAPHIEURUT, MASO, NGAYRUT, SOTIEN)
VALUES
(1, 1, '2023-01-20', 500000),
(2, 1, '2023-01-25', 1000000),
(3, 2, '2023-02-22', 1500000),
(4, 2, '2023-02-28', 2000000),
(5, 3, '2023-03-25', 2500000);

-- Chèn dữ liệu vào bảng LOAI_TIET_KIEM
INSERT INTO LOAI_TIET_KIEM (KYHAN, LAISUAT)
VALUES
(0, 0.0015),
(3, 0.005),
(6, 0.0055);


-- Chèn dữ liệu vào bảng CONFIG_TOITHIEU
INSERT INTO CONFIG_TOITHIEU (ID, NGAYGUI, SOTIENGUI)
VALUES
('ConfigID',15, 100000)
