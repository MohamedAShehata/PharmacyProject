create database Pharmacyadb;
use Pharmacyadb;
create table Pharmacist (
user_name varchar(20) not null primary key ,
pharmacist_name varchar(30) not null,
SSN varchar(15) not null unique,
phone varchar(14) not null
 );
create table Bill (
bill_ID int not null auto_increment primary key ,
date_of_purchase date not null ,
seller_username varchar(20) not null,
foreign key (seller_username) references Pharmacist (user_name) 
);

create table Medicine(
medicine_ID int not null auto_increment  primary key ,
medicine_name varchar(30) not null,
price float not null,
effective_material varchar(30) not null,
quantity int not null ,
expiration_date date not null,
unit varchar(30) not null,
UNIQUE (medicine_name ,unit)
);
create table Bill_Medicine(
bill_ID int not null ,
medicine_ID int not null ,
quantity int not null ,
primary key(bill_ID,medicine_ID),
foreign key(bill_id) references Bill (bill_ID),
foreign key (medicine_id) references Medicine (medicine_ID)
);


insert into Medicine values(null,'Panadol',30,'Paracetamol',100,'2023-01-01','Tablets'),
(null,'Adol',40,'Paracetamol',50,'2023-05-10','Capsules'),
(null,'Methamol',25,'Paracetamol',30,'2022-07-10','Syrup'),
(null,'Cataflam',36,'Declofenac',35,'2022-10-10','Tablets'),
(null,'Catafast',24,'Declofenac',80,'2022-12-5','Granules'),
(null,'Voltaren',24,'Diclofenac',80,'2022-12-5','Ampoule'),
(null,'Rani',15,'Ranitidine',60,'2022-7-01','Granules'),
(null,'Rantag',10,'Ranitidine',50,'2022-12-01','Tablets'),
(null,'Tramadol',10,'Tramadol',50,'2022-12-01','Ampoule'),
(null,'Rani',2,'Ranitidine',100,'2022-10-01','Tablets'),
(null,'Contramal',30,'Tramadol',20,'2023-01-12','Tablets'),
(null,'Farcopril',35,'Captopri',30,'2021-12-20','Tablets'),
(null,'Tenormin',45,'Atenolol',10,'2023-05-05','Tablets'),
(null,'Teklo',25,'Atenolol',30,'2022-12-11','Tablets'),
(null,'Lipiless',15.5,'Atorvastatin',20,'2022-05-11','Tablets'),
(null,'MEGAMOX',45.5,'AMOXICILLIN',10,'2022-12-21','Tablets');


insert into pharmacist values('Alzhraa','Alzhraa Yousef','29934578193452','1432590865423'),
('Alyaa','Alyaa Ahmed','29835558193489','1432590333323'),
('Dina','Dina Abozaid','29992378193450','1433254865424'),
('Elshimaa','Elshimaa Mohammed','29734566663452','1434490865467'),
('Abdelrahman','Abdelrahman Maher','29834578175342','1436590865478'),
('Mohammed','Mohammed Ahmed','29734578193452','1474590865490'),
('root','admin','29834578175202','01436590865478');


insert into bill values(null,'2021-07-10','Alzhraa'),
(null,'2021-03-5','Elshimaa'),
(null,'2021-04-12','Mohammed'),
(null,'2021-10-9','Dina'),
(null,'2021-02-3','Abdelrahman'),
(null,'2021-11-4','Alzhraa'),
(null,'2021-10-9','Dina'),
(null,'2020-02-3','Elshimaa'),
(null,'2020-11-4','Alzhraa'),
(null,'2020-02-5','Alyaa');


insert into bill_medicine values(1,2,2),(1,4,1),(1,5,1),(2,6,1),(2,2,2),
(3,1,1),(3,2,2),(4,5,1),(5,6,1),(6,2,2),(7,3,2),(8,1,1),(9,1,1),(10,5,1),
(8,5,2),(10,3,2),(10,2,1);


delimiter //
create procedure getQuantityPrice(name varchar(30),Unit varchar(30))
begin
select quantity,price from medicine where medicine_name=name and unit=Unit;
end //
delimiter ;



delimiter //
create procedure getMedicineID(name varchar(30),Unit varchar(30))
begin
select medicine_ID from medicine where medicine_name=name and unit=Unit;
end //
delimiter ;


delimiter //
create trigger reduceQuantity
before insert on bill_medicine for each row
begin
update medicine set medicine.quantity=medicine.quantity-new.quantity where medicine.medicine_ID=new.medicine_ID;
end //
delimiter ;



CREATE USER 'Alzhraa'@'localhost' IDENTIFIED BY 'password1';
CREATE USER 'Alyaa'@'localhost' IDENTIFIED BY 'password2';
CREATE USER 'Dina'@'localhost' IDENTIFIED BY 'password3';
CREATE USER 'Elshimaa'@'localhost' IDENTIFIED BY 'password4';
CREATE USER 'Abdelrahman'@'localhost' IDENTIFIED BY 'password5';
CREATE USER 'Mohammed'@'localhost' IDENTIFIED BY 'password6';

 create user 'c'@'localhost'  IDENTIFIED BY  'c'; 
 select * from mysql.user;


GRANT insert , select on pharmacyadb.Bill TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;

GRANT select on pharmacyadb.medicine TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;

GRANT insert,select on pharmacyadb.Bill_Medicine TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;

GRANT update(quantity) on pharmacyadb.medicine TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;

GRANT select(pharmacist_name,user_name) on pharmacyadb.Pharmacist TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;


GRANT EXECUTE ON PROCEDURE getQuantityPrice TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;
GRANT EXECUTE ON PROCEDURE getMedicineID TO Alzhraa@localhost,Alyaa@localhost,
Dina@localhost,Elshimaa@localhost,Abdelrahman@localhost,Mohammed@localhost;