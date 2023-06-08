# MiniCinema
Zpracoval jsem projekt na téma objednání lístků do kina a různé věci s tím související. Program se spouští v aplikaci Visual Studio. Po spuštění Vás program vyzve na stisknutí klávesy ENTER, 
díky které se připojíte do databáze. Následně se Vám ukáže menu, ve kterém si zvolíte volbu:
### 1.) Objednání lístků
Uživateli se vypíší filmy, které se právě promítají, vybere si a následně vyplní "fakturační údaje" a na závěr si vybere sedadlo. Poté se vše uloží do databáze
a pošle se email na zadaný email.
### 2.) Log-In 
=> Přihlášení = uživatel se přihlásí a následně může upravovat objednávky, mazat je a nebo např. si nechat vypsat jeho objednávky.
### 3.) Reset DB 
Zavolá se metoda, která dropne všechny tabulky v databázi a poté je ihned vytvoří zpět => "resetuji databázi" (pro přehlednost)
### 4.) Konec 
Ukončí program

## Vhodné užití
Vše je dělané intuitivně, tudíž si zákazník nepotřebuje pamatovat např. své id, ale když si chce nějak upravit objednávku, stačí zadat číslo objednávky (jako v reálném životě)
a k objednávce se dostane. Této myšlenky reálného užití jsem se snažil držet :)
### Jiné informace
Součástí projektu je diagram propojení tabulek a i samotný .sql file
