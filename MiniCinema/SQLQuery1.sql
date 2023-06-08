create table zakaznik(
	id int primary key identity(1,1),
	jmeno varchar(30) not null,
	prijmeni varchar(30) not null,
	email varchar(50) not null check(email like '%@%'),
	heslo varchar(30) not null
);

create table film(
	id int primary key identity(1,1),
	nazev varchar(30) not null,
	cena varchar(20) not null,
	delka varchar(20) not null
);	


create table sedadlo(
	id int primary key identity(1,1),
	cislo int not null check(cislo > 0)
);	

create table promitani(
	id int primary key identity(1,1),
	nazev varchar(30) not null,
	film_id int not null foreign key references film(id),
	sedadlo_id int not null foreign key references sedadlo(id)
);

create table objednavka(
	id int primary key identity(1,1),
	cisloObj int not null,
	zakaznik_id int not null references zakaznik(id),
	promitani_id int not null references promitani(id)
);

