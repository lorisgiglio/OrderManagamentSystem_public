# Order Management Solution

## Versione .NET
8.0

## Database
Il database utilizzato è SQLite, serverless, scelto per la sua semplicità e facilità di configurazione.

Ogni API ha il proprio database e il proprio contesto di database e le proprie migrazioni per gestire la creazione 
e l'aggiornamento del database.

All'avvio di ogni API, viene verificato se il database associato esiste ed eventualmente creato
nel percorso dell'eseguibile (coerente con la connection string definita in appsettings.json).

Le tabelle hanno chiavi primarie, vincoli di univocità e hanno campi mandatori dove necessario 
(es: non possono esserci due categorie con lo stesso nome, deve per forza esserci un nome di categoria).


## Autenticazione
Nessuna.

## Modello di Progetto
Clean Architecture.

## Pattern Utilizzati
Repository Pattern, Factory Pattern, Dependency Injection.

## Logging
E' stato predisposto logging su file (Serilog) oltre al logging di default in console.

## Testing
Gli unit test sono stati implementati utilizzando xUnit, Moq e FluentAssertions sotto la cartella Test

## NuGet Packages
Versioni centralizzate in Directory.Packages.props

## Host
lauchProfile 'https'
Address API : localhost:7000 (HTTPS)
Category API: localhost:7001 (HTTPS)
Order API   : localhost:7002 (HTTPS)
Product API : localhost:7003 (HTTPS)
User API    : localhost:7004 (HTTPS)


## Organizzazione dell'Applicazione
L'applicazione è composta da 5 API distinte sotto la cartella Api.

Gli endpoint, nel caso di aggiornamenti o inserimenti, utilizzano dei DTO per mostrare solo i campi richiesti; negli altri casi, 
se necessario, mostrano l'oggetto associato alla tabella del db.

Gli endpoint sono esposti tramite Minimal API per semplicità e velocità di sviluppo 
(le API vengono definite da codice da \API\Endpoints\ di ciascun progetto API).

La gestione degli errori è stata implementata tramite middleware per intercettare gli errori 
ma non è stata definita una specifica strategia in base al tipo di errore 
(es: una violazione di chiave è un errore generico gestito).

## Validazione dei dati

Per quanto riguarda gli oggetti con dipendenze (es: Order che dipende da User, Product, ...) le soluzioni che ho analizzato sono:

1- cache interna ad ogni api, con GET della dipendenza mantenuta in memoria e con riciclo periodico;

   (rischio elevato di incoerenza dei dati tra oggetto e dipendenza)

2- dati delle dipendenze in cache ad esempio con una cache distribuita che si auto aggiorna 
   (Categories, Users, Products in cache);

   (esiste una minima possibilità di incoerenza dei dati con cache molto grandi e lentezza di refresh della cache);

3- validazione sincrona delle dipendenze tramite chiamate alle API delle dipendenze durante l'inserimento 
   dell'oggetto principale;

   (in caso di indisponibilità dell'API dipendente l'inserimento fallisce bloccando tutti gli inserimenti);

4- validazione asincrona delle dipendenze tramite Background Worker che verifica la consistenza dei dati 
   solo dopo l'inserimento;

   (soluzione più robusta, gli oggetti appena inseriti e da validare sono marcati come "Draft" 
   finchè non sono validati "Ready" o rifiutati "Refused", in caso di errori di validazione ad esempio per prodotto mancante)


## Soluzione Implementata

Ho implementato la soluzione 4 aggiungendo a tutte le API un endpoint che accetta gli id da validare: 
esempio: l'Api di Product prevede anche un /products/check-ids che accetta una lista di Id (product Id) e 
restituisce uno stato che indica se gli id richiesti sono tutti validi o meno. 
Nella verifica delle dipendenze se un id di prodotto da verificare viene trovato con stato REFUSED o DRAFT non viene considerato valido.

Se sono tutti validi (ad esempio insieme alle altre validazioni come su User e Address) l'ordine viene spostato 
in stato READY altrimenti in REFUSED.

Tutte le entità associate alle API ereditano un IStatus che prevede uno stato iniziale di tipo enum:
Draft - in bozza che è lo stato iniziale all'inserimento,
Ready - ha superato le validazioni delle dipendenze,
Refused - non ha superato le validazioni delle dipendenze,
NotNeeded - stato per entità che non richiedono validazioni esterne (es: Category).

Viene gestito anche il caso di indisponibilità delle API dipendenti reiterando il controllo in modo continuativo fino
al ritorno della disponibilità del servizio. In ogni caso nessuna API viene interrotta o bloccata.


## Possibili Miglioramenti 

Potevo definire in aggiunta alcune policy di controllo per un numero definito retry e frequenza dopodichè gestire 
l'ordine in qualche modo.

Per gli elementi REFUSED per errori nelle dipendenze sarebbe da implementare una politica di 
gestione (es: notifica, cancellazione)

La classe di validazione del Background Worker (ValidationBackgroundService)
poteva essere implementata in modo ancora più generico e configurabile.


## Esecuzione dell'Applicazione

Per eseguire l'applicazione, aprire la soluzione in Visual Studio 2022 o superiore.

- Tasto destro sulla soluzione > Proprietà
- Scegliere "Progetti di avvio multiplo" (Multiple startup projects)
- Selezionare i progetti da avviare se non già selezionati :
  
  * OrderManagement.Api.Adresses  - Action: Start - Debug Target: https
  * OrderManagement.Api.Categories- Action: Start - Debug Target: https
  * OrderManagement.Api.Orders    - Action: Start - Debug Target: https
  * OrderManagement.Api.Products  - Action: Start - Debug Target: https
  * OrderManagement.Api.Users     - Action: Start - Debug Target: https

Salvare e avviare il debug che è configurato come multi-progetto per avviare tutte le API contemporaneamente.

All'avvio :
- verrà creato il database per ciascuna Api se non esiste;
- si aprirano le console per ciascuna Api;
- si apriranno le pagine degli Swagger per ciascuna API che si potranno utilizzare per simulare delle chiamate.


