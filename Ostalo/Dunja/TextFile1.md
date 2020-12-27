# ODABRANA POGLAVLJA OPERATIVNIH SISTEMA
***
- [Klasa *Rasporedjivac*]
	- [Atributi]
	- [Metode]
		- [Metode za generisanje *Task-ova*]
		- [Metode za kontrolu *Thread-ova*]
- [Pokretanje *Rasporedjivaca*]
***

## Klasa *Rasporedjivac*
Klasa *Rasporedjivac* omogućava kreiranje zadataka i njihovo raspoređivanje na odgovarajući način.
Kreirani zadaci se smještaju u niz i čekaju na svoj red izvršavanja, koji zavisi od tipa raspoređivanja.

Implementirana su 2 tipa raspoređivanja:
- Nepreventivno (*non-preemptive*)
- Preventivno (*preemptive*)

Kada se pokrene raspoređivač, tada započinje izvršavanje zadataka koji su bili kreirani prije njegovog pokretanja.
Omogućeno je i naknadno dodavanje zadataka nakon što je raspoređivač započeo sa izvršavanjem zadataka.

## Atributi
Da bismo obezbijedili raspoređivanje i izvršavanje pristiglih zadataka, koriste se 2 liste u kojima se čuvaju zadaci.

Za memorisanje zadataka koristi se sortirana lista *TaskoviZaIzvrsavanje*, koja sadrži prioritet izvršavanja
zadataka i *Tuple*, čiji su elementi zadatak (akcija) koji se izvršava i vrijeme na koje je ograničeno 
njegovo izvršavanje.
Elementi sortirane liste se uvijek održavaju u takvom poretku da je zadatak sa najvećim prioritetom
(manji broj označava veći prioritet) uvijek na početku liste. Na taj način se obezbjeđuje prioritetetno
(preventivno) raspoređivanje.

U slučaju nepreventivnog raspoređivanja, kao prioritet se prosljeđuje redni broj zadatka na osnovu
vremena njegovog pristizanja u odnosu na prethodne zadatke.

Kada postoji slobodna nit koja može da primi zadatak i izvrši ga, zadatak se iz prethodne liste "prebacuje"
u novu listu - *ListaAktivnihTaskova*. Ova lista sadrži *Tuple* koji je prethodno opisan.

Pored listi, postoje i sljedeće promjenljive :
- *MaxBrojNiti* - definiše broj niti koje se mogu koristiti za raspoređivanje
- *BrojAktivnihNiti* - definiše broj niti koje trenutno izvršavaju zadatke
- *UkupnoVrijemeIzvrsavanja* - ograničava vrijeme trajanja rasporedjivanja zadataka
- *Time*- mjeri ukupno vrijeme izvršavanja
- *Identifikator* - određuje da li se radi preventivno (1) ili nepreventivno (0) raspoređivanje

Takođe, postoje i 3 zasebne niti, koje vrše odgovarajuću kontrolu procesa raspoređivanja.
## Metode
## Metode za generisanje *Task-ova*
Za rad sa listama i dodavanje zadataka u liste, koriste se sljedeće metode:
- *GenerisanjeTaska* 
	
	``` public void GenerisanjeTaska(int i, int time); ```

	Ova metoda prima redni broj taska na osnovu vremena njegovog pristizanja, koji je određen for petljom.
	Takođe prima i broj sekundi na koje je potrebno ograničiti vrijeme izvršavanja zadatka.
	U ovoj metodi se vrši pozivanje funkcije *DodajTask*, a kao argument joj se prosljeđuje novokreirana 
	akcija sa lambda funkcijom, koja sadrži ispise o zadatku.

- *DodajTask*

	``` public void DodajTask(Action akcija);```

	Ova metoda kao argument prima zadatak (akciju). Na osnovu vrijednosti promjenljive *Identifikator* 
	određuje se da li se radi o preventivnom ili nepreventivnom resporedjivanju. Ako je vrijednost 0,
	zadatak se dodaje u listu sa prioritetom koji je jedna njegovom rednom broju (po redosljedu pristizanja),
	a ako je vrijednost 1, onda se generiše *random* vrijednost koja će biti proslijedjena kao prioritet
	pristiglog zadatka. U listu se prosljedjuje i vrijeme na koje je ograničeno izvršavanje svakog taska i
	u ovom slučaju ono iznosi 5 sekundi.

- PokretanjeTaska

	``` public void PokretanjeTaska();```

	Ova metoda nema argumenete. Najprije se vrši provjera da li je lista *TaskoviZaIzvršavanje* prazna.
	Ako nije, vrš se "skidanje" zadataka sa početka te liste i stavlja u listu *ListaAktivnihTaskova*,
	pri čemu se povećava broj aktivnih niti.

	U ovoj funkciji se kreira nit koja započinje izvršavanje zadatka :

	``` Task.Factory.StartNew(akcija.Item1, new CancellationTokenSource(akcija.Item2).Token);```

	Funkciji je proslijedjen i *CancellationToken* koji se koristi za prekidanje zadatka u slučaju 
	prekoračenja ograničenog vremena.
## Metode za kontrolu *Thread-ova*
Metode koje se koriste za kontrolu niti (*Thred-ova*) izvršavaju se na zasebnim nitima u obliku 
beskonačne petlje ``` while(true)```.

- ProvjeraSlobodnih

	``` public void ProvjeraSlobodnih();```

	Unutar ove funkcije vrši se kontunualna provjera da li postoji slobodna nit koja može da preuzme 
	izvšavanje zadatka koje čeka na svoje izvšavanje. Ta provjera se bazira na poredjenju vrijednosti
	dvije promjenljive - ako je *BrojAktivnihNiti* manji od *MaxBrojaNiti*, tada se poziva funkcija 
	*PokretanjeTaska*.

	Ova funkcija takođe vrši provjeru da li je prekoračeno ukupno vrijeme izvršavanja.

- ProvjeraZavršenih

	``` public void ProvjeraZavrsenih();```

	Ova funkcija prolazi kroz listu *ListaAktivnihTaskova*, pri čemu se vrši provjera da li je neki
	od taskova već završen ili je prekinut ( stanja *RanToCompletion* i *Canceled*).
	Ukoliko postoji takav zadatak, on se uklanja iz liste i vrši se dekrementovanje broja aktivnih zadataka.

# Pokretanje *Rasporedjivaca*

Radi demonstracije zadatka kreirana su dva objekta tipa *Rasporedjivac* :

```Rasporedjivac RasporedjivacBezPrioriteta = new Rasporedjivac(3, 15, 0);```
```Rasporedjivac RasporedjivacSaPrioritetom = new Rasporedjivac(4, 20, 1);```

Za klasu *Rasporedjivac* implementiran je konstruktor koji prizvata 3 argumenta :

```Rasporedjivac(int BrojNiti, int vrijeme, int identifikator)```

Prvi argument *BrojNiti* predstavlja maksimalan broj niti koji raspoređivač zadataka može da koristi prilikom 
njihovog raspoređivanja.

Argument *vrijeme* predstavlja vrijeme u sekundama nakon kojeg treba da se obustavi proces raspoređivanja.

Kao posljednji argument prosljedjuje se *identifikator* - broj kojim se određuje tip raspoređivača.

***


	


