CREATE DATABASE QLQuanNet
GO
USE QLQuanNet
GO
CREATE TABLE TaiKhoan
(
	Ten_tk varchar(50) not null  Primary key , 
	MatKhau varchar(50),
)
GO
CREATE TABLE May
(
	May int not null primary key,

	Ten_tk varchar(50) not null,
	SoTien float,
	SoGioChoi int,
	Foreign key(Ten_tk) references TaiKhoan(Ten_tk),
	Check (SoTien>=5000)
)
GO