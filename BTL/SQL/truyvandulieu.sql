------------/* Truy vấn dữ liệu demo */---------------------
/* Xe đang gửi trong mỗi bãi */
SELECT B.TenBaiXe, COUNT(*) AS SoXeDangGui
FROM BaiXe B
JOIN LuotGuiXe L ON B.MaBaiXe = L.MaBaiXe
WHERE L.TrangThai = 0
GROUP BY B.TenBaiXe;

/* Số chỗ trỗng mỗi bãi */
SELECT B.TenBaiXe,
       B.TongSoCho - COUNT(L.MaLuotGui) AS SoChoTrong
FROM BaiXe B
LEFT JOIN LuotGuiXe L
     ON B.MaBaiXe = L.MaBaiXe AND L.TrangThai = 0
GROUP BY B.TenBaiXe, B.TongSoCho;


/* Lịch sử gửi xe của 1 thẻ */
SELECT *
FROM LuotGuiXe
WHERE MaThe = 'VANG01'
ORDER BY ThoiGianVao;
